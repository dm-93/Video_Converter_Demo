# Cambium Video Encoding with Bitmovin

This repository contains an Azure Function that handles video processing using the Bitmovin API. The main functionality includes triggering video encoding workflows in response to blob uploads to an Azure Blob Storage container.

## Table of Contents

- [Overview](#overview)
- [Setup and Configuration](#setup-and-configuration)
- [Project Structure](#project-structure)

## Overview

This project utilizes the Bitmovin API to encode videos stored in Azure Blob Storage. It consists of an Azure Function that is triggered by blob uploads. The encoding process involves converting videos to MP4 format with specific audio and video configurations.

## Setup and Configuration

### Prerequisites

- .NET SDK
- Azure Subscription
- Bitmovin API Key
- Azure Storage Account

### Configuration

1. **Bitmovin API Key**: Set your Bitmovin API key in the `VideoHandler` constructor.
2. **Azure Storage Connection String**: Configure your Azure Storage connection string in the `local.settings.json` file or in the Azure Function App settings.

### Local Settings

Create a `local.settings.json` file in the root of the project with the following structure:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "Your_Azure_Storage_Connection_String",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "connection-string": "Your_Azure_Storage_Connection_String"
  }
}
```
### Project Structure
```json
.
├── VideoHandler.cs
├── Services
│   ├── AudioConfigurationService.cs
│   ├── MP4EncodingBuilder.cs
│   ├── VideoConfigurationService.cs
│   └── Interfaces
│       ├── IConfigurationService.cs
│       └── IEncodingBuider.cs
```
- [VideoHandler.cs]: Contains the Azure Function HandleVideo that is triggered by blob uploads. It manages the video encoding process using the Bitmovin API.
- [Services/AudioConfigurationService.cs]: Handles the creation and retrieval of audio codec configurations.
- [Services/MP4EncodingBuilder.cs]: Manages the creation and configuration of encoding jobs, streams, and muxing.
- [Services/VideoConfigurationService.cs]: Handles the creation and retrieval of video codec configurations.
- [Services/Interfaces/IConfigurationService.cs]: Interface for configuration services.
- [Services/Interfaces/IEncodingBuider.c]s: Interface for encoding builders.
