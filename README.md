# TimedWebScrap

This project is an Azure Function application that periodically fetches data from the Public APIs endpoint 
and stores the success/failure attempt logs in an Azure Table and the full payload in an Azure Blob. It also provides GET API calls to list logs for a specific time period and fetch a payload from the blob for a specific log entry.

## Requirements

To run the Azure Function Data Fetcher, you need the following:

- Azure Function (Cloud or Local)
- Azure Storage (Cloud or Local storage emulator)
    - Azure Table storage
    - Azure Blob storage
- .NET Core SDK installed on your machine

## Installation and Setup Instructions

- Clone the project repository to your local machine.
- Open the solution in Visual Studio or your preferred IDE.
- Configure the Azure Storage connection strings in the local.settings.json file or your Azure Function application settings for both Table storage and Blob storage.
- Build the solution to restore the NuGet packages and compile the project.

## Functionality

The Azure Function Data Fetcher performs the following tasks:

- Every minute, it fetches data from the Public APIs endpoint.
- It logs the success/failure attempt in the Azure Table.
- It stores the full payload in the Azure Blob.

<!--
API Endpoints

The Azure Function Data Fetcher exposes the following API endpoints:
List Logs for Specific Time Period

Endpoint: GET /api/logs

This endpoint retrieves a list of logs for a specific time period.

Parameters:

    from (required): The starting date and time of the time period in ISO 8601 format.
    to (required): The ending date and time of the time period in ISO 8601 format.

Fetch Payload for Specific Log Entry

Endpoint: GET /api/logs/{logId}/payload

This endpoint retrieves the payload from the blob storage for a specific log entry.

Parameters:

    logId (required): The ID of the log entry.

Usage

To run the Azure Function Data Fetcher, follow these steps:

    Deploy the Azure Function to your Azure account or run it locally using the Azure Functions Core Tools.
    Ensure that your Azure Storage account is properly configured and accessible.
    Make HTTP requests to the exposed API endpoints to interact with the Azure Function.
-->
Additional Notes

You can customize the schedule and data fetching logic in the Azure Function code according to your requirements.
Feel free to contribute and provide feedback.
