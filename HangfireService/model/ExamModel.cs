using System.Collections.Generic;

namespace HangfireService.model
{
    public class ExamModel
    {

        public class InitialValues
        {
        }

        public class IpLimit
        {
            /// <summary>
            /// 
            /// </summary>
            public int limitNum { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string limitFreq { get; set; }
        }

        public class CookieLimit
        {
            /// <summary>
            /// 
            /// </summary>
            public int limitNum { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string limitFreq { get; set; }
        }

        public class LoginLimit
        {
            /// <summary>
            /// 
            /// </summary>
            public int limitNum { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string limitFreq { get; set; }
        }

        public class AnswerSetting
        {
            /// <summary>
            /// 
            /// </summary>
            public string password { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string progressBar { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string loginRequired { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string questionNumber { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string autoSave { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public InitialValues initialValues { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int maxAnswers { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string wechatOnly { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public IpLimit ipLimit { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public CookieLimit cookieLimit { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public LoginLimit loginLimit { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string onePageOneQuestion { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string answerSheetVisible { get; set; }
        }

        public class SubmittedSetting
        {
            /// <summary>
            /// 
            /// </summary>
            public string contentHtml { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string enableUpdate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string redirectUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string answerAnalysis { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string transcriptVisible { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string rankVisible { get; set; }
        }

        public class ExamSetting
        {
            /// <summary>
            /// 
            /// </summary>
            public long startTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public long endTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int minSubmitMinutes { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int maxSubmitMinutes { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string exerciseMode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string randomOrder { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> randomSurvey { get; set; }
        }

        public class ExamSettingModel
        {
            /// <summary>
            /// 
            /// </summary>
            public int status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public AnswerSetting answerSetting { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public SubmittedSetting submittedSetting { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public ExamSetting examSetting { get; set; }
        }



        public class ClientInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public string agent { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string browser { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string platformVersion { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string browserVersion { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string platform { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string remoteIp { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string deviceType { get; set; }
        }

        public class AnswerInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public long startTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public long endTime { get; set; }
        }

        public class ExamClient
        {
            /// <summary>
            /// 
            /// </summary>
            public ClientInfo clientInfo { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public AnswerInfo answerInfo { get; set; }
        }



    }
}
