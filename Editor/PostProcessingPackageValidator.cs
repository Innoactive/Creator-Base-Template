using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace Innoactive.Creator.XRInteraction.Editors.Utils
{
    /// <summary>
    /// Makes sure the Post Processing package is enabled.
    /// </summary>
    [InitializeOnLoad]
    public class PostProcessingPackageValidator : MonoBehaviour
    {
        private const string Package = "com.unity.postprocessing";
        
        private enum PackageRequestStatus
        {
            None,
            Init,
            Listing,
            Adding,
            WaitingForPackage,
            Failure
        }
        
        private static ListRequest listRequest;
        private static AddRequest addRequest;
        private static PackageCollection packageCollection;

        private static PackageRequestStatus packageRequestStatus = PackageRequestStatus.None;
        
        static PostProcessingPackageValidator()
        {
            packageRequestStatus = PackageRequestStatus.Init;
            EditorApplication.update += EditorUpdate;
        }

        private static void EditorUpdate()
        {
            switch (packageRequestStatus)
            {
                case PackageRequestStatus.Init:
                    RequestPackageList();
                    break;
                case PackageRequestStatus.Listing:
                    RetrievePackageListResult();
                    break;
                case PackageRequestStatus.Adding:
                    AddPackageIfMissing();
                    break;
                case PackageRequestStatus.WaitingForPackage:
                    WaitForPackageStatus();
                    break;
                default:
                    CleanPackageValidation();
                    break;
            }
        }

        private static void RequestPackageList()
        {
            listRequest = Client.List();
            packageRequestStatus = PackageRequestStatus.Listing;
        }

        private static void RetrievePackageListResult()
        {
            if (listRequest == null || listRequest.IsCompleted == false)
            {
                return;
            }

            if (listRequest.Status == StatusCode.Failure)
            {
                packageRequestStatus = PackageRequestStatus.Failure;
                Debug.LogErrorFormat("There was an error trying to enable '{0}' - Error Code: [{1}] .\n{2}", Package, listRequest.Error.errorCode, listRequest.Error.message);
            }
            else
            {
                packageRequestStatus = PackageRequestStatus.Adding;
                packageCollection = listRequest.Result;
            }

            listRequest = null;
        }

        private static void AddPackageIfMissing()
        {
            if (packageCollection.Any(packageInfo => packageInfo.name == Package))
            {
                packageRequestStatus = PackageRequestStatus.None;
            }
            else
            {
                packageRequestStatus = PackageRequestStatus.WaitingForPackage;
                addRequest = Client.Add(Package);
            }
        }

        private static void WaitForPackageStatus()
        {
            if (addRequest == null || addRequest.IsCompleted == false)
            {
                return;
            }

            if (addRequest.Status >= StatusCode.Failure)
            {
                packageRequestStatus = PackageRequestStatus.Failure;
                Debug.LogErrorFormat("There was an error trying to enable '{0}' - Error Code: [{1}] .\n{2}", Package, addRequest.Error.errorCode, addRequest.Error.message);
            }
            else
            {
                packageRequestStatus = PackageRequestStatus.None;
                Debug.LogFormat("The package '{0} version {1}' has been automatically added", addRequest.Result.displayName, addRequest.Result.version);
            }

            addRequest = null;
        }

        private static void CleanPackageValidation()
        {
            packageCollection = null;
                    
            packageRequestStatus = PackageRequestStatus.None;
            EditorApplication.update -= EditorUpdate;
        }
    }
}
