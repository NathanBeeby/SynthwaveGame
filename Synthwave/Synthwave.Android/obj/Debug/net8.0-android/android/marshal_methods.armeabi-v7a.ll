; ModuleID = 'marshal_methods.armeabi-v7a.ll'
source_filename = "marshal_methods.armeabi-v7a.ll"
target datalayout = "e-m:e-p:32:32-Fi8-i64:64-v128:64:128-a:0:32-n32-S64"
target triple = "armv7-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [189 x ptr] zeroinitializer, align 4

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [372 x i32] [
	i32 2616222, ; 0: System.Net.NetworkInformation.dll => 0x27eb9e => 70
	i32 10166715, ; 1: System.Net.NameResolution.dll => 0x9b21bb => 69
	i32 15721112, ; 2: System.Runtime.Intrinsics.dll => 0xefe298 => 110
	i32 18015263, ; 3: Synthwave.Core => 0x112e41f => 184
	i32 34839235, ; 4: System.IO.FileSystem.DriveInfo => 0x2139ac3 => 50
	i32 39485524, ; 5: System.Net.WebSockets.dll => 0x25a8054 => 82
	i32 42639949, ; 6: System.Threading.Thread => 0x28aa24d => 147
	i32 65960268, ; 7: Microsoft.Win32.SystemEvents.dll => 0x3ee794c => 175
	i32 66541672, ; 8: System.Diagnostics.StackTrace => 0x3f75868 => 32
	i32 68219467, ; 9: System.Security.Cryptography.Primitives => 0x410f24b => 126
	i32 82292897, ; 10: System.Runtime.CompilerServices.VisualC.dll => 0x4e7b0a1 => 104
	i32 117431740, ; 11: System.Runtime.InteropServices => 0x6ffddbc => 109
	i32 122350210, ; 12: System.Threading.Channels.dll => 0x74aea82 => 141
	i32 142721839, ; 13: System.Net.WebHeaderCollection => 0x881c32f => 79
	i32 149972175, ; 14: System.Security.Cryptography.Primitives.dll => 0x8f064cf => 126
	i32 159306688, ; 15: System.ComponentModel.Annotations => 0x97ed3c0 => 15
	i32 176265551, ; 16: System.ServiceProcess => 0xa81994f => 134
	i32 184328833, ; 17: System.ValueTuple.dll => 0xafca281 => 153
	i32 205061960, ; 18: System.ComponentModel => 0xc38ff48 => 20
	i32 210846173, ; 19: fr-FR/Synthwave.Core.resources.dll => 0xc9141dd => 1
	i32 220171995, ; 20: System.Diagnostics.Debug => 0xd1f8edb => 28
	i32 230752869, ; 21: Microsoft.CSharp.dll => 0xdc10265 => 3
	i32 231409092, ; 22: System.Linq.Parallel => 0xdcb05c4 => 61
	i32 231814094, ; 23: System.Globalization => 0xdd133ce => 44
	i32 246610117, ; 24: System.Reflection.Emit.Lightweight => 0xeb2f8c5 => 93
	i32 276479776, ; 25: System.Threading.Timer.dll => 0x107abf20 => 149
	i32 291076382, ; 26: System.IO.Pipes.AccessControl.dll => 0x1159791e => 56
	i32 298918909, ; 27: System.Net.Ping.dll => 0x11d123fd => 71
	i32 321597661, ; 28: System.Numerics => 0x132b30dd => 85
	i32 360082299, ; 29: System.ServiceModel.Web => 0x15766b7b => 133
	i32 367780167, ; 30: System.IO.Pipes => 0x15ebe147 => 57
	i32 374914964, ; 31: System.Transactions.Local => 0x1658bf94 => 151
	i32 375677976, ; 32: System.Net.ServicePoint.dll => 0x16646418 => 76
	i32 379916513, ; 33: System.Threading.Thread.dll => 0x16a510e1 => 147
	i32 385762202, ; 34: System.Memory.dll => 0x16fe439a => 64
	i32 392610295, ; 35: System.Threading.ThreadPool.dll => 0x1766c1f7 => 148
	i32 395744057, ; 36: _Microsoft.Android.Resource.Designer => 0x17969339 => 185
	i32 403441872, ; 37: WindowsBase => 0x180c08d0 => 167
	i32 442565967, ; 38: System.Collections => 0x1a61054f => 14
	i32 451504562, ; 39: System.Security.Cryptography.X509Certificates => 0x1ae969b2 => 127
	i32 456227837, ; 40: System.Web.HttpUtility.dll => 0x1b317bfd => 154
	i32 459347974, ; 41: System.Runtime.Serialization.Primitives.dll => 0x1b611806 => 115
	i32 459926847, ; 42: Synthwave.Core.dll => 0x1b69ed3f => 184
	i32 465846621, ; 43: mscorlib => 0x1bc4415d => 168
	i32 469710990, ; 44: System.dll => 0x1bff388e => 166
	i32 498788369, ; 45: System.ObjectModel => 0x1dbae811 => 86
	i32 507640256, ; 46: MonoGame.Framework => 0x1e41f9c0 => 176
	i32 526420162, ; 47: System.Transactions.dll => 0x1f6088c2 => 152
	i32 530272170, ; 48: System.Linq.Queryable => 0x1f9b4faa => 62
	i32 540030774, ; 49: System.IO.FileSystem.dll => 0x20303736 => 53
	i32 545304856, ; 50: System.Runtime.Extensions => 0x2080b118 => 105
	i32 546455878, ; 51: System.Runtime.Serialization.Xml => 0x20924146 => 116
	i32 549171840, ; 52: System.Globalization.Calendars => 0x20bbb280 => 42
	i32 565461376, ; 53: Synthwave => 0x21b44180 => 2
	i32 577335427, ; 54: System.Security.Cryptography.Cng => 0x22697083 => 122
	i32 601371474, ; 55: System.IO.IsolatedStorage.dll => 0x23d83352 => 54
	i32 605376203, ; 56: System.IO.Compression.FileSystem => 0x24154ecb => 46
	i32 613668793, ; 57: System.Security.Cryptography.Algorithms => 0x2493d7b9 => 121
	i32 643868501, ; 58: System.Net => 0x2660a755 => 83
	i32 662205335, ; 59: System.Text.Encodings.Web.dll => 0x27787397 => 138
	i32 672442732, ; 60: System.Collections.Concurrent => 0x2814a96c => 10
	i32 683518922, ; 61: System.Net.Security => 0x28bdabca => 75
	i32 690569205, ; 62: System.Xml.Linq.dll => 0x29293ff5 => 157
	i32 693804605, ; 63: System.Windows => 0x295a9e3d => 156
	i32 699345723, ; 64: System.Reflection.Emit => 0x29af2b3b => 94
	i32 700358131, ; 65: System.IO.Compression.ZipFile => 0x29be9df3 => 47
	i32 722857257, ; 66: System.Runtime.Loader.dll => 0x2b15ed29 => 111
	i32 735137430, ; 67: System.Security.SecureString.dll => 0x2bd14e96 => 131
	i32 752232764, ; 68: System.Diagnostics.Contracts.dll => 0x2cd6293c => 27
	i32 759454413, ; 69: System.Net.Requests => 0x2d445acd => 74
	i32 762598435, ; 70: System.IO.Pipes.dll => 0x2d745423 => 57
	i32 775507847, ; 71: System.IO.Compression => 0x2e394f87 => 48
	i32 804715423, ; 72: System.Data.Common => 0x2ff6fb9f => 24
	i32 809851609, ; 73: System.Drawing.Common.dll => 0x30455ad9 => 177
	i32 823281589, ; 74: System.Private.Uri.dll => 0x311247b5 => 88
	i32 830298997, ; 75: System.IO.Compression.Brotli => 0x317d5b75 => 45
	i32 832635846, ; 76: System.Xml.XPath.dll => 0x31a103c6 => 162
	i32 834051424, ; 77: System.Net.Quic => 0x31b69d60 => 73
	i32 847111869, ; 78: es-ES/Synthwave.Core.resources.dll => 0x327de6bd => 0
	i32 873119928, ; 79: Microsoft.VisualBasic => 0x340ac0b8 => 5
	i32 873207192, ; 80: System.Private.Windows.Core => 0x340c1598 => 178
	i32 877678880, ; 81: System.Globalization.dll => 0x34505120 => 44
	i32 878954865, ; 82: System.Net.Http.Json => 0x3463c971 => 65
	i32 891529482, ; 83: Synthwave.dll => 0x3523a90a => 2
	i32 904024072, ; 84: System.ComponentModel.Primitives.dll => 0x35e25008 => 18
	i32 911108515, ; 85: System.IO.MemoryMappedFiles.dll => 0x364e69a3 => 55
	i32 952186615, ; 86: System.Runtime.InteropServices.JavaScript.dll => 0x38c136f7 => 107
	i32 975236339, ; 87: System.Diagnostics.Tracing => 0x3a20ecf3 => 36
	i32 975874589, ; 88: System.Xml.XDocument => 0x3a2aaa1d => 160
	i32 986514023, ; 89: System.Private.DataContractSerialization.dll => 0x3acd0267 => 87
	i32 987214855, ; 90: System.Diagnostics.Tools => 0x3ad7b407 => 34
	i32 992768348, ; 91: System.Collections.dll => 0x3b2c715c => 14
	i32 994442037, ; 92: System.IO.FileSystem => 0x3b45fb35 => 53
	i32 1001831731, ; 93: System.IO.UnmanagedMemoryStream.dll => 0x3bb6bd33 => 58
	i32 1019214401, ; 94: System.Drawing => 0x3cbffa41 => 38
	i32 1036536393, ; 95: System.Drawing.Primitives.dll => 0x3dc84a49 => 37
	i32 1044663988, ; 96: System.Linq.Expressions.dll => 0x3e444eb4 => 60
	i32 1082857460, ; 97: System.ComponentModel.TypeConverter => 0x408b17f4 => 19
	i32 1098259244, ; 98: System => 0x41761b2c => 166
	i32 1133267265, ; 99: fr-FR\Synthwave.Core.resources => 0x438c4941 => 1
	i32 1170634674, ; 100: System.Web.dll => 0x45c677b2 => 155
	i32 1208641965, ; 101: System.Diagnostics.Process => 0x480a69ad => 31
	i32 1219128291, ; 102: System.IO.IsolatedStorage => 0x48aa6be3 => 54
	i32 1253011324, ; 103: Microsoft.Win32.Registry => 0x4aaf6f7c => 7
	i32 1309188875, ; 104: System.Private.DataContractSerialization => 0x4e08a30b => 87
	i32 1324164729, ; 105: System.Linq => 0x4eed2679 => 63
	i32 1335329327, ; 106: System.Runtime.Serialization.Json.dll => 0x4f97822f => 114
	i32 1364015309, ; 107: System.IO => 0x514d38cd => 59
	i32 1377478080, ; 108: es-ES\Synthwave.Core.resources => 0x521aa5c0 => 0
	i32 1379779777, ; 109: System.Resources.ResourceManager => 0x523dc4c1 => 101
	i32 1380458928, ; 110: System.Private.Windows.GdiPlus.dll => 0x524821b0 => 179
	i32 1402170036, ; 111: System.Configuration.dll => 0x53936ab4 => 21
	i32 1408764838, ; 112: System.Runtime.Serialization.Formatters.dll => 0x53f80ba6 => 113
	i32 1411638395, ; 113: System.Runtime.CompilerServices.Unsafe => 0x5423e47b => 103
	i32 1422545099, ; 114: System.Runtime.CompilerServices.VisualC => 0x54ca50cb => 104
	i32 1434145427, ; 115: System.Runtime.Handles => 0x557b5293 => 106
	i32 1439761251, ; 116: System.Net.Quic.dll => 0x55d10363 => 73
	i32 1452070440, ; 117: System.Formats.Asn1.dll => 0x568cd628 => 40
	i32 1453312822, ; 118: System.Diagnostics.Tools.dll => 0x569fcb36 => 34
	i32 1457743152, ; 119: System.Runtime.Extensions.dll => 0x56e36530 => 105
	i32 1458022317, ; 120: System.Net.Security.dll => 0x56e7a7ad => 75
	i32 1461234159, ; 121: System.Collections.Immutable.dll => 0x5718a9ef => 11
	i32 1461719063, ; 122: System.Security.Cryptography.OpenSsl => 0x57201017 => 125
	i32 1462112819, ; 123: System.IO.Compression.dll => 0x57261233 => 48
	i32 1479771757, ; 124: System.Collections.Immutable => 0x5833866d => 11
	i32 1480492111, ; 125: System.IO.Compression.Brotli.dll => 0x583e844f => 45
	i32 1487239319, ; 126: Microsoft.Win32.Primitives => 0x58a57897 => 6
	i32 1536373174, ; 127: System.Diagnostics.TextWriterTraceListener => 0x5b9331b6 => 33
	i32 1543031311, ; 128: System.Text.RegularExpressions.dll => 0x5bf8ca0f => 140
	i32 1543355203, ; 129: System.Reflection.Emit.dll => 0x5bfdbb43 => 94
	i32 1550322496, ; 130: System.Reflection.Extensions.dll => 0x5c680b40 => 95
	i32 1565862583, ; 131: System.IO.FileSystem.Primitives => 0x5d552ab7 => 51
	i32 1566207040, ; 132: System.Threading.Tasks.Dataflow.dll => 0x5d5a6c40 => 143
	i32 1573704789, ; 133: System.Runtime.Serialization.Json => 0x5dccd455 => 114
	i32 1580037396, ; 134: System.Threading.Overlapped => 0x5e2d7514 => 142
	i32 1592978981, ; 135: System.Runtime.Serialization.dll => 0x5ef2ee25 => 117
	i32 1601112923, ; 136: System.Xml.Serialization => 0x5f6f0b5b => 159
	i32 1603525486, ; 137: Microsoft.Maui.Controls.HotReload.Forms.dll => 0x5f93db6e => 180
	i32 1604827217, ; 138: System.Net.WebClient => 0x5fa7b851 => 78
	i32 1618516317, ; 139: System.Net.WebSockets.Client.dll => 0x6078995d => 81
	i32 1622358360, ; 140: System.Dynamic.Runtime => 0x60b33958 => 39
	i32 1639515021, ; 141: System.Net.Http.dll => 0x61b9038d => 66
	i32 1639986890, ; 142: System.Text.RegularExpressions => 0x61c036ca => 140
	i32 1641389582, ; 143: System.ComponentModel.EventBasedAsync.dll => 0x61d59e0e => 17
	i32 1654293721, ; 144: System.Private.Windows.Core.dll => 0x629a84d9 => 178
	i32 1657153582, ; 145: System.Runtime => 0x62c6282e => 118
	i32 1675553242, ; 146: System.IO.FileSystem.DriveInfo.dll => 0x63dee9da => 50
	i32 1677501392, ; 147: System.Net.Primitives.dll => 0x63fca3d0 => 72
	i32 1678508291, ; 148: System.Net.WebSockets => 0x640c0103 => 82
	i32 1679769178, ; 149: System.Security.Cryptography => 0x641f3e5a => 128
	i32 1691477237, ; 150: System.Reflection.Metadata => 0x64d1e4f5 => 96
	i32 1696967625, ; 151: System.Security.Cryptography.Csp => 0x6525abc9 => 123
	i32 1701541528, ; 152: System.Diagnostics.Debug.dll => 0x656b7698 => 28
	i32 1726116996, ; 153: System.Reflection.dll => 0x66e27484 => 99
	i32 1728033016, ; 154: System.Diagnostics.FileVersionInfo.dll => 0x66ffb0f8 => 30
	i32 1744735666, ; 155: System.Transactions.Local.dll => 0x67fe8db2 => 151
	i32 1746316138, ; 156: Mono.Android.Export => 0x6816ab6a => 171
	i32 1750313021, ; 157: Microsoft.Win32.Primitives.dll => 0x6853a83d => 6
	i32 1758240030, ; 158: System.Resources.Reader.dll => 0x68cc9d1e => 100
	i32 1763938596, ; 159: System.Diagnostics.TraceSource.dll => 0x69239124 => 35
	i32 1765942094, ; 160: System.Reflection.Extensions => 0x6942234e => 95
	i32 1776026572, ; 161: System.Core.dll => 0x69dc03cc => 23
	i32 1777075843, ; 162: System.Globalization.Extensions.dll => 0x69ec0683 => 43
	i32 1780572499, ; 163: Mono.Android.Runtime.dll => 0x6a216153 => 172
	i32 1818787751, ; 164: Microsoft.VisualBasic.Core => 0x6c687fa7 => 4
	i32 1824175904, ; 165: System.Text.Encoding.Extensions => 0x6cbab720 => 136
	i32 1824722060, ; 166: System.Runtime.Serialization.Formatters => 0x6cc30c8c => 113
	i32 1827303595, ; 167: Microsoft.VisualStudio.DesignTools.TapContract => 0x6cea70ab => 182
	i32 1858542181, ; 168: System.Linq.Expressions => 0x6ec71a65 => 60
	i32 1870277092, ; 169: System.Reflection.Primitives => 0x6f7a29e4 => 97
	i32 1879696579, ; 170: System.Formats.Tar.dll => 0x7009e4c3 => 41
	i32 1885918049, ; 171: Microsoft.VisualStudio.DesignTools.TapContract.dll => 0x7068d361 => 182
	i32 1888955245, ; 172: System.Diagnostics.Contracts => 0x70972b6d => 27
	i32 1889954781, ; 173: System.Reflection.Metadata.dll => 0x70a66bdd => 96
	i32 1898237753, ; 174: System.Reflection.DispatchProxy => 0x7124cf39 => 91
	i32 1900610850, ; 175: System.Resources.ResourceManager.dll => 0x71490522 => 101
	i32 1910275211, ; 176: System.Collections.NonGeneric.dll => 0x71dc7c8b => 12
	i32 1939592360, ; 177: System.Private.Xml.Linq => 0x739bd4a8 => 89
	i32 1956758971, ; 178: System.Resources.Writer => 0x74a1c5bb => 102
	i32 2011961780, ; 179: System.Buffers.dll => 0x77ec19b4 => 9
	i32 2045470958, ; 180: System.Private.Xml => 0x79eb68ee => 90
	i32 2060060697, ; 181: System.Windows.dll => 0x7aca0819 => 156
	i32 2070888862, ; 182: System.Diagnostics.TraceSource => 0x7b6f419e => 35
	i32 2079903147, ; 183: System.Runtime.dll => 0x7bf8cdab => 118
	i32 2090596640, ; 184: System.Numerics.Vectors => 0x7c9bf920 => 84
	i32 2117912485, ; 185: Microsoft.VisualStudio.DesignTools.XamlTapContract.dll => 0x7e3cc7a5 => 183
	i32 2127167465, ; 186: System.Console => 0x7ec9ffe9 => 22
	i32 2142473426, ; 187: System.Collections.Specialized => 0x7fb38cd2 => 13
	i32 2143790110, ; 188: System.Xml.XmlSerializer.dll => 0x7fc7a41e => 164
	i32 2146852085, ; 189: Microsoft.VisualBasic.dll => 0x7ff65cf5 => 5
	i32 2193016926, ; 190: System.ObjectModel.dll => 0x82b6c85e => 86
	i32 2201231467, ; 191: System.Net.Http => 0x8334206b => 66
	i32 2222056684, ; 192: System.Threading.Tasks.Parallel => 0x8471e4ec => 145
	i32 2252106437, ; 193: System.Xml.Serialization.dll => 0x863c6ac5 => 159
	i32 2256313426, ; 194: System.Globalization.Extensions => 0x867c9c52 => 43
	i32 2265110946, ; 195: System.Security.AccessControl.dll => 0x8702d9a2 => 119
	i32 2293034957, ; 196: System.ServiceModel.Web.dll => 0x88acefcd => 133
	i32 2295906218, ; 197: System.Net.Sockets => 0x88d8bfaa => 77
	i32 2298471582, ; 198: System.Net.Mail => 0x88ffe49e => 68
	i32 2305521784, ; 199: System.Private.CoreLib.dll => 0x896b7878 => 174
	i32 2320631194, ; 200: System.Threading.Tasks.Parallel.dll => 0x8a52059a => 145
	i32 2340441535, ; 201: System.Runtime.InteropServices.RuntimeInformation.dll => 0x8b804dbf => 108
	i32 2344264397, ; 202: System.ValueTuple => 0x8bbaa2cd => 153
	i32 2353062107, ; 203: System.Net.Primitives => 0x8c40e0db => 72
	i32 2368005991, ; 204: System.Xml.ReaderWriter.dll => 0x8d24e767 => 158
	i32 2378619854, ; 205: System.Security.Cryptography.Csp.dll => 0x8dc6dbce => 123
	i32 2383496789, ; 206: System.Security.Principal.Windows.dll => 0x8e114655 => 129
	i32 2401565422, ; 207: System.Web.HttpUtility => 0x8f24faee => 154
	i32 2409983638, ; 208: Microsoft.VisualStudio.DesignTools.MobileTapContracts.dll => 0x8fa56e96 => 181
	i32 2421380589, ; 209: System.Threading.Tasks.Dataflow => 0x905355ed => 143
	i32 2435356389, ; 210: System.Console.dll => 0x912896e5 => 22
	i32 2435904999, ; 211: System.ComponentModel.DataAnnotations.dll => 0x9130f5e7 => 16
	i32 2454642406, ; 212: System.Text.Encoding.dll => 0x924edee6 => 137
	i32 2458678730, ; 213: System.Net.Sockets.dll => 0x928c75ca => 77
	i32 2459001652, ; 214: System.Linq.Parallel.dll => 0x92916334 => 61
	i32 2471841756, ; 215: netstandard.dll => 0x93554fdc => 169
	i32 2475788418, ; 216: Java.Interop.dll => 0x93918882 => 170
	i32 2483903535, ; 217: System.ComponentModel.EventBasedAsync => 0x940d5c2f => 17
	i32 2484371297, ; 218: System.Net.ServicePoint => 0x94147f61 => 76
	i32 2490993605, ; 219: System.AppContext.dll => 0x94798bc5 => 8
	i32 2501346920, ; 220: System.Data.DataSetExtensions => 0x95178668 => 25
	i32 2538310050, ; 221: System.Reflection.Emit.Lightweight.dll => 0x974b89a2 => 93
	i32 2562349572, ; 222: Microsoft.CSharp => 0x98ba5a04 => 3
	i32 2570120770, ; 223: System.Text.Encodings.Web => 0x9930ee42 => 138
	i32 2585220780, ; 224: System.Text.Encoding.Extensions.dll => 0x9a1756ac => 136
	i32 2585805581, ; 225: System.Net.Ping => 0x9a20430d => 71
	i32 2589602615, ; 226: System.Threading.ThreadPool => 0x9a5a3337 => 148
	i32 2617129537, ; 227: System.Private.Xml.dll => 0x9bfe3a41 => 90
	i32 2618712057, ; 228: System.Reflection.TypeExtensions.dll => 0x9c165ff9 => 98
	i32 2627185994, ; 229: System.Diagnostics.TextWriterTraceListener.dll => 0x9c97ad4a => 33
	i32 2629843544, ; 230: System.IO.Compression.ZipFile.dll => 0x9cc03a58 => 47
	i32 2663698177, ; 231: System.Runtime.Loader => 0x9ec4cf01 => 111
	i32 2664396074, ; 232: System.Xml.XDocument.dll => 0x9ecf752a => 160
	i32 2665622720, ; 233: System.Drawing.Primitives => 0x9ee22cc0 => 37
	i32 2676780864, ; 234: System.Data.Common.dll => 0x9f8c6f40 => 24
	i32 2686887180, ; 235: System.Runtime.Serialization.Xml.dll => 0xa026a50c => 116
	i32 2693849962, ; 236: System.IO.dll => 0xa090e36a => 59
	i32 2715334215, ; 237: System.Threading.Tasks.dll => 0xa1d8b647 => 146
	i32 2717744543, ; 238: System.Security.Claims => 0xa1fd7d9f => 120
	i32 2719963679, ; 239: System.Security.Cryptography.Cng.dll => 0xa21f5a1f => 122
	i32 2724373263, ; 240: System.Runtime.Numerics.dll => 0xa262a30f => 112
	i32 2735172069, ; 241: System.Threading.Channels => 0xa30769e5 => 141
	i32 2740948882, ; 242: System.IO.Pipes.AccessControl => 0xa35f8f92 => 56
	i32 2748088231, ; 243: System.Runtime.InteropServices.JavaScript => 0xa3cc7fa7 => 107
	i32 2765824710, ; 244: System.Text.Encoding.CodePages.dll => 0xa4db22c6 => 135
	i32 2795666278, ; 245: Microsoft.Win32.SystemEvents => 0xa6a27b66 => 175
	i32 2803228030, ; 246: System.Xml.XPath.XDocument.dll => 0xa715dd7e => 161
	i32 2819470561, ; 247: System.Xml.dll => 0xa80db4e1 => 165
	i32 2821205001, ; 248: System.ServiceProcess.dll => 0xa8282c09 => 134
	i32 2824502124, ; 249: System.Xml.XmlDocument => 0xa85a7b6c => 163
	i32 2849599387, ; 250: System.Threading.Overlapped.dll => 0xa9d96f9b => 142
	i32 2861098320, ; 251: Mono.Android.Export.dll => 0xaa88e550 => 171
	i32 2875220617, ; 252: System.Globalization.Calendars.dll => 0xab606289 => 42
	i32 2887636118, ; 253: System.Net.dll => 0xac1dd496 => 83
	i32 2899753641, ; 254: System.IO.UnmanagedMemoryStream => 0xacd6baa9 => 58
	i32 2900621748, ; 255: System.Dynamic.Runtime.dll => 0xace3f9b4 => 39
	i32 2901442782, ; 256: System.Reflection => 0xacf080de => 99
	i32 2905242038, ; 257: mscorlib.dll => 0xad2a79b6 => 168
	i32 2909740682, ; 258: System.Private.CoreLib => 0xad6f1e8a => 174
	i32 2919462931, ; 259: System.Numerics.Vectors.dll => 0xae037813 => 84
	i32 2936416060, ; 260: System.Resources.Reader => 0xaf06273c => 100
	i32 2940926066, ; 261: System.Diagnostics.StackTrace.dll => 0xaf4af872 => 32
	i32 2942453041, ; 262: System.Xml.XPath.XDocument => 0xaf624531 => 161
	i32 2959614098, ; 263: System.ComponentModel.dll => 0xb0682092 => 20
	i32 2968338931, ; 264: System.Security.Principal.Windows => 0xb0ed41f3 => 129
	i32 2972252294, ; 265: System.Security.Cryptography.Algorithms.dll => 0xb128f886 => 121
	i32 3023353419, ; 266: WindowsBase.dll => 0xb434b64b => 167
	i32 3038032645, ; 267: _Microsoft.Android.Resource.Designer.dll => 0xb514b305 => 185
	i32 3059408633, ; 268: Mono.Android.Runtime => 0xb65adef9 => 172
	i32 3059793426, ; 269: System.ComponentModel.Primitives => 0xb660be12 => 18
	i32 3075834255, ; 270: System.Threading.Tasks => 0xb755818f => 146
	i32 3090735792, ; 271: System.Security.Cryptography.X509Certificates.dll => 0xb838e2b0 => 127
	i32 3099732863, ; 272: System.Security.Claims.dll => 0xb8c22b7f => 120
	i32 3103600923, ; 273: System.Formats.Asn1 => 0xb8fd311b => 40
	i32 3111772706, ; 274: System.Runtime.Serialization => 0xb979e222 => 117
	i32 3121463068, ; 275: System.IO.FileSystem.AccessControl.dll => 0xba0dbf1c => 49
	i32 3124832203, ; 276: System.Threading.Tasks.Extensions => 0xba4127cb => 144
	i32 3132293585, ; 277: System.Security.AccessControl => 0xbab301d1 => 119
	i32 3147165239, ; 278: System.Diagnostics.Tracing.dll => 0xbb95ee37 => 36
	i32 3159123045, ; 279: System.Reflection.Primitives.dll => 0xbc4c6465 => 97
	i32 3160747431, ; 280: System.IO.MemoryMappedFiles => 0xbc652da7 => 55
	i32 3192346100, ; 281: System.Security.SecureString => 0xbe4755f4 => 131
	i32 3193515020, ; 282: System.Web => 0xbe592c0c => 155
	i32 3204380047, ; 283: System.Data.dll => 0xbefef58f => 26
	i32 3209718065, ; 284: System.Xml.XmlDocument.dll => 0xbf506931 => 163
	i32 3217618498, ; 285: Microsoft.VisualStudio.DesignTools.XamlTapContract => 0xbfc8f642 => 183
	i32 3220365878, ; 286: System.Threading => 0xbff2e236 => 150
	i32 3226221578, ; 287: System.Runtime.Handles.dll => 0xc04c3c0a => 106
	i32 3251039220, ; 288: System.Reflection.DispatchProxy.dll => 0xc1c6ebf4 => 91
	i32 3265493905, ; 289: System.Linq.Queryable.dll => 0xc2a37b91 => 62
	i32 3265893370, ; 290: System.Threading.Tasks.Extensions.dll => 0xc2a993fa => 144
	i32 3277815716, ; 291: System.Resources.Writer.dll => 0xc35f7fa4 => 102
	i32 3279906254, ; 292: Microsoft.Win32.Registry.dll => 0xc37f65ce => 7
	i32 3280506390, ; 293: System.ComponentModel.Annotations.dll => 0xc3888e16 => 15
	i32 3290767353, ; 294: System.Security.Cryptography.Encoding => 0xc4251ff9 => 124
	i32 3299363146, ; 295: System.Text.Encoding => 0xc4a8494a => 137
	i32 3303498502, ; 296: System.Diagnostics.FileVersionInfo => 0xc4e76306 => 30
	i32 3316684772, ; 297: System.Net.Requests.dll => 0xc5b097e4 => 74
	i32 3317144872, ; 298: System.Data => 0xc5b79d28 => 26
	i32 3358260929, ; 299: System.Text.Json => 0xc82afec1 => 139
	i32 3366347497, ; 300: Java.Interop => 0xc8a662e9 => 170
	i32 3395150330, ; 301: System.Runtime.CompilerServices.Unsafe.dll => 0xca5de1fa => 103
	i32 3403906625, ; 302: System.Security.Cryptography.OpenSsl.dll => 0xcae37e41 => 125
	i32 3429136800, ; 303: System.Xml => 0xcc6479a0 => 165
	i32 3430777524, ; 304: netstandard => 0xcc7d82b4 => 169
	i32 3436207936, ; 305: System.Private.Windows.GdiPlus => 0xccd05f40 => 179
	i32 3445260447, ; 306: System.Formats.Tar => 0xcd5a809f => 41
	i32 3471940407, ; 307: System.ComponentModel.TypeConverter.dll => 0xcef19b37 => 19
	i32 3476120550, ; 308: Mono.Android => 0xcf3163e6 => 173
	i32 3485117614, ; 309: System.Text.Json.dll => 0xcfbaacae => 139
	i32 3486566296, ; 310: System.Transactions => 0xcfd0c798 => 152
	i32 3509114376, ; 311: System.Xml.Linq => 0xd128d608 => 157
	i32 3515174580, ; 312: System.Security.dll => 0xd1854eb4 => 132
	i32 3530912306, ; 313: System.Configuration => 0xd2757232 => 21
	i32 3539954161, ; 314: System.Net.HttpListener => 0xd2ff69f1 => 67
	i32 3560100363, ; 315: System.Threading.Timer => 0xd432d20b => 149
	i32 3570554715, ; 316: System.IO.FileSystem.AccessControl => 0xd4d2575b => 49
	i32 3598340787, ; 317: System.Net.WebSockets.Client => 0xd67a52b3 => 81
	i32 3608519521, ; 318: System.Linq.dll => 0xd715a361 => 63
	i32 3624195450, ; 319: System.Runtime.InteropServices.RuntimeInformation => 0xd804d57a => 108
	i32 3638274909, ; 320: System.IO.FileSystem.Primitives.dll => 0xd8dbab5d => 51
	i32 3645089577, ; 321: System.ComponentModel.DataAnnotations => 0xd943a729 => 16
	i32 3660523487, ; 322: System.Net.NetworkInformation => 0xda2f27df => 70
	i32 3672681054, ; 323: Mono.Android.dll => 0xdae8aa5e => 173
	i32 3676670898, ; 324: Microsoft.Maui.Controls.HotReload.Forms => 0xdb258bb2 => 180
	i32 3689375977, ; 325: System.Drawing.Common => 0xdbe768e9 => 177
	i32 3700866549, ; 326: System.Net.WebProxy.dll => 0xdc96bdf5 => 80
	i32 3716563718, ; 327: System.Runtime.Intrinsics => 0xdd864306 => 110
	i32 3732100267, ; 328: System.Net.NameResolution => 0xde7354ab => 69
	i32 3737834244, ; 329: System.Net.Http.Json.dll => 0xdecad304 => 65
	i32 3748608112, ; 330: System.Diagnostics.DiagnosticSource => 0xdf6f3870 => 29
	i32 3751444290, ; 331: System.Xml.XPath => 0xdf9a7f42 => 162
	i32 3792276235, ; 332: System.Collections.NonGeneric => 0xe2098b0b => 12
	i32 3802395368, ; 333: System.Collections.Specialized.dll => 0xe2a3f2e8 => 13
	i32 3819260425, ; 334: System.Net.WebProxy => 0xe3a54a09 => 80
	i32 3823082795, ; 335: System.Security.Cryptography.dll => 0xe3df9d2b => 128
	i32 3829621856, ; 336: System.Numerics.dll => 0xe4436460 => 85
	i32 3831343120, ; 337: MonoGame.Framework.dll => 0xe45da810 => 176
	i32 3844307129, ; 338: System.Net.Mail.dll => 0xe52378b9 => 68
	i32 3849253459, ; 339: System.Runtime.InteropServices.dll => 0xe56ef253 => 109
	i32 3870376305, ; 340: System.Net.HttpListener.dll => 0xe6b14171 => 67
	i32 3873536506, ; 341: System.Security.Principal => 0xe6e179fa => 130
	i32 3875112723, ; 342: System.Security.Cryptography.Encoding.dll => 0xe6f98713 => 124
	i32 3885497537, ; 343: System.Net.WebHeaderCollection.dll => 0xe797fcc1 => 79
	i32 3896106733, ; 344: System.Collections.Concurrent.dll => 0xe839deed => 10
	i32 3901907137, ; 345: Microsoft.VisualBasic.Core.dll => 0xe89260c1 => 4
	i32 3920810846, ; 346: System.IO.Compression.FileSystem.dll => 0xe9b2d35e => 46
	i32 3928044579, ; 347: System.Xml.ReaderWriter => 0xea213423 => 158
	i32 3930554604, ; 348: System.Security.Principal.dll => 0xea4780ec => 130
	i32 3945713374, ; 349: System.Data.DataSetExtensions.dll => 0xeb2ecede => 25
	i32 3953953790, ; 350: System.Text.Encoding.CodePages => 0xebac8bfe => 135
	i32 4003436829, ; 351: System.Diagnostics.Process.dll => 0xee9f991d => 31
	i32 4025784931, ; 352: System.Memory => 0xeff49a63 => 64
	i32 4054681211, ; 353: System.Reflection.Emit.ILGeneration => 0xf1ad867b => 92
	i32 4068434129, ; 354: System.Private.Xml.Linq.dll => 0xf27f60d1 => 89
	i32 4073602200, ; 355: System.Threading.dll => 0xf2ce3c98 => 150
	i32 4099507663, ; 356: System.Drawing.dll => 0xf45985cf => 38
	i32 4100113165, ; 357: System.Private.Uri => 0xf462c30d => 88
	i32 4127667938, ; 358: System.IO.FileSystem.Watcher => 0xf60736e2 => 52
	i32 4130442656, ; 359: System.AppContext => 0xf6318da0 => 8
	i32 4147896353, ; 360: System.Reflection.Emit.ILGeneration.dll => 0xf73be021 => 92
	i32 4151237749, ; 361: System.Core => 0xf76edc75 => 23
	i32 4159265925, ; 362: System.Xml.XmlSerializer => 0xf7e95c85 => 164
	i32 4161255271, ; 363: System.Reflection.TypeExtensions => 0xf807b767 => 98
	i32 4164802419, ; 364: System.IO.FileSystem.Watcher.dll => 0xf83dd773 => 52
	i32 4181436372, ; 365: System.Runtime.Serialization.Primitives => 0xf93ba7d4 => 115
	i32 4182880526, ; 366: Microsoft.VisualStudio.DesignTools.MobileTapContracts => 0xf951b10e => 181
	i32 4185676441, ; 367: System.Security => 0xf97c5a99 => 132
	i32 4196529839, ; 368: System.Net.WebClient.dll => 0xfa21f6af => 78
	i32 4213026141, ; 369: System.Diagnostics.DiagnosticSource.dll => 0xfb1dad5d => 29
	i32 4260525087, ; 370: System.Buffers => 0xfdf2741f => 9
	i32 4274976490 ; 371: System.Runtime.Numerics => 0xfecef6ea => 112
], align 4

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [372 x i32] [
	i32 70, ; 0
	i32 69, ; 1
	i32 110, ; 2
	i32 184, ; 3
	i32 50, ; 4
	i32 82, ; 5
	i32 147, ; 6
	i32 175, ; 7
	i32 32, ; 8
	i32 126, ; 9
	i32 104, ; 10
	i32 109, ; 11
	i32 141, ; 12
	i32 79, ; 13
	i32 126, ; 14
	i32 15, ; 15
	i32 134, ; 16
	i32 153, ; 17
	i32 20, ; 18
	i32 1, ; 19
	i32 28, ; 20
	i32 3, ; 21
	i32 61, ; 22
	i32 44, ; 23
	i32 93, ; 24
	i32 149, ; 25
	i32 56, ; 26
	i32 71, ; 27
	i32 85, ; 28
	i32 133, ; 29
	i32 57, ; 30
	i32 151, ; 31
	i32 76, ; 32
	i32 147, ; 33
	i32 64, ; 34
	i32 148, ; 35
	i32 185, ; 36
	i32 167, ; 37
	i32 14, ; 38
	i32 127, ; 39
	i32 154, ; 40
	i32 115, ; 41
	i32 184, ; 42
	i32 168, ; 43
	i32 166, ; 44
	i32 86, ; 45
	i32 176, ; 46
	i32 152, ; 47
	i32 62, ; 48
	i32 53, ; 49
	i32 105, ; 50
	i32 116, ; 51
	i32 42, ; 52
	i32 2, ; 53
	i32 122, ; 54
	i32 54, ; 55
	i32 46, ; 56
	i32 121, ; 57
	i32 83, ; 58
	i32 138, ; 59
	i32 10, ; 60
	i32 75, ; 61
	i32 157, ; 62
	i32 156, ; 63
	i32 94, ; 64
	i32 47, ; 65
	i32 111, ; 66
	i32 131, ; 67
	i32 27, ; 68
	i32 74, ; 69
	i32 57, ; 70
	i32 48, ; 71
	i32 24, ; 72
	i32 177, ; 73
	i32 88, ; 74
	i32 45, ; 75
	i32 162, ; 76
	i32 73, ; 77
	i32 0, ; 78
	i32 5, ; 79
	i32 178, ; 80
	i32 44, ; 81
	i32 65, ; 82
	i32 2, ; 83
	i32 18, ; 84
	i32 55, ; 85
	i32 107, ; 86
	i32 36, ; 87
	i32 160, ; 88
	i32 87, ; 89
	i32 34, ; 90
	i32 14, ; 91
	i32 53, ; 92
	i32 58, ; 93
	i32 38, ; 94
	i32 37, ; 95
	i32 60, ; 96
	i32 19, ; 97
	i32 166, ; 98
	i32 1, ; 99
	i32 155, ; 100
	i32 31, ; 101
	i32 54, ; 102
	i32 7, ; 103
	i32 87, ; 104
	i32 63, ; 105
	i32 114, ; 106
	i32 59, ; 107
	i32 0, ; 108
	i32 101, ; 109
	i32 179, ; 110
	i32 21, ; 111
	i32 113, ; 112
	i32 103, ; 113
	i32 104, ; 114
	i32 106, ; 115
	i32 73, ; 116
	i32 40, ; 117
	i32 34, ; 118
	i32 105, ; 119
	i32 75, ; 120
	i32 11, ; 121
	i32 125, ; 122
	i32 48, ; 123
	i32 11, ; 124
	i32 45, ; 125
	i32 6, ; 126
	i32 33, ; 127
	i32 140, ; 128
	i32 94, ; 129
	i32 95, ; 130
	i32 51, ; 131
	i32 143, ; 132
	i32 114, ; 133
	i32 142, ; 134
	i32 117, ; 135
	i32 159, ; 136
	i32 180, ; 137
	i32 78, ; 138
	i32 81, ; 139
	i32 39, ; 140
	i32 66, ; 141
	i32 140, ; 142
	i32 17, ; 143
	i32 178, ; 144
	i32 118, ; 145
	i32 50, ; 146
	i32 72, ; 147
	i32 82, ; 148
	i32 128, ; 149
	i32 96, ; 150
	i32 123, ; 151
	i32 28, ; 152
	i32 99, ; 153
	i32 30, ; 154
	i32 151, ; 155
	i32 171, ; 156
	i32 6, ; 157
	i32 100, ; 158
	i32 35, ; 159
	i32 95, ; 160
	i32 23, ; 161
	i32 43, ; 162
	i32 172, ; 163
	i32 4, ; 164
	i32 136, ; 165
	i32 113, ; 166
	i32 182, ; 167
	i32 60, ; 168
	i32 97, ; 169
	i32 41, ; 170
	i32 182, ; 171
	i32 27, ; 172
	i32 96, ; 173
	i32 91, ; 174
	i32 101, ; 175
	i32 12, ; 176
	i32 89, ; 177
	i32 102, ; 178
	i32 9, ; 179
	i32 90, ; 180
	i32 156, ; 181
	i32 35, ; 182
	i32 118, ; 183
	i32 84, ; 184
	i32 183, ; 185
	i32 22, ; 186
	i32 13, ; 187
	i32 164, ; 188
	i32 5, ; 189
	i32 86, ; 190
	i32 66, ; 191
	i32 145, ; 192
	i32 159, ; 193
	i32 43, ; 194
	i32 119, ; 195
	i32 133, ; 196
	i32 77, ; 197
	i32 68, ; 198
	i32 174, ; 199
	i32 145, ; 200
	i32 108, ; 201
	i32 153, ; 202
	i32 72, ; 203
	i32 158, ; 204
	i32 123, ; 205
	i32 129, ; 206
	i32 154, ; 207
	i32 181, ; 208
	i32 143, ; 209
	i32 22, ; 210
	i32 16, ; 211
	i32 137, ; 212
	i32 77, ; 213
	i32 61, ; 214
	i32 169, ; 215
	i32 170, ; 216
	i32 17, ; 217
	i32 76, ; 218
	i32 8, ; 219
	i32 25, ; 220
	i32 93, ; 221
	i32 3, ; 222
	i32 138, ; 223
	i32 136, ; 224
	i32 71, ; 225
	i32 148, ; 226
	i32 90, ; 227
	i32 98, ; 228
	i32 33, ; 229
	i32 47, ; 230
	i32 111, ; 231
	i32 160, ; 232
	i32 37, ; 233
	i32 24, ; 234
	i32 116, ; 235
	i32 59, ; 236
	i32 146, ; 237
	i32 120, ; 238
	i32 122, ; 239
	i32 112, ; 240
	i32 141, ; 241
	i32 56, ; 242
	i32 107, ; 243
	i32 135, ; 244
	i32 175, ; 245
	i32 161, ; 246
	i32 165, ; 247
	i32 134, ; 248
	i32 163, ; 249
	i32 142, ; 250
	i32 171, ; 251
	i32 42, ; 252
	i32 83, ; 253
	i32 58, ; 254
	i32 39, ; 255
	i32 99, ; 256
	i32 168, ; 257
	i32 174, ; 258
	i32 84, ; 259
	i32 100, ; 260
	i32 32, ; 261
	i32 161, ; 262
	i32 20, ; 263
	i32 129, ; 264
	i32 121, ; 265
	i32 167, ; 266
	i32 185, ; 267
	i32 172, ; 268
	i32 18, ; 269
	i32 146, ; 270
	i32 127, ; 271
	i32 120, ; 272
	i32 40, ; 273
	i32 117, ; 274
	i32 49, ; 275
	i32 144, ; 276
	i32 119, ; 277
	i32 36, ; 278
	i32 97, ; 279
	i32 55, ; 280
	i32 131, ; 281
	i32 155, ; 282
	i32 26, ; 283
	i32 163, ; 284
	i32 183, ; 285
	i32 150, ; 286
	i32 106, ; 287
	i32 91, ; 288
	i32 62, ; 289
	i32 144, ; 290
	i32 102, ; 291
	i32 7, ; 292
	i32 15, ; 293
	i32 124, ; 294
	i32 137, ; 295
	i32 30, ; 296
	i32 74, ; 297
	i32 26, ; 298
	i32 139, ; 299
	i32 170, ; 300
	i32 103, ; 301
	i32 125, ; 302
	i32 165, ; 303
	i32 169, ; 304
	i32 179, ; 305
	i32 41, ; 306
	i32 19, ; 307
	i32 173, ; 308
	i32 139, ; 309
	i32 152, ; 310
	i32 157, ; 311
	i32 132, ; 312
	i32 21, ; 313
	i32 67, ; 314
	i32 149, ; 315
	i32 49, ; 316
	i32 81, ; 317
	i32 63, ; 318
	i32 108, ; 319
	i32 51, ; 320
	i32 16, ; 321
	i32 70, ; 322
	i32 173, ; 323
	i32 180, ; 324
	i32 177, ; 325
	i32 80, ; 326
	i32 110, ; 327
	i32 69, ; 328
	i32 65, ; 329
	i32 29, ; 330
	i32 162, ; 331
	i32 12, ; 332
	i32 13, ; 333
	i32 80, ; 334
	i32 128, ; 335
	i32 85, ; 336
	i32 176, ; 337
	i32 68, ; 338
	i32 109, ; 339
	i32 67, ; 340
	i32 130, ; 341
	i32 124, ; 342
	i32 79, ; 343
	i32 10, ; 344
	i32 4, ; 345
	i32 46, ; 346
	i32 158, ; 347
	i32 130, ; 348
	i32 25, ; 349
	i32 135, ; 350
	i32 31, ; 351
	i32 64, ; 352
	i32 92, ; 353
	i32 89, ; 354
	i32 150, ; 355
	i32 38, ; 356
	i32 88, ; 357
	i32 52, ; 358
	i32 8, ; 359
	i32 92, ; 360
	i32 23, ; 361
	i32 164, ; 362
	i32 98, ; 363
	i32 52, ; 364
	i32 115, ; 365
	i32 181, ; 366
	i32 132, ; 367
	i32 78, ; 368
	i32 29, ; 369
	i32 9, ; 370
	i32 112 ; 371
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 4

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 4

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 4

; Functions

; Function attributes: "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 4, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-thumb-mode,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-thumb-mode,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" }

; Metadata
!llvm.module.flags = !{!0, !1, !7}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!"Xamarin.Android remotes/origin/release/8.0.4xx @ 82d8938cf80f6d5fa6c28529ddfbdb753d805ab4"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"min_enum_size", i32 4}
