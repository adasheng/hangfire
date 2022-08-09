using Newtonsoft.Json;

namespace HangfireService.model
{
    #region 朋友圈 发送列表
    public class WechatMomentListRequest
    {
        /// <summary>
        /// 朋友圈创建人的userid
        /// </summary>
        [JsonProperty("creator", NullValueHandling = NullValueHandling.Ignore)]
        public string Creator { get; set; }

        /// <summary>
        /// 用于分页查询的游标，字符串类型，由上一次调用返回，首次调用可不填
        /// </summary>
        [JsonProperty("cursor", NullValueHandling = NullValueHandling.Ignore)]
        public string Cursor { get; set; }

        /// <summary>
        /// 朋友圈记录结束时间。Unix时间戳
        /// </summary>
        [JsonProperty("end_time")]
        public long EndTime { get; set; }

        /// <summary>
        /// 朋友圈类型。0：企业发表 1：个人发表 2：所有，包括个人创建以及企业创建，默认情况下为所有类型
        /// </summary>
        [JsonProperty("filter_type", NullValueHandling = NullValueHandling.Ignore)]
        public long? FilterType { get; set; }

        /// <summary>
        /// 返回的最大记录数，整型，最大值100，默认值100，超过最大值时取默认值
        /// </summary>
        [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? Limit { get; set; }

        /// <summary>
        /// 朋友圈记录开始时间。Unix时间戳
        /// </summary>
        [JsonProperty("start_time")]
        public long StartTime { get; set; }
    }
    public partial class WechatMomentListResult
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
        /// 朋友圈列表
        /// </summary>
        [JsonProperty("moment_list", NullValueHandling = NullValueHandling.Ignore)]
        public MomentList[] MomentList { get; set; }

        /// <summary>
        /// 分页游标，下次请求时填写以获取之后分页的记录，如果已经没有更多的数据则返回空
        /// </summary>
        [JsonProperty("next_cursor", NullValueHandling = NullValueHandling.Ignore)]
        public string NextCursor { get; set; }
    }

    public partial class MomentList
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("create_time", NullValueHandling = NullValueHandling.Ignore)]
        public string CreateTime { get; set; }

        /// <summary>
        /// 朋友圈创建来源。0：企业 1：个人
        /// </summary>
        [JsonProperty("create_type", NullValueHandling = NullValueHandling.Ignore)]
        public long? CreateType { get; set; }

        /// <summary>
        /// 朋友圈创建者userid
        /// </summary>
        [JsonProperty("creator", NullValueHandling = NullValueHandling.Ignore)]
        public string Creator { get; set; }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public Image[] Image { get; set; }

        [JsonProperty("link", NullValueHandling = NullValueHandling.Ignore)]
        public Link Link { get; set; }

        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public Location Location { get; set; }

        /// <summary>
        /// 朋友圈id
        /// </summary>
        [JsonProperty("moment_id", NullValueHandling = NullValueHandling.Ignore)]
        public string MomentId { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public Text Text { get; set; }

        [JsonProperty("video", NullValueHandling = NullValueHandling.Ignore)]
        public Video Video { get; set; }

        /// <summary>
        /// 可见范围类型。0：部分可见 1：公开
        /// </summary>
        [JsonProperty("visible_type", NullValueHandling = NullValueHandling.Ignore)]
        public long? VisibleType { get; set; }
    }

    public partial class Image
    {
        /// <summary>
        /// 图片的media_id列表，可以通过获取临时素材下载资源
        /// </summary>
        [JsonProperty("media_id", NullValueHandling = NullValueHandling.Ignore)]
        public string MediaId { get; set; }
    }

    public partial class Link
    {
        /// <summary>
        /// 网页链接标题
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        /// <summary>
        /// 网页链接url
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
    }

    public partial class Location
    {
        /// <summary>
        /// 地理位置纬度
        /// </summary>
        [JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
        public string Latitude { get; set; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        [JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
        public string Longitude { get; set; }

        /// <summary>
        /// 地理位置名称
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public partial class Text
    {
        /// <summary>
        /// 文本消息结构
        /// </summary>
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }
    }

    public partial class Video
    {
        /// <summary>
        /// 视频media_id，可以通过获取临时素材下载资源
        /// </summary>
        [JsonProperty("media_id", NullValueHandling = NullValueHandling.Ignore)]
        public string MediaId { get; set; }

        /// <summary>
        /// 视频封面media_id，可以通过获取临时素材下载资源
        /// </summary>
        [JsonProperty("thumb_media_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ThumbMediaId { get; set; }
    }

    #endregion

    #region 朋友圈 发送任务成员

    public partial class WechatMomentUserRequest
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
        /// 朋友圈id,仅支持企业发表的朋友圈id
        /// </summary>
        [JsonProperty("moment_id")]
        public string MomentId { get; set; }
    }

    public partial class WechatMomentUserResult
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
        /// 分页游标，再下次请求时填写以获取之后分页的记录，如果已经没有更多的数据则返回空
        /// </summary>
        [JsonProperty("next_cursor", NullValueHandling = NullValueHandling.Ignore)]
        public string NextCursor { get; set; }

        /// <summary>
        /// 发表任务列表
        /// </summary>
        [JsonProperty("task_list", NullValueHandling = NullValueHandling.Ignore)]
        public TaskList[] TaskList { get; set; }
    }

    public partial class TaskList
    {
        /// <summary>
        /// 成员发表状态。0:未发表 1：已发表
        /// </summary>
        [JsonProperty("publish_status", NullValueHandling = NullValueHandling.Ignore)]
        public long? PublishStatus { get; set; }

        /// <summary>
        /// 发表成员用户userid
        /// </summary>
        [JsonProperty("userid", NullValueHandling = NullValueHandling.Ignore)]
        public string Userid { get; set; }
    }
    #endregion


    #region  朋友圈 发送客户
    public partial class WechatMomentCustomerRequest
    {
        /// <summary>
        /// 用于分页查询的游标，字符串类型，由上一次调用返回，首次调用可不填
        /// </summary>
        [JsonProperty("cursor", NullValueHandling = NullValueHandling.Ignore)]
        public string Cursor { get; set; }

        /// <summary>
        /// 返回的最大记录数，整型，最大值5000，默认值3000，超过最大值时取默认值
        /// </summary>
        [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? Limit { get; set; }

        /// <summary>
        /// 朋友圈id
        /// </summary>
        [JsonProperty("moment_id")]
        public string MomentId { get; set; }

        /// <summary>
        ///
        /// 企业发表成员userid，如果是企业创建的朋友圈，可以通过获取客户朋友圈企业发表的列表获取已发表成员userid，如果是个人创建的朋友圈，创建人userid就是企业发表成员userid
        /// </summary>
        [JsonProperty("userid")]
        public string Userid { get; set; }
    }

    public partial class WechatMomentCustomerResult
    {
        /// <summary>
        /// 成员发送成功客户列表
        /// </summary>
        [JsonProperty("customer_list", NullValueHandling = NullValueHandling.Ignore)]
        public CustomerList[] CustomerList { get; set; }

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
    }

    public partial class CustomerList
    {
        /// <summary>
        /// 成员发送成功的外部联系人userid
        /// </summary>
        [JsonProperty("external_userid", NullValueHandling = NullValueHandling.Ignore)]
        public string ExternalUserid { get; set; }
    }
    #endregion

}
