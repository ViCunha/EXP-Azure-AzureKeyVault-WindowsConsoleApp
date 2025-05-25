### Overview
---
Proof of Concept (PoC) of .NET console application consuming Azure Key Vault secret
   
### Key Aspects
---
Azure Key Vaults can be used to manage secrets and certificates to be used in .NET workloads

### Environments
---
keys 
- d20250524172301
- d20250524172302

Azure
- Azure Cloud Shell
  - PowerShell # cmdlet
  - Bash # Azure CLI
- Azure Resource Group
- Azure Key Vault

### Diagrams
---
![image](https://github.com/user-attachments/assets/c4dd021a-c4ab-48f6-80f3-5c306ca7a9ec)


### Actions
---
Create required resources
  - Open Azure Clould Shell at https://shell.azure.com/

  - Get required data
```
// PowerShell # CMDLET
Get-AzLocation | Select-Object DisplayName, Location

---
// Bash # Azure CLI
az account list-locations -o table

Create environment variables

// PowerShell # CMDLET
$SCRIPT_KEY = "d20250524172301"
$LOCATION = "westeurope"
$OBJECT_ID = "..."
$SECRET_NAME = 'MySecretName'
$SECRECT_VALUE = ConvertTo-SecureString "MySecrectValue-01" -AsPlainText -Force
write-output $SCRIPT_KEY

---

// Bash # Azure CLI
SCRIPT_KEY="d20250524172302"
LOCATION="westeurope"
OBJECT_ID="..."
SECRET_NAME='MySecretName'
SECRECT_VALUE='MySecretValue-02'
echo $SCRIPT_KEY
```

- Create Azure Resource Group
```
// PowerShell # CMDLET
New-AzResourceGroup -Location $LOCATION -Name ("{0}-rg" -f $SCRIPT_KEY)

---
// Bash # Azure CLI
az group create --location $LOCATION --name "${SCRIPT_KEY}-rg"
```

- Create Azure Key Vault
```
// PowerShell # CMDLET
New-AzKeyVault -Name ("{0}-keyvault" -f $SCRIPT_KEY) -ResourceGroupName ("{0}-rg" -f $SCRIPT_KEY) -Location $LOCATION

---
// Bash # Azure CLI
az keyvault create --location ${LOCATION} --name "${SCRIPT_KEY}-keyvault" --resource-group "${SCRIPT_KEY}-rg"
```

- Set Key Vault Administrator role
```
// PowerShell # CMDLET
New-AzRoleAssignment -ResourceGroupName ("{0}-rg" -f $SCRIPT_KEY) -ObjectId $OBJECT_ID -RoleDefinitionName "Key Vault Administrator" -AllowDelegation

---
// Bash # Azure CLI
az role assignment create --role "Key Vault Administrator" --assignee $OBJECT_ID --scope "/"
```

- Set a new secret in Key Vault
```
// PowerShell # CMDLET
Set-AzKeyVaultSecret -Name $SECRET_NAME -SecretValue $SECRECT_VALUE -VaultName ("{0}-keyvault" -f $SCRIPT_KEY)

---
// Bash # Azure CLI
az keyvault secret set --name ${SECRET_NAME} --value ${SECRECT_VALUE} --vault-name "${SCRIPT_KEY}-keyvault"
```

- Copy Key Vault id
```
// PowerShell # CMDLET
Get-AzKeyVaultSecret -VaultName ("{0}-keyvault" -f $SCRIPT_KEY) -Name $SECRET_NAME

Vault Name   : d20250524172301-keyvault
Name         : MySecretName
Version      : de494eec8e09482e90e3fb4ffc542052
Id           : https://d20250524172301-keyvault.vault.azure.net:443/secrets/MySecretName/...
Enabled      : True
Expires      : 
Not Before   : 
Created      : 5/24/2025 10:03:28 PM
Updated      : 5/24/2025 10:03:28 PM
Content Type : 
Tags         : 

---
// Bash # Azure CLI
az keyvault secret show --vault-name "${SCRIPT_KEY}-keyvault" --name ${SECRET_NAME}

{
  "attributes": {
    "created": "2025-05-24T22:07:50+00:00",
    "enabled": true,
    "expires": null,
    "notBefore": null,
    "recoverableDays": 90,
    "recoveryLevel": "Recoverable+Purgeable",
    "updated": "2025-05-24T22:07:50+00:00"
  },
  "contentType": null,
  "id": "https://d20250524172302-keyvault.vault.azure.net/secrets/MySecretName/...",
  "kid": null,
  "managed": null,
  "name": "MySecretName",
  "tags": {
    "file-encoding": "utf-8"
  },
  "value": "MySecretValue"
}
```

Create the console app

- Create the solution folder
```
md EXP-Azure-AzureKeyVault-WindowsConsoleApp
cd EXP-Azure-AzureKeyVault-WindowsConsoleApp
```

- Create the a Console App solution and project
```
dotnet new --help
dotnet new list
dotnet new solution --name 'EXP-Azure-AzureKeyVault-WindowsConsoleApp'
dotnet new create console --name 'EXP-Azure-AzureKeyVault-WindowsConsoleApp'
dotnet solution add 'EXP-Azure-AzureKeyVault-WindowsConsoleApp/EXP-Azure-AzureKeyVault-WindowsConsoleApp.csproj'
```

- Add libraries in the project
```
cd .\EXP-Azure-AzureKeyVault-WindowsConsoleApp\
dotnet add package Azure.Security.KeyVault.Secrets
dotnet add package Azure.Identity
```

- Set environment variables
```
$Env:ENV_KEY_VAULT_NAME='d20250524172302-keyvault'
$Env:ENV_KEY_VAULT_SECRET_NAME='MySecretValue'
```

- Install Az PowerShell module in Visual Studio Developer PowerShell
```
Install-Module -Name Az -AllowClobber -Scope CurrentUser
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
Import-Module Az.Accounts
```

- Sign in to Azure
```
Connect-AzAccount
```

- Open Microsoft Visual Studio with the solution
```
devenv  'EXP-Azure-AzureKeyVault-WindowsConsoleApp.sln'
```

- Implement the code
```
using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

const string CONST_ENV_KEY_VAULT_NAME_NOT_FOUND = "The specified key was not found in the Key Vault.";

const string CONST_ENV_KEY_VAULT_SECRET_NAME_NOT_FOUND = "The specified secret name was not found in the Key Vault.";

string KeyVaultName = Environment.GetEnvironmentVariable("ENV_KEY_VAULT_NAME") ?? CONST_ENV_KEY_VAULT_NAME_NOT_FOUND;

string KeyVaultSecretName = Environment.GetEnvironmentVariable("ENV_KEY_VAULT_SECRET_NAME") ?? CONST_ENV_KEY_VAULT_SECRET_NAME_NOT_FOUND;

if (KeyVaultName == CONST_ENV_KEY_VAULT_NAME_NOT_FOUND)
{
    Console.WriteLine(CONST_ENV_KEY_VAULT_NAME_NOT_FOUND);

    Environment.Exit(1);
}

if (KeyVaultSecretName == CONST_ENV_KEY_VAULT_SECRET_NAME_NOT_FOUND)
{
    Console.WriteLine(CONST_ENV_KEY_VAULT_SECRET_NAME_NOT_FOUND);

    Environment.Exit(1);
}

string KeyVaultUri = $"https://{KeyVaultName}.vault.azure.net/";

Console.WriteLine(KeyVaultUri);

var client = new SecretClient(new Uri(KeyVaultUri), new DefaultAzureCredential());

var secret = await client.GetSecretAsync(KeyVaultSecretName);

Console.WriteLine(secret.Value.Value);
```

- Build and run
```
dotnet build
dotnet run
```

Publish the workload

- Create the repository

- Publish the code to GitHub


### Media
---
![image](https://github.com/user-attachments/assets/7ee349aa-bcc1-4bf5-9c90-92cb42debc7c)

### References
---
- https://learn.microsoft.com/en-us/azure/key-vault/secrets/quick-create-net?tabs=azure-powershell
