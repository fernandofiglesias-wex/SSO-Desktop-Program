# Release Deployment Guide

## 📦 Available Release Files

This directory contains ready-to-distribute executables for the SSO Desktop Program:

### **SSOConfigManagerUI-v2.0.0-standalone.exe** (Recommended)
- **Size**: ~128 MB
- **Dependencies**: None - runs on any Windows machine
- **Use Case**: General distribution, unknown target environments
- **Requirements**: Windows 10/11 or Windows Server 2016+

### **SSOConfigManagerUI-v2.0.0-framework-dependent.exe**
- **Size**: ~126 KB
- **Dependencies**: Requires .NET 5.0 Runtime
- **Use Case**: Controlled environments with .NET already installed
- **Requirements**: Windows 10/11 + .NET 5.0 Runtime

## 🚀 GitHub Release Instructions

### Option 1: GitHub Web Interface (Recommended)
1. Go to your repository: `https://github.com/fernandofiglesias-wex/SSO-Desktop-Program`
2. Click **"Releases"** on the right sidebar
3. Click **"Create a new release"**
4. Fill in the release information:
   - **Tag version**: `v2.0.0`
   - **Release title**: `SSO Desktop Program v2.0.0`
   - **Description**: Use the changelog from README.md
5. **Attach binary files**:
   - Drag and drop both `.exe` files from this folder
   - Or click "Attach binaries" and select them
6. Check **"Set as the latest release"**
7. Click **"Publish release"**

### Option 2: GitHub CLI (Advanced)
```bash
# Install GitHub CLI first: https://cli.github.com/
gh release create v2.0.0 \
  ./releases/SSOConfigManagerUI-v2.0.0-standalone.exe \
  ./releases/SSOConfigManagerUI-v2.0.0-framework-dependent.exe \
  --title "SSO Desktop Program v2.0.0" \
  --notes "Complete UI conversion with modern OOP architecture. See README.md for full changelog."
```

## 📝 Release Notes Template

```markdown
# SSO Desktop Program v2.0.0

## 🎉 Major Update: Complete UI Conversion

This release represents a complete transformation from console application to modern Windows Forms GUI with professional OOP architecture.

### ✨ New Features
- **Graphical User Interface**: Full Windows Forms application replacing command-line interface
- **Real-time Validation**: Application name validation with instant feedback
- **Enhanced Property Management**: Bulk operations, masking support, and intuitive editing
- **Self-contained Distribution**: No .NET runtime dependencies required

### 🏗️ Architecture Improvements
- **Object-Oriented Design**: Domain models, service layer, and validation framework
- **Modern Design Patterns**: Strategy, Template Method, and Factory patterns
- **Separation of Concerns**: Clear layering and dependency injection support
- **Testable Code**: Interface-based design for better maintainability

### 🔧 Technical Enhancements
- **Performance Optimization**: Validation debouncing to prevent UI slowdown
- **Error Handling**: Comprehensive error messages and user feedback
- **Deployment Options**: Framework-dependent and self-contained executables
- **Professional Documentation**: Complete README with troubleshooting guide

### 📦 Download Options
- **Standalone Version** (Recommended): No dependencies, runs anywhere
- **Framework-Dependent**: Smaller size, requires .NET 5.0 runtime

### ⚠️ Breaking Changes
- Console interface removed (functionality preserved in GUI)
- Requires Windows 10/11 or Windows Server 2016+

### 🔗 Requirements
- Microsoft Enterprise Single Sign-On Service
- SSO Administrators group membership
- Windows 10/11 or Windows Server 2016+
```

## 🎯 Distribution Best Practices

### For Enterprise Deployment
1. **Use the standalone version** for maximum compatibility
2. **Test in your environment** before wide deployment
3. **Document SSO permissions** required for your users
4. **Consider group policy deployment** for large organizations

### For Individual Users
1. **Download the standalone version**
2. **Run as Administrator** for SSO operations
3. **Verify SSO group membership** before use
4. **Keep executable in a permanent location**

## 📋 Post-Release Checklist

- [ ] Verify GitHub release is published
- [ ] Test download links work
- [ ] Update any deployment documentation
- [ ] Notify relevant teams/users
- [ ] Monitor for user feedback and issues

## 🆘 Support Information

For issues with releases:
- **GitHub Issues**: Create an issue with the `bug` or `question` label
- **Enterprise Support**: Contact BizTalk@WexInc.com
- **Documentation**: See README.md for troubleshooting guide