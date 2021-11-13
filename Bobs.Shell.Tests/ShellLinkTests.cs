using Xunit;
using Bobs.Shell;
using System.IO;

namespace Bobs.Shell.Tests
{
    public class ShellLinkTests: IClassFixture<FileFixture>
    {
        private readonly FileFixture fixture;

        public ShellLinkTests(FileFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Save_ShouldCreateLinkFile()
        {
            // Arrange
            ShellLink shellLink = new();
            string linkPath = this.fixture.GetLinkPath();
            Assert.False(File.Exists(linkPath));

            // Act
            shellLink.Save(linkPath);

            // Assert
            Assert.True(File.Exists(linkPath));
        }

        [Fact]
        public void Load_ReadsSavedLink()
        {
            // Arrange
            ShellLink shellLink1 = new()
            {
                Target = fixture.CreateTargetFile(),
                Arguments = "/1 /2",
                WorkingDirectory = fixture.CreateWorkingDirectory(),
                Description = "This is a test!",
                Hotkey = 50,
                HotkeyModifier = HotkeyFlags.Control | HotkeyFlags.Alt,
                IconLocation = fixture.CreateIconFile(),
                IconIndex = -99,
                ShowCommand = ShowCommand.Maximized,
            };
            string linkPath = this.fixture.GetLinkPath();
            shellLink1.Save(linkPath);

            // Act
            ShellLink shellLink2 = new();
            shellLink2.Load(linkPath);

            // Assert
            Assert.Equal(shellLink1.Target, shellLink2.Target);
            Assert.Equal(shellLink1.Arguments, shellLink2.Arguments);
            Assert.Equal(shellLink1.WorkingDirectory, shellLink2.WorkingDirectory);
            Assert.Equal(shellLink1.Description, shellLink2.Description);
            Assert.Equal(shellLink1.Hotkey, shellLink2.Hotkey);
            Assert.Equal(shellLink1.HotkeyModifier, shellLink2.HotkeyModifier);
            Assert.Equal(shellLink1.IconLocation, shellLink2.IconLocation);
            Assert.Equal(shellLink1.IconIndex, shellLink2.IconIndex);
            Assert.Equal(shellLink1.ShowCommand, shellLink2.ShowCommand);
        }
    }
}