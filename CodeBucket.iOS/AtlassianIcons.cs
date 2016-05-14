using UIKit;

namespace CodeBucket
{
    public class AtlassianIcon
    {
        public char CharacterCode { get; private set; }

        public AtlassianIcon(char characterCode)
        {
            CharacterCode = characterCode;
        }

        public static implicit operator AtlassianIcon(char d)
        {
            return new AtlassianIcon(d);
        }

        public static implicit operator UIImage(AtlassianIcon icon)
        {
            return icon.ToImage();
        }

        public static AtlassianIcon Add = (char)0xf101;


        public static AtlassianIcon Addcomment = (char)0xf102;


        public static AtlassianIcon Addsmall = (char)0xf103;


        public static AtlassianIcon Approve = (char)0xf104;


        public static AtlassianIcon Appswitcher = (char)0xf105;


        public static AtlassianIcon Arrowsdown = (char)0xf106;


        public static AtlassianIcon Arrowsleft = (char)0xf107;


        public static AtlassianIcon Arrowsright = (char)0xf108;


        public static AtlassianIcon Arrowsup = (char)0xf109;


        public static AtlassianIcon Attachment = (char)0xf10a;


        public static AtlassianIcon Attachmentsmall = (char)0xf10b;


        public static AtlassianIcon Autocompletedate = (char)0xf10c;


        public static AtlassianIcon Backpage = (char)0xf10d;


        public static AtlassianIcon Blogroll = (char)0xf10e;


        public static AtlassianIcon Bpdecisions = (char)0xf10f;


        public static AtlassianIcon Bpdefault = (char)0xf110;


        public static AtlassianIcon Bpfiles = (char)0xf111;


        public static AtlassianIcon Bprequirements = (char)0xf112;


        public static AtlassianIcon Bphowto = (char)0xf113;


        public static AtlassianIcon Bpjira = (char)0xf114;


        public static AtlassianIcon Bpmeeting = (char)0xf115;


        public static AtlassianIcon Bpretrospective = (char)0xf116;


        public static AtlassianIcon Bpsharedlinks = (char)0xf117;


        public static AtlassianIcon Bptroubleshooting = (char)0xf118;


        public static AtlassianIcon Build = (char)0xf119;


        public static AtlassianIcon Calendar = (char)0xf11a;


        public static AtlassianIcon Closedialog = (char)0xf11b;


        public static AtlassianIcon Collapsed = (char)0xf11c;


        public static AtlassianIcon Comment = (char)0xf11d;


        public static AtlassianIcon Configure = (char)0xf11e;


        public static AtlassianIcon Confluence = (char)0xf11f;


        public static AtlassianIcon Copyclipboard = (char)0xf120;


        public static AtlassianIcon Custombullet = (char)0xf121;


        public static AtlassianIcon Delete = (char)0xf122;


        public static AtlassianIcon Deploy = (char)0xf123;


        public static AtlassianIcon Details = (char)0xf124;


        public static AtlassianIcon Devtoolsarrowleft = (char)0xf125;


        public static AtlassianIcon Devtoolsarrowright = (char)0xf126;


        public static AtlassianIcon Devtoolsbranch = (char)0xf127;


        public static AtlassianIcon Devtoolsbranchsmall = (char)0xf128;


        public static AtlassianIcon Devtoolsbrowseup = (char)0xf129;


        public static AtlassianIcon Devtoolscheckout = (char)0xf12a;


        public static AtlassianIcon Devtoolsclone = (char)0xf12b;


        public static AtlassianIcon Devtoolscommit = (char)0xf12c;


        public static AtlassianIcon Devtoolscompare = (char)0xf12d;


        public static AtlassianIcon Devtoolsfile = (char)0xf12e;


        public static AtlassianIcon Devtoolsfilebinary = (char)0xf12f;


        public static AtlassianIcon Devtoolsfilecommented = (char)0xf130;


        public static AtlassianIcon Devtoolsfolderclosed = (char)0xf131;


        public static AtlassianIcon Devtoolsfolderopen = (char)0xf132;


        public static AtlassianIcon Devtoolsfork = (char)0xf133;


        public static AtlassianIcon Devtoolspullrequest = (char)0xf134;


        public static AtlassianIcon Devtoolsrepository = (char)0xf135;


        public static AtlassianIcon Devtoolsrepositoryforked = (char)0xf136;


        public static AtlassianIcon Devtoolsrepositorylocked = (char)0xf137;


        public static AtlassianIcon Devtoolssidediff = (char)0xf138;


        public static AtlassianIcon Devtoolssubmodule = (char)0xf139;


        public static AtlassianIcon Devtoolstag = (char)0xf13a;


        public static AtlassianIcon Devtoolstagsmall = (char)0xf13b;


        public static AtlassianIcon Devtoolstaskcancelled = (char)0xf13c;


        public static AtlassianIcon Devtoolstaskdisabled = (char)0xf13d;


        public static AtlassianIcon Devtoolstaskinprogress = (char)0xf13e;


        public static AtlassianIcon Doc = (char)0xf13f;


        public static AtlassianIcon Down = (char)0xf140;


        public static AtlassianIcon Dragvertical = (char)0xf141;


        public static AtlassianIcon Edit = (char)0xf142;


        public static AtlassianIcon Editsmall = (char)0xf143;


        public static AtlassianIcon Editoraligncenter = (char)0xf144;


        public static AtlassianIcon Editoralignleft = (char)0xf145;
        public static AtlassianIcon Editoralignright = (char)0xf146;
        public static AtlassianIcon Editorbold = (char)0xf147;
        public static AtlassianIcon Editorcolor = (char)0xf148;
        public static AtlassianIcon Editoremoticon = (char)0xf149;
        public static AtlassianIcon Editorhelp = (char)0xf14a;
        public static AtlassianIcon Editorhr = (char)0xf14b;
        public static AtlassianIcon Editorindent = (char)0xf14c;
        public static AtlassianIcon Editoritalic = (char)0xf14d;
        public static AtlassianIcon Editorlayout = (char)0xf14e;
        public static AtlassianIcon Editorlistbullet = (char)0xf14f;
        public static AtlassianIcon Editorlistnumber = (char)0xf150;
        public static AtlassianIcon Editormacrotoc = (char)0xf151;
        public static AtlassianIcon Editormention = (char)0xf152;
        public static AtlassianIcon Editoroutdent = (char)0xf153;
        public static AtlassianIcon Editorstyles = (char)0xf154;
        public static AtlassianIcon Editorsymbol = (char)0xf155;
        public static AtlassianIcon Editortable = (char)0xf156;
        public static AtlassianIcon Editortask = (char)0xf157;
        public static AtlassianIcon Editorunderline = (char)0xf158;
        public static AtlassianIcon Email = (char)0xf159;
        public static AtlassianIcon Error = (char)0xf15a;
        public static AtlassianIcon Expanded = (char)0xf15b;
        public static AtlassianIcon Filecode = (char)0xf15c;
        public static AtlassianIcon Filedoc = (char)0xf15d;
        public static AtlassianIcon Filejava = (char)0xf15e;
        public static AtlassianIcon Filepdf = (char)0xf15f;
        public static AtlassianIcon Fileppt = (char)0xf160;
        public static AtlassianIcon Filetxt = (char)0xf161;
        public static AtlassianIcon Filewav = (char)0xf162;
        public static AtlassianIcon Filexls = (char)0xf163;
        public static AtlassianIcon Filezip = (char)0xf164;
        public static AtlassianIcon Flag = (char)0xf165;
        public static AtlassianIcon Focus = (char)0xf166;
        public static AtlassianIcon Group = (char)0xf167;
        public static AtlassianIcon Handlehorizontal = (char)0xf168;
        public static AtlassianIcon Help = (char)0xf169;
        public static AtlassianIcon Hipchat = (char)0xf16a;
        public static AtlassianIcon Homepage = (char)0xf16b;
        public static AtlassianIcon Image = (char)0xf16c;
        public static AtlassianIcon Imageextrasmall = (char)0xf16d;
        public static AtlassianIcon Imagesmall = (char)0xf16e;
        public static AtlassianIcon Info = (char)0xf16f;
        public static AtlassianIcon Jira = (char)0xf170;
        public static AtlassianIcon Jiracompletedtask = (char)0xf171;
        public static AtlassianIcon Jiratestsession = (char)0xf172;
        public static AtlassianIcon Like = (char)0xf173;
        public static AtlassianIcon Likesmall = (char)0xf174;
        public static AtlassianIcon Weblink = (char)0xf175;
        public static AtlassianIcon Link = (char)0xf176;
        public static AtlassianIcon ListAdd = (char)0xf177;


        public static AtlassianIcon ListRemove = (char)0xf178;


        public static AtlassianIcon Locked = (char)0xf179;


        public static AtlassianIcon Lockedsmall = (char)0xf17a;


        public static AtlassianIcon Macrocode = (char)0xf17b;


        public static AtlassianIcon Macrodefault = (char)0xf17c;


        public static AtlassianIcon Macrogallery = (char)0xf17d;


        public static AtlassianIcon Macrostatus = (char)0xf17e;


        public static AtlassianIcon More = (char)0xf17f;


        public static AtlassianIcon Navchildren = (char)0xf180;


        public static AtlassianIcon Pageblank = (char)0xf181;


        public static AtlassianIcon Pageblogpost = (char)0xf182;


        public static AtlassianIcon PageDefault = (char)0xf183;


        public static AtlassianIcon Pagetemplate = (char)0xf184;


        public static AtlassianIcon Pages = (char)0xf185;


        public static AtlassianIcon quote = (char)0xf186;


        public static AtlassianIcon Redo = (char)0xf187;


        public static AtlassianIcon Remove = (char)0xf188;


        public static AtlassianIcon Removelabel = (char)0xf189;


        public static AtlassianIcon Review = (char)0xf18a;


        public static AtlassianIcon Rss = (char)0xf18b;


        public static AtlassianIcon Search = (char)0xf18c;


        public static AtlassianIcon Searchsmall = (char)0xf18d;


        public static AtlassianIcon Share = (char)0xf18e;


        public static AtlassianIcon Sidebarlink = (char)0xf18f;


        public static AtlassianIcon Sourcetree = (char)0xf190;


        public static AtlassianIcon Spacedefault = (char)0xf191;


        public static AtlassianIcon Spacepersonal = (char)0xf192;


        public static AtlassianIcon Star = (char)0xf193;


        public static AtlassianIcon Success = (char)0xf194;


        public static AtlassianIcon Tablebg = (char)0xf195;


        public static AtlassianIcon Tablecolleft = (char)0xf196;


        public static AtlassianIcon Tablecolremove = (char)0xf197;


        public static AtlassianIcon Tablecolright = (char)0xf198;


        public static AtlassianIcon Tablecopyrow = (char)0xf199;


        public static AtlassianIcon Tablecutrow = (char)0xf19a;


        public static AtlassianIcon Tableheadercolumn = (char)0xf19b;


        public static AtlassianIcon Tableheaderrow = (char)0xf19c;


        public static AtlassianIcon Tablemerge = (char)0xf19d;


        public static AtlassianIcon Tablenobg = (char)0xf19e;


        public static AtlassianIcon Tablepasterow = (char)0xf19f;


        public static AtlassianIcon Tableremove = (char)0xf1a0;


        public static AtlassianIcon Tablerowdown = (char)0xf1a1;


        public static AtlassianIcon Tablerowremove = (char)0xf1a2;


        public static AtlassianIcon Tablerowup = (char)0xf1a3;


        public static AtlassianIcon Tablesplit = (char)0xf1a4;


        public static AtlassianIcon Teamcals = (char)0xf1a5;


        public static AtlassianIcon Time = (char)0xf1a6;


        public static AtlassianIcon Undo = (char)0xf1a7;


        public static AtlassianIcon Unfocus = (char)0xf1a8;


        public static AtlassianIcon Unlocked = (char)0xf1a9;


        public static AtlassianIcon Unstar = (char)0xf1aa;


        public static AtlassianIcon Unwatch = (char)0xf1ab;


        public static AtlassianIcon Up = (char)0xf1ac;


        public static AtlassianIcon User = (char)0xf1ad;


        public static AtlassianIcon Userstatus = (char)0xf1ae;


        public static AtlassianIcon View = (char)0xf1af;


        public static AtlassianIcon Viewcard = (char)0xf1b0;


        public static AtlassianIcon Viewlist = (char)0xf1b1;


        public static AtlassianIcon Viewtable = (char)0xf1b2;


        public static AtlassianIcon Warning = (char)0xf1b3;


        public static AtlassianIcon Watch = (char)0xf1b4;


        public static AtlassianIcon Workbox = (char)0xf1b5;


        public static AtlassianIcon Workboxempty = (char)0xf1b6;


        public static AtlassianIcon Configurecolumns = (char)0xf1b7;


        public static AtlassianIcon Export = (char)0xf1b8;


        public static AtlassianIcon Exportlist = (char)0xf1b9;


        public static AtlassianIcon Fileimage = (char)0xf1ba;


        public static AtlassianIcon Adminfusion = (char)0xf1bb;


        public static AtlassianIcon Adminjirafields = (char)0xf1bc;


        public static AtlassianIcon Adminissue = (char)0xf1bd;


        public static AtlassianIcon Adminnotifications = (char)0xf1be;


        public static AtlassianIcon Adminroles = (char)0xf1bf;


        public static AtlassianIcon Adminjirascreens = (char)0xf1c0;


        public static AtlassianIcon Pause = (char)0xf1c1;


        public static AtlassianIcon Priorityhighest = (char)0xf1c2;


        public static AtlassianIcon Priorityhigh = (char)0xf1c3;


        public static AtlassianIcon Prioritymedium = (char)0xf1c4;


        public static AtlassianIcon Prioritylow = (char)0xf1c5;


        public static AtlassianIcon Prioritylowest = (char)0xf1c6;


        public static AtlassianIcon Refreshsmall = (char)0xf1c7;


        public static AtlassianIcon Sharelist = (char)0xf1c8;


        public static AtlassianIcon Switchsmall = (char)0xf1c9;


        public static AtlassianIcon Version = (char)0xf1ca;


        public static AtlassianIcon Workflow = (char)0xf1cb;


        public static AtlassianIcon Adminjirasettings = (char)0xf1cc;


        public static AtlassianIcon Component = (char)0xf1cd;


        public static AtlassianIcon Reopen = (char)0xf1ce;


        public static AtlassianIcon Roadmap = (char)0xf1cf;


        public static AtlassianIcon Deploysuccess = (char)0xf1d0;


        public static AtlassianIcon Deployfail = (char)0xf1d1;


        public static AtlassianIcon Filegeneric = (char)0xf1d2;


        public static AtlassianIcon Arrowdown = (char)0xf1d3;


        public static AtlassianIcon Arrowup = (char)0xf1d4;


        public static AtlassianIcon Blogrolllarge = (char)0xf1d5;


        public static AtlassianIcon Emaillarge = (char)0xf1d6;


        public static AtlassianIcon Layout1collarge = (char)0xf1d7;


        public static AtlassianIcon Layout2collarge = (char)0xf1d8;


        public static AtlassianIcon Layout2colleftlarge = (char)0xf1d9;


        public static AtlassianIcon Layout2colrightlarge = (char)0xf1da;


        public static AtlassianIcon Layout3colcenterlarge = (char)0xf1db;


        public static AtlassianIcon Layout3collarge = (char)0xf1dc;


        public static AtlassianIcon Navchildrenlarge = (char)0xf1dd;


        public static AtlassianIcon Pageslarge = (char)0xf1de;


        public static AtlassianIcon Sidebarlinklarge = (char)0xf1df;


        public static AtlassianIcon Teamcalslarge = (char)0xf1e0;


        public static AtlassianIcon Userlarge = (char)0xf1e1;

    }
}

