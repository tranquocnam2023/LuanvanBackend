namespace EMS_Backend.Helpers
{
    public static class ErrorCodes
    {
        #region Common Error Codes
        public const string DATABASE_CONNECTION_ERROR = "DATABASE_CONNECTION_ERROR";
        public const string VALIDATION_ERROR = "VALIDATION_ERROR";
        public const string RESOURCE_NOT_FOUND = "RESOURCE_NOT_FOUND";
        public const string OPERATION_FAILED = "OPERATION_FAILED";
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
        public const string INVALID_REQUEST = "INVALID_REQUEST";
        #endregion

        #region User Error Codes
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string USER_ALREADY_EXISTS = "USER_ALREADY_EXISTS";
        public const string INVALID_TOKEN = "INVALID_TOKEN";
        public const string ACCESS_DENIED = "ACCESS_DENIED";        
        public const string USER_DELETE_CONFLICT = "USER_DELETE_CONFLICT";
        public const string USER_UPDATE_CONFLICT = "USER_UPDATE_CONFLICT";
        public const string USER_ADD_CONFLICT = "USER_ADD_CONFLICT";
        #endregion

        #region Category Error Codes
        public const string CATEGORY_NOT_FOUND = "CATEGORY_NOT_FOUND";
        public const string CATEGORY_DELETE_CONFLICT = "CATEGORY_DELETE_CONFLICT";
        public const string CATEGORY_UPDATE_CONFLICT = "CATEGORY_UPDATE_CONFLICT";
        public const string CATEGORY_ADD_CONFLICT = "CATEGORY_ADD_CONFLICT";
        #endregion

        #region Supplier Error Codes
        public const string SUPPLIER_NOT_FOUND = "SUPPLIER_NOT_FOUND";
        public const string SUPPLIER_DELETE_CONFLICT = "SUPPLIER_DELETE_CONFLICT";
        public const string SUPPLIER_UPDATE_CONFLICT = "SUPPLIER_UPDATE_CONFLICT";
        public const string SUPPLIER_ADD_CONFLICT = "SUPPLIER_ADD_CONFLICT";
        #endregion

        #region Role Error Codes
        public const string ROLE_NOT_FOUND = "ROLE_NOT_FOUND";
        public const string ROLE_DELETE_CONFLICT = "ROLE_DELETE_CONFLICT";
        public const string ROLE_UPDATE_CONFLICT = "ROLE_UPDATE_CONFLICT";
        public const string ROLE_ADD_CONFLICT = "ROLE_ADD_CONFLICT";
        #endregion

        #region Product Error Codes
        public const string PRODUCT_NOT_FOUND = "PRODUCT_NOT_FOUND";
        public const string PRODUCT_DELETE_CONFLICT = "PRODUCT_DELETE_CONFLICT";
        public const string PRODUCT_UPDATE_CONFLICT = "PRODUCT_UPDATE_CONFLICT";
        public const string PRODUCT_ADD_CONFLICT = "PRODUCT_ADD_CONFLICT";
        #endregion

        #region Function Error Codes
        public const string FUNCTION_NOT_FOUND = "FUNCTION_NOT_FOUND";
        public const string FUNCTION_DELETE_CONFLICT = "FUNCTION_DELETE_CONFLICT";
        public const string FUNCTION_UPDATE_CONFLICT = "FUNCTION_UPDATE_CONFLICT";
        public const string FUNCTION_ADD_CONFLICT = "FUNCTION_ADD_CONFLICT";
        #endregion
    }
}
