# SimpleWidgetsLayoutScript
----
### 简介
本轮子可以通过简单的语法，配上自己的具体实现，可以快速而又简单的界面标记语言。


### 例子
在测试项目Test根目录，此项目已应用到我的另一个项目[SimpleRenderFramework](https://github.com/MikiraSora/SimpleRenderFramework),应用在[GameObjectWrapper](https://github.com/MikiraSora/SimpleRenderFramework/blob/redesign/SimpleRenderFramework_Ex/Utils/GameObjectWrapper.cs)类.

### 特殊功能
* 能简单的计算数值(计算器项目[BaseCalculator](https://github.com/MikiraSora/BaseCalculator))
[ GameObject:"My Tri"(x=0,y=0,width=16*16,height=256,backgroundImage="Assets/tri.png")]
会先计算width的值

* 能使用父对象/本对象的属性参与计算
[ GameObject:"Father Object"(x=0,y=0,width=256,height=256,backgroundImage="Assets/tri.png")]
{
	[ GameObject:"Child Object"(x=0,y=#x,angle=180+90,width=\$width/2,height=\$height/2,backgroundImage="Assets/tri.png")]
}
"\$"指父对象的属性，"#"指本对象的属性


![](http://i1.piimg.com/1949/fba4e6a3bdd66554.png)

#####若有疑问或者建议，请提交issue或者邮件