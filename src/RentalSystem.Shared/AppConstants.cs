using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;

namespace RentalSystem.Shared.AppConstants
{
    public static class AppConstants
    {
        static AppConstants()
        {
            try
            {
                Env.TraversePath().Load();
            }
            catch (Exception)
            {
                throw new Exception("Nie można załadować zmiennych środowiskowych z pliku .env");
            }
        }
        private static string Get(string key, string defaultValue = "")
        {
            return Environment.GetEnvironmentVariable(key) ?? defaultValue;
        }

        // Firebase
        public static string FIREBASE_API_KEY => Get("FIREBASE_API_KEY");
        public const string FIREBASE_AUTH_DOMAIN = "rentathing-cb445.firebaseapp.com";
        public const string FIREBASE_KEY_FILENAME = "firebase_config.json";
        public const string FIRESTORE_PROJECT_ID = "rentathing-cb445";
        public const string FIREBASE_USER_COLLECTION = "users";

        // Setup
        public const int BACKEND_GRPC_PORT = 5003;
        public const string BACKEND_HOST = "http://localhost";
        public const int BACKEND_HTTP_PORT = 5002;
        public static string BACKEND_GRPC_URL => $"{BACKEND_HOST}:{BACKEND_GRPC_PORT}";
        public static string BACKEND_REST_URL => $"{BACKEND_HOST}:{BACKEND_HTTP_PORT}";
    }
}