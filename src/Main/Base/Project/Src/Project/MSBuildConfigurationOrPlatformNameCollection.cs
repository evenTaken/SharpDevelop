﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using Microsoft.Build.Construction;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// The collection for <see cref="MSBuildBasedProject.ConfigurationNames"/> and <see cref="MSBuildBasedProject.PlatformNames"/>.
	/// </summary>
	class MSBuildConfigurationOrPlatformNameCollection : IConfigurationOrPlatformNameCollection
	{
		public event ModelCollectionChangedEventHandler<string> CollectionChanged;
		
		volatile IReadOnlyList<string> listSnapshot = EmptyList<string>.Instance;
		readonly MSBuildBasedProject project;
		readonly bool isPlatform;
		
		public MSBuildConfigurationOrPlatformNameCollection(MSBuildBasedProject project, bool isPlatform)
		{
			this.project = project;
			this.isPlatform = isPlatform;
		}
		
		internal void SetContents(IEnumerable<string> updatedItems)
		{
			this.listSnapshot = updatedItems.ToArray();
		}
		
		internal void OnCollectionChanged(IReadOnlyCollection<string> oldItems, IReadOnlyCollection<string> newItems)
		{
			if (oldItems.SequenceEqual(newItems))
				return;
			var eh = CollectionChanged;
			if (eh != null)
				eh(oldItems, newItems);
		}
		
		#region IReadOnlyCollection implementation
		
		public IReadOnlyCollection<string> CreateSnapshot()
		{
			return listSnapshot;
		}
		
		public int Count {
			get {
				return listSnapshot.Count;
			}
		}
		
		public IEnumerator<string> GetEnumerator()
		{
			return listSnapshot.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return listSnapshot.GetEnumerator();
		}
		
		#endregion
		
		public string ValidateName(string name)
		{
			if (name == null)
				return null;
			name = name.Trim();
			if (!ConfigurationAndPlatform.IsValidName(name))
				return null;
			if (isPlatform)
				return MSBuildInternals.FixPlatformNameForProject(name);
			else
				return name;
		}
		
		string GetName(ConfigurationAndPlatform config)
		{
			return isPlatform ? config.Platform : config.Configuration;
		}
		
		bool HasName(ConfigurationAndPlatform config, string name)
		{
			return ConfigurationAndPlatform.ConfigurationNameComparer.Equals(GetName(config), name);
		}
		
		ConfigurationAndPlatform SetName(ConfigurationAndPlatform config, string newName)
		{
			if (isPlatform)
				return new ConfigurationAndPlatform(config.Configuration, newName);
			else
				return new ConfigurationAndPlatform(newName, config.Platform);
		}
		
		void IConfigurationOrPlatformNameCollection.Add(string newName, string copyFrom)
		{
			SD.MainThread.VerifyAccess();
			newName = ValidateName(newName);
			if (newName == null)
				throw new ArgumentException();
			lock (project.SyncRoot) {
				var projectFile = project.MSBuildProjectFile;
				var userProjectFile = project.MSBuildUserProjectFile;
				bool copiedGroupInMainFile = false;
				if (copyFrom != null) {
					foreach (ProjectPropertyGroupElement g in projectFile.PropertyGroups.ToList()) {
						var gConfig = ConfigurationAndPlatform.FromCondition(g.Condition);
						if (HasName(gConfig, copyFrom)) {
							CopyProperties(projectFile, g, SetName(gConfig, newName));
							copiedGroupInMainFile = true;
						}
					}
					foreach (ProjectPropertyGroupElement g in userProjectFile.PropertyGroups.ToList()) {
						var gConfig = ConfigurationAndPlatform.FromCondition(g.Condition);
						if (HasName(gConfig, copyFrom)) {
							CopyProperties(userProjectFile, g, SetName(gConfig, newName));
						}
					}
				}
				if (!copiedGroupInMainFile) {
					projectFile.AddPropertyGroup().Condition = (isPlatform ? new ConfigurationAndPlatform(null, newName) : new ConfigurationAndPlatform(newName, null)).ToCondition();
				}
				project.LoadConfigurationPlatformNamesFromMSBuild();
				
				// Adjust mapping:
				// If the new config/platform already exists in the solution and is mapped to some old project config/platform,
				// re-map it to the new config/platform.
				var mapping = project.ConfigurationMapping;
				if (isPlatform) {
					string newNameForSolution = MSBuildInternals.FixPlatformNameForSolution(newName);
					if (project.ParentSolution.PlatformNames.Contains(newNameForSolution, ConfigurationAndPlatform.ConfigurationNameComparer)) {
						foreach (string solutionConfiguration in project.ParentSolution.ConfigurationNames) {
							var solutionConfig = new ConfigurationAndPlatform(solutionConfiguration, newNameForSolution);
							var projectConfig = mapping.GetProjectConfiguration(solutionConfig);
							mapping.SetProjectConfiguration(solutionConfig, SetName(projectConfig, newName));
						}
					}
				} else {
					if (project.ParentSolution.ConfigurationNames.Contains(newName, ConfigurationAndPlatform.ConfigurationNameComparer)) {
						foreach (string solutionPlatform in project.ParentSolution.PlatformNames) {
							var solutionConfig = new ConfigurationAndPlatform(newName, solutionPlatform);
							var projectConfig = mapping.GetProjectConfiguration(solutionConfig);
							mapping.SetProjectConfiguration(solutionConfig, SetName(projectConfig, newName));
						}
					}
				}
				project.ActiveConfiguration = mapping.GetProjectConfiguration(project.ParentSolution.ActiveConfiguration);
			}
		}
		
		/// <summary>
		/// copy properties from g into a new property group for newConfiguration and newPlatform
		/// </summary>
		void CopyProperties(ProjectRootElement project, ProjectPropertyGroupElement g, ConfigurationAndPlatform newConfig)
		{
			ProjectPropertyGroupElement ng = project.AddPropertyGroup();
			ng.Condition = newConfig.ToCondition();
			foreach (var p in g.Properties) {
				ng.AddProperty(p.Name, p.Value).Condition = p.Condition;
			}
		}
		
		/// <summary>
		/// Finds the &lt;Configuration&gt; or &lt;Platform&gt; element in this property group.
		/// </summary>
		ProjectPropertyElement FindConfigElement(ProjectPropertyGroupElement g)
		{
			return g.Properties.FirstOrDefault(p => MSBuildInternals.PropertyNameComparer.Equals(p.Name, isPlatform ? "Platform" : "Configuration"));
		}
		
		void IConfigurationOrPlatformNameCollection.Remove(string name)
		{
			SD.MainThread.VerifyAccess();
			lock (project.SyncRoot) {
				string otherName = null;
				foreach (string configName in this) {
					if (!ConfigurationAndPlatform.ConfigurationNameComparer.Equals(configName, name)) {
						otherName = name;
						break;
					}
				}
				if (otherName == null) {
					throw new InvalidOperationException("cannot remove the last configuration/platform");
				}
				foreach (ProjectPropertyGroupElement g in project.MSBuildProjectFile.PropertyGroups.Concat(project.MSBuildUserProjectFile.PropertyGroups).ToList()) {
					ProjectPropertyElement prop = FindConfigElement(g);
					if (prop != null && ConfigurationAndPlatform.ConfigurationNameComparer.Equals(prop.Value, name)) {
						prop.Value = otherName;
					}
					
					var gConfig = ConfigurationAndPlatform.FromCondition(g.Condition);
					if (HasName(gConfig, name)) {
						g.Parent.RemoveChild(g);
					}
				}
				project.LoadConfigurationPlatformNamesFromMSBuild();
				
				AdjustMapping(name, otherName);
			}
		}
		
		void IConfigurationOrPlatformNameCollection.Rename(string oldName, string newName)
		{
			newName = ValidateName(newName);
			if (newName == null)
				throw new ArgumentException();
			
			lock (project.SyncRoot) {
				foreach (ProjectPropertyGroupElement g in project.MSBuildProjectFile.PropertyGroups.Concat(project.MSBuildUserProjectFile.PropertyGroups)) {
					// Rename the default configuration setting
					ProjectPropertyElement prop = FindConfigElement(g);
					if (prop != null && ConfigurationAndPlatform.ConfigurationNameComparer.Equals(prop.Value, oldName)) {
						prop.Value = newName;
					}
					
					// Rename the configuration in conditions
					var gConfig = ConfigurationAndPlatform.FromCondition(g.Condition);
					if (HasName(gConfig, oldName)) {
						g.Condition = SetName(gConfig, newName).ToCondition();
					}
				}
				project.LoadConfigurationPlatformNamesFromMSBuild();
				
				AdjustMapping(oldName, newName);
			}
		}
		
		void AdjustMapping(string oldName, string newName)
		{
			var mapping = project.ConfigurationMapping;
			foreach (string solutionConfiguration in project.ParentSolution.ConfigurationNames) {
				foreach (string solutionPlatform in project.ParentSolution.PlatformNames) {
					var solutionConfig = new ConfigurationAndPlatform(solutionConfiguration, solutionPlatform);
					var projectConfig = mapping.GetProjectConfiguration(solutionConfig);
					if (HasName(projectConfig, oldName))
						mapping.SetProjectConfiguration(solutionConfig, SetName(projectConfig, newName));
				}
			}
			// Adjust active configuration:
			if (HasName(project.ActiveConfiguration, oldName))
				project.ActiveConfiguration = SetName(project.ActiveConfiguration, newName);
		}
	}
}
