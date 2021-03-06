﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Management.Test.CloudService.Development.Scaffolding
{
    using System.IO;
    using System.Management.Automation;
    using System.Reflection;
    using Microsoft.WindowsAzure.Management.CloudService.Development.Scaffolding;
    using Microsoft.WindowsAzure.Management.Test.Utilities.Common;
    using Microsoft.WindowsAzure.Management.Utilities.CloudService;
    using Microsoft.WindowsAzure.Management.Utilities.Common;
    using Microsoft.WindowsAzure.Management.Utilities.Properties;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NewAzureRoleTemplateTests : TestBase
    {
        private MockCommandRuntime mockCommandRuntime;

        private NewAzureRoleTemplateCommand addTemplateCmdlet;

        [TestInitialize]
        public void SetupTest()
        {
            GlobalPathInfo.GlobalSettingsDirectory = Data.AzureSdkAppDir;
            CmdletSubscriptionExtensions.SessionManager = new InMemorySessionManager();
            mockCommandRuntime = new MockCommandRuntime();
        }

        [TestMethod]
        public void NewAzureRoleTemplateWithWebRole()
        {
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "WebRoleTemplate");
            addTemplateCmdlet = new NewAzureRoleTemplateCommand() { Web = true, CommandRuntime = mockCommandRuntime };

            addTemplateCmdlet.ExecuteCmdlet();

            Assert.AreEqual<string>(outputPath, ((PSObject)mockCommandRuntime.OutputPipeline[0]).GetVariableValue<string>(Parameters.Path));
            Testing.AssertDirectoryIdentical(Path.Combine(Resources.GeneralScaffolding, RoleType.WebRole.ToString()), outputPath);
        }

        [TestMethod]
        public void NewAzureRoleTemplateWithWorkerRole()
        {
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "WorkerRoleTemplate");
            addTemplateCmdlet = new NewAzureRoleTemplateCommand() { Worker = true, CommandRuntime = mockCommandRuntime };

            addTemplateCmdlet.ExecuteCmdlet();

            Assert.AreEqual<string>(outputPath, ((PSObject)mockCommandRuntime.OutputPipeline[0]).GetVariableValue<string>(Parameters.Path));
            Testing.AssertDirectoryIdentical(Path.Combine(Resources.GeneralScaffolding, RoleType.WorkerRole.ToString()), outputPath);
        }

        [TestMethod]
        public void NewAzureRoleTemplateWithOutputPath()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                string outputPath = files.RootPath;
                addTemplateCmdlet = new NewAzureRoleTemplateCommand() { Worker = true, CommandRuntime = mockCommandRuntime, Output = outputPath };

                addTemplateCmdlet.ExecuteCmdlet();

                Assert.AreEqual<string>(outputPath, ((PSObject)mockCommandRuntime.OutputPipeline[0]).GetVariableValue<string>(Parameters.Path));
                Testing.AssertDirectoryIdentical(Path.Combine(Resources.GeneralScaffolding, RoleType.WorkerRole.ToString()), outputPath);
            }
        }

        [TestMethod]
        public void NewAzureRoleTemplateWithDirectoryExists()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                string outputPath = files.CreateDirectory("test");
                addTemplateCmdlet = new NewAzureRoleTemplateCommand() { Worker = true, CommandRuntime = mockCommandRuntime, Output = outputPath };

                addTemplateCmdlet.ExecuteCmdlet();

                Assert.AreEqual<string>(
                    outputPath,
                    ((PSObject)mockCommandRuntime.OutputPipeline[0]).GetVariableValue<string>(Parameters.Path));
                Testing.AssertDirectoryIdentical(
                    Path.Combine(Resources.GeneralScaffolding,
                    RoleType.WorkerRole.ToString()),
                    outputPath);
            }
        }

        [TestMethod]
        public void NewAzureRoleTemplateWithRunningOutsideDefaultDirectory()
        {
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "TestDir", "WebRoleTemplate");
            addTemplateCmdlet = new NewAzureRoleTemplateCommand() { Web = true, CommandRuntime = mockCommandRuntime };
            string originalDir = Directory.GetCurrentDirectory();
            Directory.CreateDirectory("TestDir");
            Directory.SetCurrentDirectory("TestDir");

            try
            {
                addTemplateCmdlet.ExecuteCmdlet();

                Assert.AreEqual<string>(
                    outputPath,
                    ((PSObject)mockCommandRuntime.OutputPipeline[0]).GetVariableValue<string>(Parameters.Path));
                Testing.AssertDirectoryIdentical(
                    Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                    Resources.GeneralScaffolding, RoleType.WebRole.ToString())),
                    outputPath);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }
        }
    }
}
