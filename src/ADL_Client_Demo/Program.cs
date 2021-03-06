﻿using System;
using System.Collections.Generic;
using System.Linq;
using AzureDataLakeClient.Analytics;
using Microsoft.Azure.Management.DataLake.Analytics.Models;

namespace ADL_Client_Demo
{
    class Program
    {
        private static void Main(string[] args)
        {
            string tenant = "microsoft.onmicrosoft.com"; // change this to YOUR tenant
            string adla_account = "datainsightsadhoc"; // change this to an ADL Analytics account you have access to 
            string adls_account = "datainsightsadhoc"; // change this to an ADL Store account you have access to 

            var auth_session = new AzureDataLakeClient.Authentication.AuthenticatedSession("ADL_Demo_Client", tenant);
            auth_session.Authenticate();

            var job_client = new AzureDataLakeClient.Analytics.AnalyticsJobClient(adla_account, auth_session);

            //Demo_Get10MostRecentJobs(job_client);
            //Demo_Get5FailedJobs(job_client);
            //Demo_GetJobsSubmittedByMe(job_client);
            //Demo_GetJobsSubmittedByUsers(job_client);
            //Demo_GetJobsSubmitedSinceMidnight(job_client);
            // Demo_GetJobs_Submitter_Begins_With(job_client);
            Demo_GetJobs_Submitter_Contains(job_client);

            //var fs_client = new AzureDataLakeClient.Store.StoreFileSystemClient(adls_account, auth_session);
            //Demo_ListFilesAtRoot(fs_client);
        }

        private static void Demo_ListFilesAtRoot(AzureDataLakeClient.Store.StoreFileSystemClient fs_client)
        {
            //var root = AzureDataLakeClient.Store.FsPath.Root; // same as "/"
            var root = new AzureDataLakeClient.Store.FsPath("/Samples");
            var lfo = new AzureDataLakeClient.Store.ListFilesOptions();
            foreach (var page in fs_client.ListFilesPaged(root,lfo))
            {
                foreach (var fileitemn in page.FileItems)
                {
                    Console.WriteLine("path={0} filename={1}",page.Path,fileitemn.PathSuffix);                    
                }
            }

        }

        private static void Demo_GetJobsSubmittedByMe(AnalyticsJobClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 10;
            opts.Filter.SubmitterIsCurrentUser = true;

            var jobs = job_client.GetJobs(opts);

            PrintJobs(jobs);
        }


        private static void Demo_GetJobsSubmittedByUsers(AnalyticsJobClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 10;
            opts.Filter.Submitter.OneOf("mrys@microsoft.com", "saveenr@microsoft.com");

            var jobs = job_client.GetJobs(opts);

            PrintJobs(jobs);
        }

        private static void Demo_GetJobs_Submitter_Begins_With(AnalyticsJobClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 10;
            opts.Filter.Submitter.BeginsWith("saa");

            var jobs = job_client.GetJobs(opts);

            PrintJobs(jobs);
        }

        private static void Demo_GetJobs_Submitter_Contains(AnalyticsJobClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 10;
            opts.Filter.Submitter.Contains("eenr");

            var jobs = job_client.GetJobs(opts);

            PrintJobs(jobs);
        }


        private static void Demo_Get10MostRecentJobs(AnalyticsJobClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 10;

            var jobfields = new AzureDataLakeClient.Analytics.JobListFields();
            opts.Sorting.Direction = OrderByDirection.Descending;
            opts.Sorting.Field = jobfields.field_submittime;

            var jobs = job_client.GetJobs(opts);

            PrintJobs(jobs);
        }

        private static void Demo_Get5FailedJobs(AnalyticsJobClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 5;

            opts.Filter.Result.OneOf(JobResult.Failed);

            var jobs = job_client.GetJobs(opts);

            PrintJobs(jobs);
        }

        private static void Demo_GetJobsSubmitedInLast2hours(AnalyticsJobClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Filter.SubmitTime.InRange(AzureDataLakeClient.OData.Utils.RangeDateTime.InTheLastNHours(2));
            var jobs = job_client.GetJobs(opts);
            PrintJobs(jobs);
        }

        private static void Demo_GetJobsSubmitedSinceMidnight(AnalyticsJobClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Filter.SubmitTime.InRange(AzureDataLakeClient.OData.Utils.RangeDateTime.SinceLocalMidnight());
            var jobs = job_client.GetJobs(opts);
            PrintJobs(jobs);
        }

        private static void PrintJobs(IEnumerable<JobInformation> jobs)
        {
            foreach (var job in jobs)
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("Name = {0}", job.Name);
                Console.WriteLine("DoP = {0}; Priority = {1}", job.DegreeOfParallelism, job.Priority);
                Console.WriteLine("Result = {0}; State = {1}", job.Result, job.State);
                Console.WriteLine("SubmitTime = {0} [ Local = {1} ] ", job.SubmitTime.Value, job.SubmitTime.Value.ToLocalTime());
                Console.WriteLine("Submitter = {0}", job.Submitter);
            }
        }


    }
}
