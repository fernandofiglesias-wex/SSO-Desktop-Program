# SSO Desktop Program

A Windows Forms application for managing Microsoft Enterprise Single Sign-On (SSO) configuration store applications. This tool provides a user-friendly graphical interface for creating, configuring, and managing SSO applications and their properties.

## 🚀 Features

- **Create SSO Applications**: Set up new configuration store applications with custom properties
- **Manage Properties**: Add, modify, and delete application properties with masking support
- **View Applications**: List all existing SSO applications in your environment
- **Delete Applications**: Remove SSO applications when no longer needed
- **Property Validation**: Real-time validation of application names and property formats
- **User-Friendly Interface**: Intuitive Windows Forms GUI replacing complex command-line operations

## 📋 Prerequisites

### For Running the Application

**Option 1: Framework-Dependent Version**
- Windows 10/11 or Windows Server 2016+
- .NET 5.0 Runtime or later
- Microsoft Enterprise Single Sign-On Service installed and configured

**Option 2: Self-Contained Version (Recommended)**
- Windows 10/11 or Windows Server 2016+
- Microsoft Enterprise Single Sign-On Service installed and configured
- No additional .NET runtime required

### Permissions Required
- Member of **SSO Administrators** group
- Member of **SSO Affiliate Administrators** group (for application creation)
- Appropriate permissions to access the SSO Configuration Database

## 📥 Installation

### Download and Run the Self-Contained Version (Easiest)
1. Download the `standalone.zip` from the `publish` folder
2. Extract the files in your desired location
3. Run as an administrator (recommended for SSO operations)

### Build from Source
```bash
# Clone the repository
git clone https://github.com/fernandofiglesias-wex/SSO-Desktop-Program.git
cd SSO-Desktop-Program/SSOConfigManagerUI

# Build the application
dotnet build -c Release

# Or create a standalone executable
dotnet publish -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true
```

## 🎯 Usage

### Starting the Application
1. Launch `SSOConfigManagerUI.exe`
2. The main window displays five primary functions:

### Main Operations

#### 1. **Show All Applications**
- Displays a list of all existing SSO configuration store applications
- Useful for getting an overview of your SSO environment

#### 2. **Create SSO Application**
- **Application Name**: Enter a unique name for your SSO application
- **Description**: Provide a meaningful description
- **Initial Property**: Optionally add a property during creation
  - Property Key: The name of the configuration property
  - Property Value: The value to store
  - Masked: Check if the property should be encrypted/masked

#### 3. **Set Properties**
- **Application Name**: Enter an existing application name (validates in real-time)
- **Property Management**:
  - Add new properties with key-value pairs
  - Modify existing property values
  - Delete unwanted properties
  - Toggle masking for sensitive data
- **Bulk Operations**: Modify multiple properties in a single operation

#### 4. **Get Properties**
- Enter an application name to retrieve all its properties
- Displays property keys, values, and masking status
- Useful for configuration verification and troubleshooting

#### 5. **Delete Application**
- Permanently removes an SSO application and all its properties
- Includes confirmation dialog to prevent accidental deletion
- **Warning**: This operation cannot be undone

### Property Management Best Practices

- **Use meaningful property names**: Follow a consistent naming convention
- **Mask sensitive data**: Enable masking for passwords, tokens, and confidential information
- **Document your properties**: Use descriptive names and maintain external documentation
- **Test configurations**: Always verify property values after modification

## 🏗️ Architecture

### Object-Oriented Design
The application follows modern OOP principles:

- **Domain Models**: `SSOApplication`, `SSOProperty`, `ApplicationSettings`
- **Service Layer**: `ISSOService`, `IApplicationService` with concrete implementations
- **Data Transfer Objects**: Clean contracts for requests and responses
- **Base Form Classes**: Reusable UI components with inheritance
- **Validation Framework**: Strategy pattern for pluggable validation rules

### Key Components

```
SSOConfigManagerUI/
├── Models/               # Domain models and data structures
├── Services/             # Business logic and SSO operations
├── DTOs/                 # Data transfer objects
├── Validation/           # Validation components and strategies
├── Base/                 # Base form classes and common UI
└── Forms/                # Windows Forms UI components
```

## 🔧 Configuration

### Default Settings
- **Contact Info**: `BizTalk@WexInc.com`
- **User Account**: `SSO Affiliate Administrators`
- **Admin Account**: `SSO Administrators`

### Customization
Settings can be modified in the `ApplicationSettings` class or through configuration files.

## ⚠️ Troubleshooting

### Common Issues

**"Application doesn't exist" Error**
- Verify the application name spelling
- Ensure you have read permissions to the SSO database
- Check that the SSO service is running

**Permission Denied**
- Run the application as Administrator
- Verify SSO group membership
- Check SSO service configuration

**Property Deletion Not Working**
- The application uses a "nuclear option" approach, recreating the entire application
- Ensure you have delete permissions
- Large applications may take longer to process

**Performance Issues**
- Validation includes debouncing to prevent excessive SSO API calls
- For large environments, consider using the Get Properties function first

### Logging
The application provides user-friendly error messages. For detailed troubleshooting:
1. Check Windows Event Logs under "Enterprise Single Sign-On"
2. Verify SSO service status in Services console
3. Review SSO database connectivity

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Setup
```bash
# Clone the repository
git clone https://github.com/fernandofiglesias-wex/SSO-Desktop-Program.git

# Open in Visual Studio or VS Code
cd SSO-Desktop-Program/SSOConfigManagerUI

# Restore packages and build
dotnet restore
dotnet build
```

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

For issues, questions, or feature requests:
- Create an issue on GitHub
- Contact: BizTalk@WexInc.com
- Documentation: Check the SSO Administration Guide

## 📝 Changelog

### v2.0.0 (Current)
- ✨ Complete UI conversion from console application
- ✨ Object-oriented architecture with modern design patterns
- ✨ Real-time application name validation
- ✨ Enhanced property management with bulk operations
- ✨ Improved error handling and user feedback
- ✨ Performance optimizations with validation debouncing
- 🔧 Self-contained executable distribution
- 🔧 Comprehensive validation framework

### v1.0.0 (Legacy)
- 📱 Console-based SSO configuration management
- ⚙️ Basic CRUD operations for SSO applications
- 🔧 Command-line interface

## 🔗 Related Documentation

- [Microsoft Enterprise Single Sign-On Documentation](https://docs.microsoft.com/en-us/biztalk/core/enterprise-single-sign-on-sso)
- [SSO Administration Guide](https://docs.microsoft.com/en-us/biztalk/core/how-to-administer-sso)
- [BizTalk Server SSO Configuration](https://docs.microsoft.com/en-us/biztalk/core/configuring-sso)

---

**Note**: This application is designed for enterprise environments with Microsoft Enterprise Single Sign-On infrastructure. Ensure proper SSO service configuration before use.