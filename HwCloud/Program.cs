

using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;


//https://account.huaweicloud.com/usercenter/rest/me
Console.WriteLine("登录https://www.huaweicloud.com/ 然后打开网站 https://account.huaweicloud.com/usercenter/rest/me 控制台获取cookie填入到下面。");

Console.WriteLine("请填入cookie：");
var cookie = Console.ReadLine();
Console.WriteLine("请选择地区：\r\n1:香港\r\n2:新加坡");
var ap= Console.ReadLine();

//ap-southeast-1 香港
//ap-southeast-3 新加坡
var projectsName = "ap-southeast-3";
if (ap == "1")
{
    projectsName = "ap-southeast-1";
}

var instanceName = "Debian12-" + RandomNumberGenerator.GetInt32(0, 100000000);



var client = new HttpClient();

client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
client.DefaultRequestHeaders.Add("x-language", "zh-cn");
client.DefaultRequestHeaders.Add("origin", "https://console.huaweicloud.com");
client.DefaultRequestHeaders.Add("referer", "https://console.huaweicloud.com/smb/mobile/?region=cn-north-4");
client.DefaultRequestHeaders.Add("Cookie", cookie);

var request = new HttpRequestMessage(HttpMethod.Post, "https://console.huaweicloud.com/smb/rest/bss/v1/orders");

var response = await client.SendAsync(request);
response.EnsureSuccessStatusCode();
var orderText= await response.Content.ReadAsStringAsync();
using var orderTextDoc = JsonDocument.Parse(orderText);
string orderId = orderTextDoc.RootElement.GetProperty("orderId").GetString();

Console.WriteLine("创建订单："+orderId);

var projects=await client.GetStringAsync("https://console.huaweicloud.com/smb/rest/iam/v3/projects?lang=zh-cn");
using var projectsDoc = JsonDocument.Parse(projects);
// 查找 projects 中 name 为 ap-southeast-1 的项
string? projectId = projectsDoc.RootElement.GetProperty("projects")
    .EnumerateArray()
    .FirstOrDefault(p => p.GetProperty("name").GetString() == projectsName)
    .GetProperty("id")
    .GetString();

Console.WriteLine("获取Id：" + projectId);


request = new HttpRequestMessage(HttpMethod.Post, "https://console.huaweicloud.com/smb/rest/corscrm/v1/orders/subscription");

var content = ap == "1"? 
    new StringContent("{\"order_infos\":[{\"order_id\":\""+ orderId + "\",\"purchase_scene\":{\"shopping_mall\":\"CLOUD_SERVICE\"},\"purchase_scene_param\":\"{\\\"service_type_code\\\":\\\"hws.service.type.hcss\\\"}\",\"customer_info\":{\"project_id\":\""+ projectId + "\"},\"offering_infos\":[{\"subscription_num\":1,\"offering_info\":{\"offering_id\":\"OFFI1005427793455669288\",\"offering_num\":1,\"subscribe_classify\":\"INDEPENDENT_PERIODIC_CLOUD_SERVICE\",\"subscribe_classify_param\":\"{\\\"period_type\\\":\\\"MONTH\\\",\\\"period_num\\\":1,\\\"region_code\\\":\\\"ap-southeast-1\\\"}\",\"provision_info\":{\"cloud_service_form\":{\"instance_name\":\""+ instanceName + "\",\"description\":\"\",\"product_id\":\"670514055d7571632a19bb3b\",\"plan_id\":\"670514055d7571632a19bb3f\",\"region\":\"ap-southeast-1\",\"period_type\":\"MONTH\",\"period_num\":1,\"purchase_quantity\":1,\"charging_mode\":\"prePaid\",\"auto_recharge\":false,\"order_id\":\""+ orderId + "\",\"custom_tags\":[{\"key\":\"product_type\",\"value\":\"light_server\"},{\"key\":\"plan_type\",\"value\":\"basic\"},{\"key\":\"image_name\",\"value\":\"Debian\"}],\"configurable_vars\":[{\"var_key\":\"ecs_product_id\",\"var_value\":\"OFFI985166072920064007\"},{\"var_key\":\"cbc_product_id\",\"var_value\":\"OFFI989800017645719552\"},{\"var_key\":\"is_evs_enabled\",\"var_value\":false},{\"var_key\":\"is_cbr_enabled\",\"var_value\":false},{\"var_key\":\"is_hss_enabled\",\"var_value\":false}],\"cloud_service_type\":\"hws.service.type.hcss\",\"resource_type\":\"hws.resource.type.hecsfusion\",\"resource_spec_code\":\"hf.large.05.40g.30m.linux\"}},\"product_info\":{\"service_type_code\":\"hws.service.type.hcss\",\"resource_type_code\":\"hws.resource.type.hecsfusion\",\"resource_spec_code\":\"hf.large.05.40g.30m.linux\",\"resource_name\":\""+ instanceName + "\",\"product_spec_desc\":\"{\\\"specDesc\\\":{\\\"zh-cn\\\":{\\\"Flexus应用服务器L实例\\\":\\\"2核 | 1G | Debian 12.0 | 系统盘 40 GiB\\\",\\\"弹性公网IP\\\":\\\"峰值带宽 30 Mbps\\\",\\\"流量包\\\":\\\"1024 GB\\\"}}}\",\"product_spec_abbreviation_desc\":\"\"},\"relation_offerings\":[{\"subscribe_classify\":\"INDEPENDENT_PERIODIC_CLOUD_SERVICE\",\"subscribe_classify_param\":\"{\\\"period_type\\\":\\\"MONTH\\\",\\\"period_num\\\":1,\\\"region_code\\\":\\\"ap-southeast-1\\\"}\",\"offering_id\":\"OFFI985166072920064007\",\"offering_num\":1,\"product_info\":{\"service_type_code\":\"hws.service.type.ec2\",\"resource_type_code\":\"hws.resource.type.vm\",\"resource_spec_code\":\"t7.large.05.linux\",\"spec_size\":null,\"spec_size_measure_id\":null,\"product_num\":1}},{\"subscribe_classify\":\"INDEPENDENT_PERIODIC_CLOUD_SERVICE\",\"subscribe_classify_param\":\"{\\\"period_type\\\":\\\"MONTH\\\",\\\"period_num\\\":1,\\\"region_code\\\":\\\"ap-southeast-1\\\"}\",\"offering_id\":\"OFFI984841507560419328\",\"offering_num\":1,\"product_info\":{\"service_type_code\":\"hws.service.type.ebs\",\"resource_type_code\":\"hws.resource.type.volume\",\"resource_spec_code\":\"GPSSD_for_smb\",\"spec_size\":40,\"spec_size_measure_id\":17,\"product_num\":1}},{\"subscribe_classify\":\"INDEPENDENT_RIGHTS_AND_INTERESTS\",\"subscribe_classify_param\":\"{\\\"period_type\\\":\\\"MONTH\\\",\\\"period_num\\\":1,\\\"region_code\\\":\\\"ap-southeast-1\\\"}\",\"offering_id\":\"OFFI989800017645719552\",\"offering_num\":1,\"product_info\":{\"service_type_code\":\"hws.service.type.vpc\",\"resource_type_code\":\"hws.resource.type.bandwidth\",\"resource_spec_code\":\"12_bgp_1024G_30M_HECS\",\"spec_size\":null,\"spec_size_measure_id\":null,\"product_num\":1}},{\"subscribe_classify\":\"INDEPENDENT_PERIODIC_CLOUD_SERVICE\",\"subscribe_classify_param\":\"{\\\"period_type\\\":\\\"MONTH\\\",\\\"period_num\\\":1,\\\"region_code\\\":\\\"ap-southeast-1\\\"}\",\"offering_id\":\"00301-226402-0--0\",\"offering_num\":1,\"product_info\":{\"service_type_code\":\"hws.service.type.ec2\",\"resource_type_code\":\"hws.resource.type.vm.image\",\"resource_spec_code\":\"image\",\"spec_size\":null,\"spec_size_measure_id\":null,\"product_num\":1}}]}}]}]}", null, "application/json") : 
    new StringContent("{\"order_infos\":[{\"order_id\":\""+ orderId + "\",\"purchase_scene\":{\"shopping_mall\":\"CLOUD_SERVICE\"},\"purchase_scene_param\":\"{\\\"service_type_code\\\":\\\"hws.service.type.hcss\\\"}\",\"customer_info\":{\"project_id\":\""+ projectId + "\"},\"offering_infos\":[{\"subscription_num\":1,\"offering_info\":{\"offering_id\":\"OFFI1005427793459863570\",\"offering_num\":1,\"subscribe_classify\":\"INDEPENDENT_PERIODIC_CLOUD_SERVICE\",\"subscribe_classify_param\":\"{\\\"period_type\\\":\\\"MONTH\\\",\\\"period_num\\\":1,\\\"region_code\\\":\\\"ap-southeast-3\\\"}\",\"provision_info\":{\"cloud_service_form\":{\"instance_name\":\""+ instanceName + "\",\"description\":\"\",\"product_id\":\"66683fa6582e671497d2d32e\",\"plan_id\":\"66683fa6582e671497d2d331\",\"region\":\"ap-southeast-3\",\"period_type\":\"MONTH\",\"period_num\":1,\"purchase_quantity\":1,\"charging_mode\":\"prePaid\",\"auto_recharge\":false,\"order_id\":\""+ orderId + "\",\"custom_tags\":[{\"key\":\"product_type\",\"value\":\"light_server\"},{\"key\":\"plan_type\",\"value\":\"basic\"},{\"key\":\"image_name\",\"value\":\"CentOS\"}],\"configurable_vars\":[{\"var_key\":\"ecs_product_id\",\"var_value\":\"OFFI985166072928452623\"},{\"var_key\":\"cbc_product_id\",\"var_value\":\"OFFI989799820086366208\"},{\"var_key\":\"is_evs_enabled\",\"var_value\":false},{\"var_key\":\"is_cbr_enabled\",\"var_value\":false},{\"var_key\":\"is_hss_enabled\",\"var_value\":false}],\"cloud_service_type\":\"hws.service.type.hcss\",\"resource_type\":\"hws.resource.type.hecsfusion\",\"resource_spec_code\":\"hf.large.05.40g.30m.linux\"}},\"product_info\":{\"service_type_code\":\"hws.service.type.hcss\",\"resource_type_code\":\"hws.resource.type.hecsfusion\",\"resource_spec_code\":\"hf.large.05.40g.30m.linux\",\"resource_name\":\""+ instanceName + "\",\"product_spec_desc\":\"{\\\"specDesc\\\":{\\\"zh-cn\\\":{\\\"Flexus应用服务器L实例\\\":\\\"2核 | 1G | CentOS 8.2 | 系统盘 40 GiB\\\",\\\"弹性公网IP\\\":\\\"峰值带宽 30 Mbps\\\",\\\"流量包\\\":\\\"1024 GB\\\"}}}\",\"product_spec_abbreviation_desc\":\"\"},\"relation_offerings\":[{\"subscribe_classify\":\"INDEPENDENT_PERIODIC_CLOUD_SERVICE\",\"subscribe_classify_param\":\"{\\\"period_type\\\":\\\"MONTH\\\",\\\"period_num\\\":1,\\\"region_code\\\":\\\"ap-southeast-3\\\"}\",\"offering_id\":\"OFFI985166072928452623\",\"offering_num\":1,\"product_info\":{\"service_type_code\":\"hws.service.type.ec2\",\"resource_type_code\":\"hws.resource.type.vm\",\"resource_spec_code\":\"t7.large.05.linux\",\"spec_size\":null,\"spec_size_measure_id\":null,\"product_num\":1}},{\"subscribe_classify\":\"INDEPENDENT_PERIODIC_CLOUD_SERVICE\",\"subscribe_classify_param\":\"{\\\"period_type\\\":\\\"MONTH\\\",\\\"period_num\\\":1,\\\"region_code\\\":\\\"ap-southeast-3\\\"}\",\"offering_id\":\"OFFI995583411575369729\",\"offering_num\":1,\"product_info\":{\"service_type_code\":\"hws.service.type.ebs\",\"resource_type_code\":\"hws.resource.type.volume\",\"resource_spec_code\":\"GPSSD_for_smb\",\"spec_size\":40,\"spec_size_measure_id\":17,\"product_num\":1}},{\"subscribe_classify\":\"INDEPENDENT_RIGHTS_AND_INTERESTS\",\"subscribe_classify_param\":\"{\\\"period_type\\\":\\\"MONTH\\\",\\\"period_num\\\":1,\\\"region_code\\\":\\\"ap-southeast-3\\\"}\",\"offering_id\":\"OFFI989799820086366208\",\"offering_num\":1,\"product_info\":{\"service_type_code\":\"hws.service.type.vpc\",\"resource_type_code\":\"hws.resource.type.bandwidth\",\"resource_spec_code\":\"12_bgp_1024G_30M_HECS\",\"spec_size\":null,\"spec_size_measure_id\":null,\"product_num\":1}},{\"subscribe_classify\":\"INDEPENDENT_PERIODIC_CLOUD_SERVICE\",\"subscribe_classify_param\":\"{\\\"period_type\\\":\\\"MONTH\\\",\\\"period_num\\\":1,\\\"region_code\\\":\\\"ap-southeast-3\\\"}\",\"offering_id\":\"00301-262187-0--0\",\"offering_num\":1,\"product_info\":{\"service_type_code\":\"hws.service.type.ec2\",\"resource_type_code\":\"hws.resource.type.vm.image\",\"resource_spec_code\":\"image\",\"spec_size\":null,\"spec_size_measure_id\":null,\"product_num\":1}}]}}]}]}", null, "application/json");
request.Content = content;

response = await client.SendAsync(request);
response.EnsureSuccessStatusCode();
var resstr =await response.Content.ReadAsStringAsync();
Console.WriteLine(resstr);
Console.WriteLine();
Console.WriteLine();
if (resstr.IndexOf("\"result_code\":null,")!=-1)
{
    Console.WriteLine($"下单成功打开下面链接支付：https://account.huaweicloud.com/usercenter/?region=ap-southeast-3&locale=zh-cn#/ordercenter/userindex/unpaidOrderDetail?orderId={orderId}&extendType=BoOrder&entry=unpaidOrder");
}
else
{
    Console.WriteLine("下单错误");
}

 class Order
{
    public int OrderId { get; set; }
}