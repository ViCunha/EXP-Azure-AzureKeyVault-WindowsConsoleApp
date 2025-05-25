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