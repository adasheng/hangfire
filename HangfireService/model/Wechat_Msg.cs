using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HangfireService.model
{
    public class Wechat_Msg
    {
        public partial class WeChatMassSendModelRequstBody
        {
            /// <summary>
            /// 群发任务的类型，默认为single，表示发送给客户，group表示发送给客户群
            /// </summary>
            [JsonProperty("chat_type")]
            public string ChatType { get; set; }

            /// <summary>
            /// 群发任务创建人企业账号id
            /// </summary>
            [JsonProperty("creator", NullValueHandling = NullValueHandling.Ignore)]
            public string Creator { get; set; }

            /// <summary>
            /// 用于分页查询的游标，字符串类型，由上一次调用返回，首次调用可不填
            /// </summary>
            [JsonProperty("cursor", NullValueHandling = NullValueHandling.Ignore)]
            public string Cursor { get; set; }

            /// <summary>
            /// 群发任务记录结束时间
            /// </summary>
            [JsonProperty("end_time")]
            public long EndTime { get; set; }

            /// <summary>
            /// 创建人类型。0：企业发表 1：个人发表 2：所有，包括个人创建以及企业创建，默认情况下为所有类型
            /// </summary>
            [JsonProperty("filter_type", NullValueHandling = NullValueHandling.Ignore)]
            public long? FilterType { get; set; }

            /// <summary>
            /// 返回的最大记录数，整型，最大值100，默认值50，超过最大值时取默认值
            /// </summary>
            [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
            public long? Limit { get; set; }

            /// <summary>
            /// 群发任务记录开始时间
            /// </summary>
            [JsonProperty("start_time")]
            public long StartTime { get; set; }
        }


        public partial class WeChatMassSendModelResult
        {
            /// <summary>
            /// 返回码
            /// </summary>
            [JsonProperty("errcode", NullValueHandling = NullValueHandling.Ignore)]
            public long? Errcode { get; set; }

            /// <summary>
            /// 对返回码的文本描述内容
            /// </summary>
            [JsonProperty("errmsg", NullValueHandling = NullValueHandling.Ignore)]
            public string Errmsg { get; set; }

            /// <summary>
            /// 群发记录列表
            /// </summary>
            [JsonProperty("group_msg_list", NullValueHandling = NullValueHandling.Ignore)]
            public GroupMsgList[] GroupMsgList { get; set; }

            /// <summary>
            /// 分页游标，再下次请求时填写以获取之后分页的记录，如果已经没有更多的数据则返回空
            /// </summary>
            [JsonProperty("next_cursor", NullValueHandling = NullValueHandling.Ignore)]
            public string NextCursor { get; set; }
        }

        public partial class MassSendModel
        {
            public string MsgId { get; set; }

            public string MsgName { get; set; }

            public string CreateDate { get; set; }

            public string Creator { get; set; }

            public string MsgType { get; set; }

            public string MsgOrigin { get; set; }

            public string MsgDetails { get; set; }

        }

        public partial class WeChatSendTaskRequstBody
        {
            /// <summary>
            /// 用于分页查询的游标，字符串类型，由上一次调用返回，首次调用可不填
            /// </summary>
            [JsonProperty("cursor", NullValueHandling = NullValueHandling.Ignore)]
            public string Cursor { get; set; }

            /// <summary>
            /// 返回的最大记录数，整型，最大值1000，默认值500，超过最大值时取默认值
            /// </summary>
            [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
            public long? Limit { get; set; }

            /// <summary>
            /// 群发消息的id，通过获取群发记录列表接口返回
            /// </summary>
            [JsonProperty("msgid")]
            public string Msgid { get; set; }
        }

        public partial class WeChatSendTaskResult
        {
            /// <summary>
            /// 返回码
            /// </summary>
            [JsonProperty("MsgId", NullValueHandling = NullValueHandling.Ignore)]
            public string MsgId { get; set; }
            /// <summary>
            /// 返回码
            /// </summary>
            [JsonProperty("errcode", NullValueHandling = NullValueHandling.Ignore)]
            public long? Errcode { get; set; }

            /// <summary>
            /// 对返回码的文本描述内容
            /// </summary>
            [JsonProperty("errmsg", NullValueHandling = NullValueHandling.Ignore)]
            public string Errmsg { get; set; }

            /// <summary>
            /// 分页游标，再下次请求时填写以获取之后分页的记录，如果已经没有更多的数据则返回空
            /// </summary>
            [JsonProperty("next_cursor", NullValueHandling = NullValueHandling.Ignore)]
            public string NextCursor { get; set; }

            /// <summary>
            /// 群发成员发送任务列表
            /// </summary>
            [JsonProperty("task_list", NullValueHandling = NullValueHandling.Ignore)]
            public TaskList[] TaskList { get; set; }
        }

        public partial class WeChatSendMsgResultRequestBody
        {
            /// <summary>
            /// 用于分页查询的游标，字符串类型，由上一次调用返回，首次调用可不填
            /// </summary>
            [JsonProperty("cursor", NullValueHandling = NullValueHandling.Ignore)]
            public string Cursor { get; set; }

            /// <summary>
            /// 返回的最大记录数，整型，最大值1000，默认值500，超过最大值时取默认值
            /// </summary>
            [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
            public long? Limit { get; set; }

            /// <summary>
            /// 群发消息的id，通过获取群发记录列表接口返回
            /// </summary>
            [JsonProperty("msgid")]
            public string Msgid { get; set; }

            /// <summary>
            /// 发送成员userid，通过获取群发成员发送任务列表接口返回
            /// </summary>
            [JsonProperty("userid")]
            public string Userid { get; set; }
        }

        public partial class WeChatSendMsgResult
        {
            /// <summary>
            /// 对返回码的文本描述内容
            /// </summary>
            [JsonProperty("MsgId", NullValueHandling = NullValueHandling.Ignore)]
            public string MsgId { get; set; }


            /// <summary>
            /// 返回码
            /// </summary>
            [JsonProperty("errcode", NullValueHandling = NullValueHandling.Ignore)]
            public long? Errcode { get; set; }

            /// <summary>
            /// 对返回码的文本描述内容
            /// </summary>
            [JsonProperty("errmsg", NullValueHandling = NullValueHandling.Ignore)]
            public string Errmsg { get; set; }

            /// <summary>
            /// 分页游标，再下次请求时填写以获取之后分页的记录，如果已经没有更多的数据则返回空
            /// </summary>
            [JsonProperty("next_cursor", NullValueHandling = NullValueHandling.Ignore)]
            public string NextCursor { get; set; }

            /// <summary>
            /// 群成员发送结果列表
            /// </summary>
            [JsonProperty("send_list", NullValueHandling = NullValueHandling.Ignore)]
            public SendList[] SendList { get; set; }
        }

        public partial class SendList
        {
            [JsonProperty("chat_id", NullValueHandling = NullValueHandling.Ignore)]
            public string ChatId { get; set; }

            [JsonProperty("external_userid", NullValueHandling = NullValueHandling.Ignore)]
            public string ExternalUserid { get; set; }

            [JsonProperty("send_time", NullValueHandling = NullValueHandling.Ignore)]
            public long? SendTime { get; set; }

            [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
            public long? Status { get; set; }

            [JsonProperty("userid", NullValueHandling = NullValueHandling.Ignore)]
            public string Userid { get; set; }
        }
        public partial class TaskList
        {
            [JsonProperty("send_time", NullValueHandling = NullValueHandling.Ignore)]
            public long? SendTime { get; set; }

            [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
            public long? Status { get; set; }

            [JsonProperty("userid", NullValueHandling = NullValueHandling.Ignore)]
            public string Userid { get; set; }
        }

        public partial class SendTaskModel
        {
            public string WechatUserID { get; set; }
            public long SendTime { get; set; }

            public string Status { get; set; }

            public string MsgId { get; set; }

        }

        public partial class GroupMsgList
        {
            [JsonProperty("attachments", NullValueHandling = NullValueHandling.Ignore)]
            public Attachment[] Attachments { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [JsonProperty("create_time", NullValueHandling = NullValueHandling.Ignore)]
            public string CreateTime { get; set; }

            /// <summary>
            /// 群发消息创建来源。0：企业 1：个人
            /// </summary>
            [JsonProperty("create_type", NullValueHandling = NullValueHandling.Ignore)]
            public long? CreateType { get; set; }

            /// <summary>
            /// 群发消息创建者userid，API接口创建的群发消息不返回该字段
            /// </summary>
            [JsonProperty("creator", NullValueHandling = NullValueHandling.Ignore)]
            public string Creator { get; set; }

            /// <summary>
            /// 企业群发消息的id，可用于获取企业群发成员执行结果
            /// </summary>
            [JsonProperty("msgid", NullValueHandling = NullValueHandling.Ignore)]
            public string Msgid { get; set; }

            [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
            public Text Text { get; set; }
        }

        public partial class Attachment
        {
            [JsonProperty("file", NullValueHandling = NullValueHandling.Ignore)]
            public File File { get; set; }

            [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
            public Image Image { get; set; }

            [JsonProperty("link", NullValueHandling = NullValueHandling.Ignore)]
            public Link Link { get; set; }

            [JsonProperty("miniprogram", NullValueHandling = NullValueHandling.Ignore)]
            public Miniprogram Miniprogram { get; set; }

            /// <summary>
            /// 值值必须是video是image
            /// </summary>
            [JsonProperty("msgtype")]
            public string Msgtype { get; set; }

            [JsonProperty("video", NullValueHandling = NullValueHandling.Ignore)]
            public Video Video { get; set; }
        }

        public partial class File
        {
            [JsonProperty("media_id")]
            public string MediaId { get; set; }
        }

        public partial class Image
        {
            /// <summary>
            /// 图片的media_id，可以通过获取临时素材下载资源
            /// </summary>
            [JsonProperty("media_id")]
            public string MediaId { get; set; }

            /// <summary>
            /// 图片的url，与图片的media_id不能共存优先吐出media_id
            /// </summary>
            [JsonProperty("pic_url")]
            public string PicUrl { get; set; }
        }

        public partial class Link
        {
            /// <summary>
            /// 图文消息的描述，最多512个字节
            /// </summary>
            [JsonProperty("desc")]
            public string Desc { get; set; }

            /// <summary>
            /// 图文消息封面的url
            /// </summary>
            [JsonProperty("picurl")]
            public string Picurl { get; set; }

            /// <summary>
            /// 图文消息标题
            /// </summary>
            [JsonProperty("title")]
            public string Title { get; set; }

            /// <summary>
            /// 图文消息的描述，最多512个字节
            /// </summary>
            [JsonProperty("url")]
            public string Url { get; set; }
        }

        public partial class Miniprogram
        {
            /// <summary>
            /// 小程序appid，必须是关联到企业的小程序应用
            /// </summary>
            [JsonProperty("appid")]
            public string Appid { get; set; }

            /// <summary>
            /// 小程序page路径
            /// </summary>
            [JsonProperty("page")]
            public string Page { get; set; }

            [JsonProperty("pic_media_id")]
            public string PicMediaId { get; set; }

            /// <summary>
            /// 小程序消息标题，最多64个字节
            /// </summary>
            [JsonProperty("title")]
            public string Title { get; set; }
        }

        public partial class Video
        {
            /// <summary>
            /// 视频的media_id，可以通过获取临时素材下载资源
            /// </summary>
            [JsonProperty("media_id")]
            public string MediaId { get; set; }
        }

        public partial class Text
        {
            /// <summary>
            /// 消息文本内容，最多4000个字节
            /// </summary>
            [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
            public string Content { get; set; }
        }

       

       

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }

    }
}
