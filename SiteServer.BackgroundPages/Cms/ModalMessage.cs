﻿using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalMessage : BasePageCms
    {
        public Literal LtlHtml;

        private string _type;
        public const string TypePreviewImage = "PreviewImage";
        public const string TypePreviewVideo = "PreviewVideo";
        public const string TypePreviewVideoByUrl = "PreviewVideoByUrl";

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl(string html)
        {
            return PageUtils.GetCmsUrl(nameof(ModalMessage), new NameValueCollection
            {
                {"html", TranslateUtils.EncryptStringBySecretKey(html)}
            });
        }

        public static string GetOpenWindowString(string title, string html, int width, int height)
        {
            return PageUtils.GetOpenLayerString(title, PageUtils.GetCmsUrl(nameof(ModalMessage), new NameValueCollection
            {
                {"html", TranslateUtils.EncryptStringBySecretKey(html)}
            }), width, height);
        }

        public static string GetOpenWindowStringToPreviewImage(int publishmentSystemId, string textBoxClientId)
        {
            return PageUtils.GetOpenLayerString("预览图片", PageUtils.GetCmsUrl(nameof(ModalMessage), new NameValueCollection
            {
                {"type", TypePreviewImage},
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"textBoxClientID", textBoxClientId}
            }), 500, 500);
        }

        public static string GetOpenWindowStringToPreviewVideo(int publishmentSystemId, string textBoxClientId)
        {
            return PageUtils.GetOpenLayerString("预览视频", PageUtils.GetCmsUrl(nameof(ModalMessage), new NameValueCollection
            {
                {"type", TypePreviewVideo},
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"textBoxClientID", textBoxClientId}
            }), 500, 500);
        }

        public static string GetOpenWindowStringToPreviewVideoByUrl(int publishmentSystemId, string videoUrl)
        {
            return PageUtils.GetOpenLayerString("预览视频", PageUtils.GetCmsUrl(nameof(ModalMessage), new NameValueCollection
            {
                {"type", TypePreviewVideoByUrl},
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"videoUrl", videoUrl}
            }), 500, 500);
        }

        public static string GetRedirectStringToPreviewVideoByUrl(int publishmentSystemId, string videoUrl)
        {
            return PageUtils.GetCmsUrl(nameof(ModalMessage), new NameValueCollection
            {
                {"type", TypePreviewVideoByUrl},
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"videoUrl", videoUrl}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _type = Request.QueryString["type"];

            if (IsPostBack) return;

            if (StringUtils.EqualsIgnoreCase(_type, TypePreviewImage))
            {
                var publishmentSystemId = Body.GetQueryInt("publishmentSystemID");
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var textBoxClientId = Body.GetQueryString("textBoxClientID");
                LtlHtml.Text = $@"
<span id=""previewImage""></span>
<script>
var rootUrl = '{PageUtils.GetRootUrl(string.Empty)}';
var publishmentSystemUrl = '{PageUtils.ParseNavigationUrl($"~/{publishmentSystemInfo.PublishmentSystemDir}")}';
var imageUrl = window.parent.document.getElementById('{textBoxClientId}').value;
if(imageUrl && imageUrl.search(/\.bmp|\.jpg|\.jpeg|\.gif|\.png$/i) != -1){{
	if (imageUrl.charAt(0) == '~'){{
		imageUrl = imageUrl.replace('~', rootUrl);
	}}else if (imageUrl.charAt(0) == '@'){{
		imageUrl = imageUrl.replace('@', publishmentSystemUrl);
	}}
	if(imageUrl.substr(0,2)=='//'){{
		imageUrl = imageUrl.replace('//', '/');
	}}
    $('#previewImage').html('<img src=""' + imageUrl + '"" class=""img-polaroid"" />');
}}
</script>
";
            }
            else if (StringUtils.EqualsIgnoreCase(_type, TypePreviewVideo))
            {
                var publishmentSystemId = Body.GetQueryInt("publishmentSystemID");
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var textBoxClientId = Body.GetQueryString("textBoxClientID");

                LtlHtml.Text = $@"
<span id=""previewVideo""></span>
<script>
var rootUrl = '{PageUtils.GetRootUrl(string.Empty)}';
var publishmentSystemUrl = '{PageUtils.ParseNavigationUrl($"~/{publishmentSystemInfo.PublishmentSystemDir}")}';
var videoUrl = window.parent.document.getElementById('{textBoxClientId}').value;
if (videoUrl.charAt(0) == '~'){{
	videoUrl = videoUrl.replace('~', rootUrl);
}}else if (videoUrl.charAt(0) == '@'){{
	videoUrl = videoUrl.replace('@', publishmentSystemUrl);
}}
if(videoUrl.substr(0,2)=='//'){{
	videoUrl = videoUrl.replace('//', '/');
}}
if (videoUrl){{
    $('#previewVideo').html('<embed src=""../assets/player.swf"" allowfullscreen=""true"" flashvars=""controlbar=over&autostart=true&file='+videoUrl+'"" width=""{450}"" height=""{350}""/>');
}}
</script>
";
            }
            else if (StringUtils.EqualsIgnoreCase(_type, TypePreviewVideoByUrl))
            {
                var publishmentSystemId = Body.GetQueryInt("publishmentSystemID");
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var videoUrl = Body.GetQueryString("videoUrl");

                LtlHtml.Text = $@"
<embed src=""../assets/player.swf"" allowfullscreen=""true"" flashvars=""controlbar=over&autostart=true&file={PageUtility
                    .ParseNavigationUrl(publishmentSystemInfo, videoUrl, true)}"" width=""{450}"" height=""{350}""/>
";
            }
            else
            {
                LtlHtml.Text = TranslateUtils.DecryptStringBySecretKey(Request.QueryString["html"]);
            }
        }
    }
}
