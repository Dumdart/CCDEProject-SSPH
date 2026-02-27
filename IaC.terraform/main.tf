terraform {
  required_providers {
    azurerm = {
      source  = "azurerm"
      version = "4.58.0"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "res-0" {
  location   = "germanywestcentral"
  name       = "CCDE-Project"
  tags       = {}
}

resource "azurerm_cosmosdb_account" "res-1" {
  access_key_metadata_writes_enabled           = true
  analytical_storage_enabled                   = false
  automatic_failover_enabled                   = true
  burst_capacity_enabled                       = false
  create_mode                                  = "Default"
  default_identity_type                        = "FirstPartyIdentity"
  free_tier_enabled                            = false
  ip_range_filter                              = ["0.0.0.0", "104.28.207.43", "13.88.56.148", "13.91.105.215", "4.210.172.107", "40.91.218.243", "62.68.194.252", "93.82.221.122"]
  is_virtual_network_filter_enabled            = false
  kind                                         = "GlobalDocumentDB"
  local_authentication_disabled                = false
  location                                     = "germanywestcentral"
  minimal_tls_version                          = "Tls12"
  multiple_write_locations_enabled             = false
  name                                         = "ccdeportfolio-db"
  network_acl_bypass_for_azure_services        = false
  network_acl_bypass_ids                       = []
  offer_type                                   = "Standard"
  partition_merge_enabled                      = false
  primary_key                                  = "" # Masked sensitive attribute
  primary_mongodb_connection_string            = "" # Masked sensitive attribute
  primary_readonly_key                         = "" # Masked sensitive attribute
  primary_readonly_mongodb_connection_string   = "" # Masked sensitive attribute
  primary_readonly_sql_connection_string       = "" # Masked sensitive attribute
  primary_sql_connection_string                = "" # Masked sensitive attribute
  public_network_access_enabled                = true
  resource_group_name                          = azurerm_resource_group.res-0.name
  secondary_key                                = "" # Masked sensitive attribute
  secondary_mongodb_connection_string          = "" # Masked sensitive attribute
  secondary_readonly_key                       = "" # Masked sensitive attribute
  secondary_readonly_mongodb_connection_string = "" # Masked sensitive attribute
  secondary_readonly_sql_connection_string     = "" # Masked sensitive attribute
  secondary_sql_connection_string              = "" # Masked sensitive attribute
  tags = {
    defaultExperience       = "Core (SQL)"
    hidden-cosmos-mmspecial = ""
    hidden-workload-type    = "Production"
  }
  analytical_storage {
    schema_type = "WellDefined"
  }
  backup {
    interval_in_minutes = 0
    retention_in_hours  = 0
    storage_redundancy  = ""
    tier                = "Continuous7Days"
    type                = "Continuous"
  }
  capabilities {
    name = "EnableServerless"
  }
  consistency_policy {
    consistency_level       = "Session"
    max_interval_in_seconds = 5
    max_staleness_prefix    = 100
  }
  geo_location {
    failover_priority = 0
    location          = "germanywestcentral"
    zone_redundant    = false
  }
}

resource "azurerm_cosmosdb_sql_database" "res-2" {
  account_name        = "ccdeportfolio-db"
  name                = "PortfolioDB"
  resource_group_name = azurerm_resource_group.res-0.name
  depends_on = [
    azurerm_cosmosdb_account.res-1,
  ]
}

resource "azurerm_cosmosdb_sql_container" "res-3" {
  account_name          = "ccdeportfolio-db"
  database_name         = "PortfolioDB"
  name                  = "PageViews"
  partition_key_kind    = "Hash"
  partition_key_paths   = ["/pageId"]
  partition_key_version = 2
  resource_group_name   = azurerm_resource_group.res-0.name
  conflict_resolution_policy {
    conflict_resolution_path      = "/_ts"
    conflict_resolution_procedure = ""
    mode                          = "LastWriterWins"
  }
  indexing_policy {
    indexing_mode = "consistent"
    included_path {
      path = "/*"
    }
  }
  depends_on = [
    azurerm_cosmosdb_sql_database.res-2,
  ]
}
resource "azurerm_cosmosdb_sql_role_definition" "res-10" {
  account_name        = "ccdeportfolio-db"
  assignable_scopes   = [azurerm_cosmosdb_account.res-1.id]
  name                = "Cosmos DB Built-in Data Reader"
  resource_group_name = azurerm_resource_group.res-0.name
  role_definition_id  = "00000000-0000-0000-0000-000000000001"
  type                = "BuiltInRole"
  permissions {
    data_actions = ["Microsoft.DocumentDB/databaseAccounts/readMetadata", "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/executeQuery", "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/read", "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/readChangeFeed"]
  }
}
resource "azurerm_cosmosdb_sql_role_definition" "res-11" {
  account_name        = "ccdeportfolio-db"
  assignable_scopes   = [azurerm_cosmosdb_account.res-1.id]
  name                = "Cosmos DB Built-in Data Contributor"
  resource_group_name = azurerm_resource_group.res-0.name
  role_definition_id  = "00000000-0000-0000-0000-000000000002"
  type                = "BuiltInRole"
  permissions {
    data_actions = ["Microsoft.DocumentDB/databaseAccounts/readMetadata", "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/*", "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*"]
  }
}
resource "azurerm_storage_account" "res-14" {
  access_tier                       = ""
  account_kind                      = "Storage"
  account_replication_type          = "LRS"
  account_tier                      = "Standard"
  allow_nested_items_to_be_public   = false
  allowed_copy_scope                = ""
  cross_tenant_replication_enabled  = false
  default_to_oauth_authentication   = true
  dns_endpoint_type                 = "Standard"
  edge_zone                         = ""
  https_traffic_only_enabled        = true
  infrastructure_encryption_enabled = false
  is_hns_enabled                    = false
  large_file_share_enabled          = false
  local_user_enabled                = true
  location                          = "germanywestcentral"
  min_tls_version                   = "TLS1_2"
  name                              = "ccdeprojecta2ad"
  nfsv3_enabled                     = false
  primary_access_key                = "" # Masked sensitive attribute
  primary_blob_connection_string    = "" # Masked sensitive attribute
  primary_connection_string         = "" # Masked sensitive attribute
  provisioned_billing_model_version = ""
  public_network_access_enabled     = true
  queue_encryption_key_type         = "Service"
  resource_group_name               = azurerm_resource_group.res-0.name
  secondary_access_key              = "" # Masked sensitive attribute
  secondary_blob_connection_string  = "" # Masked sensitive attribute
  secondary_connection_string       = "" # Masked sensitive attribute
  sftp_enabled                      = false
  shared_access_key_enabled         = true
  table_encryption_key_type         = "Service"
  tags                              = {}
  blob_properties {
    change_feed_enabled           = false
    change_feed_retention_in_days = 0
    default_service_version       = ""
    last_access_time_enabled      = false
    versioning_enabled            = false
  }
  share_properties {
    retention_policy {
      days = 7
    }
  }
}
resource "azurerm_storage_container" "res-16" {
  container_access_type             = "private"
  default_encryption_scope          = "$account-encryption-key"
  encryption_scope_override_enabled = true
  metadata                          = {}
  name                              = "app-package-ccdeproject-api-50fac68"
  storage_account_id                = azurerm_storage_account.res-14.id
  storage_account_name              = ""
}
resource "azurerm_storage_container" "res-17" {
  container_access_type             = "private"
  default_encryption_scope          = "$account-encryption-key"
  encryption_scope_override_enabled = true
  metadata                          = {}
  name                              = "azure-webjobs-hosts"
  storage_account_id                = azurerm_storage_account.res-14.id
  storage_account_name              = ""
}
resource "azurerm_storage_container" "res-18" {
  container_access_type             = "private"
  default_encryption_scope          = "$account-encryption-key"
  encryption_scope_override_enabled = true
  metadata                          = {}
  name                              = "azure-webjobs-secrets"
  storage_account_id                = azurerm_storage_account.res-14.id
  storage_account_name              = ""
}
resource "azurerm_service_plan" "res-23" {
  app_service_environment_id      = ""
  location                        = "germanywestcentral"
  maximum_elastic_worker_count    = 1
  name                            = "ASP-CCDEProject-8cfa"
  os_type                         = "Linux"
  per_site_scaling_enabled        = false
  premium_plan_auto_scale_enabled = false
  resource_group_name             = azurerm_resource_group.res-0.name
  sku_name                        = "FC1"
  tags                            = {}
  worker_count                    = 0
  zone_balancing_enabled          = false
}
resource "azurerm_function_app_flex_consumption" "res-24" {
  app_settings                       = {}
  client_certificate_enabled         = false
  client_certificate_exclusion_paths = ""
  client_certificate_mode            = "Required"
  custom_domain_verification_id      = "" # Masked sensitive attribute
  enabled                            = true
  http_concurrency                   = 0
  https_only                         = true
  instance_memory_in_mb              = 512
  location                           = "germanywestcentral"
  maximum_instance_count             = 100
  name                               = "ccdeproject-api"
  public_network_access_enabled      = true
  resource_group_name                = azurerm_resource_group.res-0.name
  runtime_name                       = "dotnet-isolated"
  runtime_version                    = "10.0"
  service_plan_id                    = azurerm_service_plan.res-23.id
  site_credential                    = [] # Masked sensitive attribute
  storage_access_key                 = ""
  storage_authentication_type        = "StorageAccountConnectionString"
  storage_container_endpoint         = "https://ccdeprojecta2ad.blob.core.windows.net/app-package-ccdeproject-api-50fac68"
  storage_container_type             = "blobContainer"
  storage_user_assigned_identity_id  = ""
  tags = {
    "hidden-link: /app-insights-resource-id" = "/subscriptions/504d8616-e1b7-42c8-8955-80350ee328dd/resourceGroups/CCDE-Project/providers/microsoft.insights/components/ccdeproject-api"
  }
  virtual_network_subnet_id                      = ""
  webdeploy_publish_basic_authentication_enabled = true
  zip_deploy_file                                = ""
  site_config {
    api_definition_url                            = ""
    api_management_api_id                         = ""
    app_command_line                              = ""
    application_insights_connection_string        = "" # Masked sensitive attribute
    application_insights_key                      = "" # Masked sensitive attribute
    container_registry_managed_identity_client_id = ""
    container_registry_use_managed_identity       = false
    default_documents                             = ["Default.htm", "Default.html", "Default.asp", "index.htm", "index.html", "iisstart.htm", "default.aspx", "index.php"]
    elastic_instance_minimum                      = 0
    health_check_eviction_time_in_min             = 0
    health_check_path                             = ""
    http2_enabled                                 = false
    ip_restriction_default_action                 = ""
    load_balancing_mode                           = "LeastRequests"
    managed_pipeline_mode                         = "Integrated"
    minimum_tls_version                           = "1.2"
    remote_debugging_enabled                      = false
    remote_debugging_version                      = ""
    runtime_scale_monitoring_enabled              = false
    scm_ip_restriction_default_action             = ""
    scm_minimum_tls_version                       = "1.2"
    scm_use_main_ip_restriction                   = false
    use_32_bit_worker                             = false
    vnet_route_all_enabled                        = false
    websockets_enabled                            = false
    worker_count                                  = 1
    cors {
      allowed_origins     = ["https://portal.azure.com", "https://thankful-ocean-0bb855e03.4.azurestaticapps.net"]
      support_credentials = false
    }
  }
}
resource "azurerm_function_app_function" "res-28" {
  config_json = jsonencode({
    bindings = [{
      authLevel = "Function"
      direction = "In"
      methods   = ["get"]
      name      = "req"
      route     = "visitor-count"
      type      = "httpTrigger"
      }, {
      direction = "Out"
      name      = "$return"
      type      = "http"
    }]
    entryPoint        = "PortfolioApi.PageView.Run"
    functionDirectory = ""
    language          = "dotnet-isolated"
    name              = "PageView"
    scriptFile        = "PortfolioApi.dll"
  })
  enabled         = true
  function_app_id = azurerm_function_app_flex_consumption.res-24.id
  language        = ""
  name            = "PageView"
  test_data       = ""
}
resource "azurerm_app_service_custom_hostname_binding" "res-29" {
  app_service_name    = "ccdeproject-api"
  hostname            = "ccdeproject-api-d0g8fyewefb2ddfk.germanywestcentral-01.azurewebsites.net"
  resource_group_name = azurerm_resource_group.res-0.name
  ssl_state           = ""
  thumbprint          = ""
  depends_on = [
    azurerm_function_app_flex_consumption.res-24,
  ]
}
resource "azurerm_monitor_action_group" "res-30" {
  enabled             = true
  location            = "global"
  name                = "Application Insights Smart Detection"
  resource_group_name = azurerm_resource_group.res-0.name
  short_name          = "SmartDetect"
  tags                = {}
  arm_role_receiver {
    name                    = "Monitoring Contributor"
    role_id                 = "749f88d5-cbae-40b8-bcfc-e573ddc772fa"
    use_common_alert_schema = true
  }
  arm_role_receiver {
    name                    = "Monitoring Reader"
    role_id                 = "43d0d8ad-25c7-4714-9337-8ba259a9fe05"
    use_common_alert_schema = true
  }
}
resource "azurerm_application_insights" "res-31" {
  application_type                      = "web"
  connection_string                     = "" # Masked sensitive attribute
  daily_data_cap_in_gb                  = 100
  daily_data_cap_notifications_disabled = false
  disable_ip_masking                    = false
  force_customer_storage_for_profiler   = false
  instrumentation_key                   = "" # Masked sensitive attribute
  internet_ingestion_enabled            = true
  internet_query_enabled                = true
  local_authentication_disabled         = false
  location                              = "germanywestcentral"
  name                                  = "ccdeproject-api"
  resource_group_name                   = azurerm_resource_group.res-0.name
  retention_in_days                     = 90
  sampling_percentage                   = 0
  tags                                  = {}
  workspace_id                          = "/subscriptions/504d8616-e1b7-42c8-8955-80350ee328dd/resourceGroups/DefaultResourceGroup-DEWC/providers/Microsoft.OperationalInsights/workspaces/DefaultWorkspace-504d8616-e1b7-42c8-8955-80350ee328dd-DEWC"
}