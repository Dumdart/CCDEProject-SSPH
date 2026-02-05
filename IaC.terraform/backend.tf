terraform {
  backend "azurerm" {
    # Insert credentials here
    resource_group_name  = ""
    storage_account_name = ""
    container_name       = ""
    key                  = ""
  }
}