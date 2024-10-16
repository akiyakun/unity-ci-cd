using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace unicicd.Editor
{
    public class CICDBuildResult
    {
        public bool BuildSucceeded = false;
        public string BuildDirectory = "";

        public static CICDBuildResult CreateFailed()
        {
            return new CICDBuildResult
            {
                BuildSucceeded = false,
            };
        }
    }
}
