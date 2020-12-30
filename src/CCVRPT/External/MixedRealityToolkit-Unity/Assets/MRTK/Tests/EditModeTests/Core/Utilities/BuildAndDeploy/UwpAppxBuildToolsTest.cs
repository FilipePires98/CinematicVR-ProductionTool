﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Build.Editor;
using NUnit.Framework;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Build.Editor
{
    class UwpAppxBuildToolsTest
    {
        #region AppX Manifest Tests
        // A string version of a sample manifest that gets generated by the Unity build.
        // Used in lieu of an actual file manifest to avoid having to do I/O during tests.
        // Note that the xml declaration must be on the first line, otherwise parsing
        // will fail.
        private const string TestManifest = @"<?xml version='1.0' encoding='utf-8'?>
            <Package xmlns:mp='http://schemas.microsoft.com/appx/2014/phone/manifest'
                     xmlns:uap='http://schemas.microsoft.com/appx/manifest/uap/windows10'
                     xmlns:uap2='http://schemas.microsoft.com/appx/manifest/uap/windows10/2'
                     xmlns:uap3='http://schemas.microsoft.com/appx/manifest/uap/windows10/3'
                     xmlns:uap4='http://schemas.microsoft.com/appx/manifest/uap/windows10/4'
                     xmlns:iot='http://schemas.microsoft.com/appx/manifest/iot/windows10'
                     xmlns:mobile='http://schemas.microsoft.com/appx/manifest/mobile/windows10'
                     IgnorableNamespaces='uap uap2 uap3 uap4 mp mobile iot'
                     xmlns='http://schemas.microsoft.com/appx/manifest/foundation/windows10'>
              <Identity Name='Microsoft.MixedReality.Toolkit' Publisher='CN=Microsoft' Version='2.6.0.0' />
              <mp:PhoneIdentity PhoneProductId='85c8bcd4-fbac-44ed-adf6-bfc01242a27f' PhonePublisherId='00000000-0000-0000-0000-000000000000' />
              <Properties>
                <DisplayName>MixedRealityToolkit</DisplayName>
                <PublisherDisplayName>Microsoft</PublisherDisplayName>
                <Logo>Assets\StoreLogo.png</Logo>
              </Properties>
              <Dependencies>
                <TargetDeviceFamily Name='Windows.Universal' MinVersion='10.0.10240.0' MaxVersionTested='10.0.18362.0' />
              </Dependencies>
              <Resources>
                <Resource Language='x-generate' />
              </Resources>
              <Applications>
                <Application Id='App' Executable='$targetnametoken$.exe' EntryPoint='Microsoft.MixedReality.Toolkit.App'>
                  <uap:VisualElements DisplayName='MixedRealityToolkit' 
                                      Square150x150Logo='Assets\Square150x150Logo.png'
                                      Square44x44Logo='Assets\Square44x44Logo.png'
                                      Description='Microsoft.MixedReality.Toolkit'
                                      BackgroundColor='transparent'>
                    <uap:DefaultTile ShortName='MixedRealityToolkit' Wide310x150Logo='Assets\Wide310x150Logo.png' />
                    <uap:SplashScreen Image='Assets\SplashScreen.png' BackgroundColor='#FFFFFF' />
                    <uap:InitialRotationPreference>
                      <uap:Rotation Preference='landscapeFlipped' />
                    </uap:InitialRotationPreference>
                  </uap:VisualElements>
                </Application>
              </Applications>
              <Capabilities>
                <Capability Name='internetClient' />
                <uap:Capability Name='musicLibrary' />
                <uap2:Capability Name='spatialPerception' />
                <DeviceCapability Name='microphone' />
              </Capabilities>
            </Package>
        "; // end of TestManifest

        /// <summary>
        /// Validates that AddGazeInputCapability will add a gazeInput
        /// capability to an existing well-formed manifest.
        /// </summary>
        [Test]
        public void TestAddGazeInputCapability_CapabilitiesNodeExists()
        {
            XElement rootElement = XElement.Parse(TestManifest);
            UwpAppxBuildTools.AddGazeInputCapability(rootElement);
            AssertSingleGazeInputCapability(rootElement);
        }

        /// <summary>
        /// Validates that AddGazeInputCapability will also add the
        /// <Capabilities></Capabilities> container tag if it's missing.
        /// </summary>
        [Test]
        public void TestAddGazeInputCapability_CapabilitiesNodeMissing()
        {
            XElement rootElement = XElement.Parse(TestManifest);
            XElement capabilitiesElement = rootElement.Element(rootElement.GetDefaultNamespace() + "Capabilities");
            capabilitiesElement.Remove();

            // Not technically necessary, but sanity checks that we aren't
            // spuriously testing the same thing as
            // TestAddGazeInputCapability_CapabilitiesNodeExists
            Assert.IsNull(rootElement.Element(rootElement.GetDefaultNamespace() + "Capabilities"));

            UwpAppxBuildTools.AddGazeInputCapability(rootElement);

            AssertSingleGazeInputCapability(rootElement);
        }

        /// <summary>
        /// Validates that AddGazeInputCapability will only add the gaze
        /// input capability if it doesn't already exist.
        /// </summary>
        [Test]
        public void TestAddGazeInputCapability_AddsOnce()
        {
            XElement rootElement = XElement.Parse(TestManifest);
            UwpAppxBuildTools.AddGazeInputCapability(rootElement);
            AssertSingleGazeInputCapability(rootElement);

            UwpAppxBuildTools.AddGazeInputCapability(rootElement);
            AssertSingleGazeInputCapability(rootElement);
        }

        private static void AssertSingleGazeInputCapability(XElement rootElement)
        {
            var gazeInputCapabilities = rootElement
                .Descendants(rootElement.GetDefaultNamespace() + "DeviceCapability")
                .Where(element => element.Attribute("Name")?.Value == "gazeInput")
                .ToList();

            Assert.AreEqual(1, gazeInputCapabilities.Count,
                            "There be a single gazeInput capability");
        }

        /// <summary>
        /// Validates that AddResearchModeCapability will add a perceptionSensorsExperimental
        /// capability and its namespace to an existing well-formed manifest.
        /// </summary>
        [Test]
        public void TestAddResearchModeCapability_CapabilitiesNodeExists()
        {
            XElement rootElement = XElement.Parse(TestManifest);
            UwpAppxBuildTools.AddResearchModeCapability(rootElement);
            AssertSingleResearchModeCapability(rootElement);
        }



        /// <summary>
        /// Validates that AddResearchModeCapability will also add the
        /// <Capabilities></Capabilities> container tag if it's missing.
        /// </summary>
        [Test]
        public void TestAddResearchModeCapability_CapabilitiesNodeMissing()
        {
            XElement rootElement = XElement.Parse(TestManifest);
            XElement capabilitiesElement = rootElement.Element(rootElement.GetDefaultNamespace() + "Capabilities");
            capabilitiesElement.Remove();

            // Not technically necessary, but sanity checks that we aren't
            // spuriously testing the same thing as
            // TestAddGazeInputCapability_CapabilitiesNodeExists
            Assert.IsNull(rootElement.Element(rootElement.GetDefaultNamespace() + "Capabilities"));

            UwpAppxBuildTools.AddResearchModeCapability(rootElement);

            AssertSingleResearchModeCapability(rootElement);
        }

        /// <summary>
        /// Validates that AddResearchModeCapability will only add the research
        /// mode capability if it doesn't already exist.
        /// </summary>
        [Test]
        public void TestAddResearchModeCapability_AddsOnce()
        {
            XElement rootElement = XElement.Parse(TestManifest);
            UwpAppxBuildTools.AddResearchModeCapability(rootElement);
            AssertSingleResearchModeCapability(rootElement);

            UwpAppxBuildTools.AddResearchModeCapability(rootElement);
            AssertSingleResearchModeCapability(rootElement);
        }

        private static void AssertSingleResearchModeCapability(XElement rootElement)
        {
            var researchModeCapabilities = rootElement
                .Element(rootElement.GetDefaultNamespace() + "Capabilities")
                .Descendants()
                .Where(element => element.Attribute("Name")?.Value == "perceptionSensorsExperimental")
                .ToList();

            Assert.AreEqual(1, researchModeCapabilities.Count,
                            "There must only be one perceptionSensorsExperimental capability");
        }

        /// <summary>
        /// Validates that AddCapability adds a capability.
        /// </summary>
        [Test]
        public void TestAddCapability_Adds()
        {
            XElement rootElement = XElement.Parse(TestManifest);
            UwpAppxBuildTools.AddCapability(rootElement, rootElement.GetDefaultNamespace() + "DeviceCapability", "gazeInput");
            AssertSingleGazeInputCapability(rootElement);
        }

        /// <summary>
        /// Validates that AddCapability will only add a capability if
        /// it doesn't already exist.
        /// </summary>
        [Test]
        public void TestAddCapability_AddsOnce()
        {
            XElement rootElement = XElement.Parse(TestManifest);
            UwpAppxBuildTools.AddCapability(rootElement, rootElement.GetDefaultNamespace() + "DeviceCapability", "gazeInput");
            AssertSingleGazeInputCapability(rootElement);

            UwpAppxBuildTools.AddCapability(rootElement, rootElement.GetDefaultNamespace() + "DeviceCapability", "gazeInput");
            AssertSingleGazeInputCapability(rootElement);
        }

        /// <summary>
        /// Validates that AddCapabilities adds a capability.
        /// </summary>
        [Test]
        public void TestAddCapabilities_Adds()
        {
            XElement rootElement = XElement.Parse(TestManifest);
            UwpAppxBuildTools.AddCapabilities(rootElement, new List<string>() { "gazeInput" });
            AssertSingleGazeInputCapability(rootElement);
        }

        /// <summary>
        /// Validates that AddCapabilities will only add a capability if
        /// it doesn't already exist.
        /// </summary>
        [Test]
        public void TestAddCapabilities_AddsOnce()
        {
            XElement rootElement = XElement.Parse(TestManifest);
            UwpAppxBuildTools.AddCapabilities(rootElement, new List<string>() { "gazeInput" });
            AssertSingleGazeInputCapability(rootElement);

            UwpAppxBuildTools.AddCapabilities(rootElement, new List<string>() { "gazeInput", "gazeInput" });
            AssertSingleGazeInputCapability(rootElement);
        }

        #endregion

        #region AssemblyCSharp Tests
        // A string version of a sample manifest that gets generated by the Unity build.
        // Used in lieu of an actual file manifest to avoid having to do I/O during tests.
        // Note that the xml declaration must be on the first line, otherwise parsing
        // will fail.
        private const string TestCSProject = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <Project ToolsVersion=""14.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
                <Import Project=""$(ProjectDir)..\..\..\UnityCommon.props"" />
                <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
                <PropertyGroup>
                    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
                    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
                    <ProjectGuid>{18d19403-3ebe-4604-b525-6be23a8a7d51}</ProjectGuid>
                    <OutputType>Library</OutputType>
                    <AppDesignerFolder>Properties</AppDesignerFolder>
                    <RootNamespace>AssemblyCSharpWSA</RootNamespace>
                    <AssemblyName>Assembly-CSharp</AssemblyName>
                    <DefaultLanguage>en-US</DefaultLanguage>
                    <TargetPlatformIdentifier>UAP</TargetPlatformIdentifier>
                    <TargetPlatformVersion>10.0.18362.0</TargetPlatformVersion>
                    <TargetPlatformMinVersion>10.0.10240.0</TargetPlatformMinVersion>
                    <MinimumVisualStudioVersion>14</MinimumVisualStudioVersion>
                    <FileAlignment>512</FileAlignment>
                    <ProjectTypeGuids>{A5A43C5B-DE2A-4C0C-9213-0A381AF9435A};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
                </PropertyGroup>
                <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|x86' "">
                    <DebugSymbols>true</DebugSymbols>
                    <DebugType>full</DebugType>
                    <Optimize>false</Optimize>
                    <OutputPath>bin\x86\Debug\</OutputPath>
                    <BaseIntermediateOutputPath>obj\x86\Debug\</BaseIntermediateOutputPath>
                    <DefineConstants>DEBUG;</DefineConstants>
                    <PlatformTarget>x86</PlatformTarget>
                    <UseVSHostingProcess>false</UseVSHostingProcess>
                    <ErrorReport>prompt</ErrorReport>
                    <Prefer32Bit>true</Prefer32Bit>
                </PropertyGroup>
            </Project>
        "; // end of TestCSProject

        /// <summary>
        /// Validates that AllowUnsafeCode will add an AllowUnsafeBlocks
        /// node to a PropertyGroup which has a Condition-attribute.
        /// </summary>
        [Test]
        public void TestAllowUnsafeCode_Adds()
        {
            XElement rootElement = XElement.Parse(TestCSProject);
            UwpAppxBuildTools.AllowUnsafeCode(rootElement);
            AssertSingleAllowUnsafeCode(rootElement);
        }

        /// <summary>
        /// Validates that AllowUnsafeCode will only add an AllowUnsafeBlocks
        /// node if it doesn't already exist.
        /// </summary>
        [Test]
        public void TestAllowUnsafeCode_AddsOnce()
        {
            XElement rootElement = XElement.Parse(TestCSProject);
            UwpAppxBuildTools.AllowUnsafeCode(rootElement);
            AssertSingleAllowUnsafeCode(rootElement);

            UwpAppxBuildTools.AllowUnsafeCode(rootElement);
            AssertSingleAllowUnsafeCode(rootElement);
        }

        private static void AssertSingleAllowUnsafeCode(XElement rootElement)
        {
            var allowUnsafeCode = rootElement
                .Elements(rootElement.GetDefaultNamespace() + "PropertyGroup")
                .Descendants()
                .Where(element => element.Name == element.Parent.GetDefaultNamespace() + "AllowUnsafeBlocks")
                .ToList();

            Assert.AreEqual(1, allowUnsafeCode.Count,
                            "There must only be one AllowUnsafeBlocks element");
        }
        #endregion
    }
}
