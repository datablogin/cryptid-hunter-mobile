# Unity CI/CD Setup Guide

## Prerequisites
1. Unity account with valid license
2. GitHub repository secrets configured

## Setup Steps

### 1. Obtain Unity License File
1. Go to the Actions tab in your GitHub repository
2. Run the "Acquire Unity activation file" workflow
3. Download the generated `.alf` file from artifacts
4. Go to https://license.unity3d.com/manual
5. Upload the `.alf` file and complete activation
6. Download the `.ulf` license file

### 2. Configure GitHub Secrets
Add these secrets to your repository (Settings → Secrets → Actions):
- `UNITY_LICENSE`: Contents of your `.ulf` file (open in text editor and copy all)
- `UNITY_EMAIL`: Your Unity account email
- `UNITY_PASSWORD`: Your Unity account password

### 3. Running Builds
- Builds trigger automatically on push to `main` or `develop` branches
- Pull requests to `main` also trigger builds
- Check the Actions tab to monitor build progress

### 4. Build Artifacts
- Android APK files are uploaded as artifacts
- iOS builds generate Xcode projects (requires Mac for final compilation)
- Artifacts are retained for 7 days

## Troubleshooting
- If builds fail with license errors, regenerate your Unity license
- For iOS builds, you'll need to download the Xcode project and build on macOS
- Check Unity version compatibility in ProjectVersion.txt