#pragma checksum "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\Home\Main_menu.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "07bbe1e51faf4d41bfd05c35d0568b7397d8ab2b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Main_menu), @"mvc.1.0.view", @"/Views/Home/Main_menu.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Main_menu.cshtml", typeof(AspNetCore.Views_Home_Main_menu))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\_ViewImports.cshtml"
using Assistant;

#line default
#line hidden
#line 2 "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\_ViewImports.cshtml"
using Assistant.Models;

#line default
#line hidden
#line 1 "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\Home\Main_menu.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"07bbe1e51faf4d41bfd05c35d0568b7397d8ab2b", @"/Views/Home/Main_menu.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fc8d0bc7d1d0e3ca7aafbc931c934509822bc9d6", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Main_menu : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 4 "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\Home\Main_menu.cshtml"
  
    ViewData["Title"] = "Main_menu";

#line default
#line hidden
            BeginContext(181, 187, true);
            WriteLiteral("\r\n\r\n<nav class=\"navbar\">\r\n\r\n    <h2> Asystent Zakupow </h2>\r\n\r\n</nav>\r\n<div class=\"col-sm-3 text-center\">\r\n\r\n    <div class=\"row mt-3\"></div>\r\n    <input type=\"button\" class=\"btn-primary\"");
            EndContext();
            BeginWriteAttribute("onclick", " onclick=\"", 368, "\"", 426, 3);
            WriteAttributeValue("", 378, "location.href=\'", 378, 15, true);
#line 17 "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\Home\Main_menu.cshtml"
WriteAttributeValue("", 393, Url.Action("List_name", "Home"), 393, 32, false);

#line default
#line hidden
            WriteAttributeValue("", 425, "\'", 425, 1, true);
            EndWriteAttribute();
            BeginContext(427, 148, true);
            WriteLiteral(" value=\"Stworz nowa listę\" style=\" margin-bottom:3px;width:200px\" />\r\n    <div class=\"row mt-3\"></div>\r\n    <input type=\"button\" class=\"btn-primary\"");
            EndContext();
            BeginWriteAttribute("onclick", " onclick=\"", 575, "\"", 639, 3);
            WriteAttributeValue("", 585, "location.href=\'", 585, 15, true);
#line 19 "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\Home\Main_menu.cshtml"
WriteAttributeValue("", 600, Url.Action("Select_list", "Generate"), 600, 38, false);

#line default
#line hidden
            WriteAttributeValue("", 638, "\'", 638, 1, true);
            EndWriteAttribute();
            BeginContext(640, 157, true);
            WriteLiteral(" value=\"Wygeneruj listę zakupów\" style=\" margin-bottom:3px; width:200px\" />\r\n\r\n    <div class=\"row mt-3\"></div>\r\n    <input type=\"button\" class=\"btn-primary\"");
            EndContext();
            BeginWriteAttribute("onclick", " onclick=\"", 797, "\"", 862, 3);
            WriteAttributeValue("", 807, "location.href=\'", 807, 15, true);
#line 22 "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\Home\Main_menu.cshtml"
WriteAttributeValue("", 822, Url.Action("Public_list_load", "Home"), 822, 39, false);

#line default
#line hidden
            WriteAttributeValue("", 861, "\'", 861, 1, true);
            EndWriteAttribute();
            BeginContext(863, 107, true);
            WriteLiteral(" value=\"Edytuj liste zakupów\" style=\"margin-bottom:3px; width:200px\" />\r\n    <div class=\"row mt-3\"></div>\r\n");
            EndContext();
#line 24 "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\Home\Main_menu.cshtml"
     if (User.Identity.IsAuthenticated)
    {

#line default
#line hidden
            BeginContext(1018, 48, true);
            WriteLiteral("        <input type=\"button\" class=\"btn-primary\"");
            EndContext();
            BeginWriteAttribute("onclick", " onclick=\"", 1066, "\"", 1132, 3);
            WriteAttributeValue("", 1076, "location.href=\'", 1076, 15, true);
#line 26 "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\Home\Main_menu.cshtml"
WriteAttributeValue("", 1091, Url.Action("Private_list_load", "Home"), 1091, 40, false);

#line default
#line hidden
            WriteAttributeValue("", 1131, "\'", 1131, 1, true);
            EndWriteAttribute();
            BeginContext(1133, 68, true);
            WriteLiteral(" value=\"Listy prywatne\" style=\" margin-bottom:3px; width:200px\" />\r\n");
            EndContext();
#line 27 "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\Home\Main_menu.cshtml"
    }

#line default
#line hidden
            BeginContext(1208, 12, true);
            WriteLiteral("</div>\r\n\r\n\r\n");
            EndContext();
            BeginContext(1221, 39, false);
#line 31 "C:\Users\Dominik\Documents\GitHub\inzynierka\login\Assistant\Assistant\Views\Home\Main_menu.cshtml"
Write(await Html.PartialAsync("FrequentList"));

#line default
#line hidden
            EndContext();
            BeginContext(1260, 4, true);
            WriteLiteral("\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public UserManager<IdentityUser> UserManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public SignInManager<IdentityUser> SignInManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
