.class public Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;
.super Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;
.source "XmlReaderResourceParser.java"

# interfaces
.implements Lmono/android/IGCUserPeer;
.implements Landroid/content/res/XmlResourceParser;
.implements Landroid/util/AttributeSet;
.implements Lorg/xmlpull/v1/XmlPullParser;


# static fields
.field static final __md_methods:Ljava/lang/String;


# instance fields
.field refList:Ljava/util/ArrayList;


# direct methods
.method static constructor <clinit>()V
    .locals 3

    .prologue
    .line 14
    const-string v0, "n_close:()V:GetCloseHandler:Android.Content.Res.IXmlResourceParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeCount:()I:GetGetAttributeCountHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getClassAttribute:()Ljava/lang/String;:GetGetClassAttributeHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getIdAttribute:()Ljava/lang/String;:GetGetIdAttributeHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getPositionDescription:()Ljava/lang/String;:GetGetPositionDescriptionHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getStyleAttribute:()I:GetGetStyleAttributeHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeBooleanValue:(IZ)Z:GetGetAttributeBooleanValue_IZHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeBooleanValue:(Ljava/lang/String;Ljava/lang/String;Z)Z:GetGetAttributeBooleanValue_Ljava_lang_String_Ljava_lang_String_ZHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeFloatValue:(IF)F:GetGetAttributeFloatValue_IFHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeFloatValue:(Ljava/lang/String;Ljava/lang/String;F)F:GetGetAttributeFloatValue_Ljava_lang_String_Ljava_lang_String_FHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeIntValue:(II)I:GetGetAttributeIntValue_IIHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeIntValue:(Ljava/lang/String;Ljava/lang/String;I)I:GetGetAttributeIntValue_Ljava_lang_String_Ljava_lang_String_IHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeListValue:(I[Ljava/lang/String;I)I:GetGetAttributeListValue_IarrayLjava_lang_String_IHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeListValue:(Ljava/lang/String;Ljava/lang/String;[Ljava/lang/String;I)I:GetGetAttributeListValue_Ljava_lang_String_Ljava_lang_String_arrayLjava_lang_String_IHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeName:(I)Ljava/lang/String;:GetGetAttributeName_IHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeNameResource:(I)I:GetGetAttributeNameResource_IHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeResourceValue:(II)I:GetGetAttributeResourceValue_IIHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeResourceValue:(Ljava/lang/String;Ljava/lang/String;I)I:GetGetAttributeResourceValue_Ljava_lang_String_Ljava_lang_String_IHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeUnsignedIntValue:(II)I:GetGetAttributeUnsignedIntValue_IIHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeUnsignedIntValue:(Ljava/lang/String;Ljava/lang/String;I)I:GetGetAttributeUnsignedIntValue_Ljava_lang_String_Ljava_lang_String_IHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeValue:(I)Ljava/lang/String;:GetGetAttributeValue_IHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeValue:(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;:GetGetAttributeValue_Ljava_lang_String_Ljava_lang_String_Handler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getIdAttributeResourceValue:(I)I:GetGetIdAttributeResourceValue_IHandler:Android.Util.IAttributeSetInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getColumnNumber:()I:GetGetColumnNumberHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getDepth:()I:GetGetDepthHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getEventType:()I:GetGetEventTypeHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getInputEncoding:()Ljava/lang/String;:GetGetInputEncodingHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_isEmptyElementTag:()Z:GetIsEmptyElementTagHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_isWhitespace:()Z:GetIsWhitespaceHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getLineNumber:()I:GetGetLineNumberHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getName:()Ljava/lang/String;:GetGetNameHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getNamespace:()Ljava/lang/String;:GetGetNamespaceHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getPrefix:()Ljava/lang/String;:GetGetPrefixHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getText:()Ljava/lang/String;:GetGetTextHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_defineEntityReplacementText:(Ljava/lang/String;Ljava/lang/String;)V:GetDefineEntityReplacementText_Ljava_lang_String_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeNamespace:(I)Ljava/lang/String;:GetGetAttributeNamespace_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributePrefix:(I)Ljava/lang/String;:GetGetAttributePrefix_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeType:(I)Ljava/lang/String;:GetGetAttributeType_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getFeature:(Ljava/lang/String;)Z:GetGetFeature_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getNamespace:(Ljava/lang/String;)Ljava/lang/String;:GetGetNamespace_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getNamespaceCount:(I)I:GetGetNamespaceCount_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getNamespacePrefix:(I)Ljava/lang/String;:GetGetNamespacePrefix_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getNamespaceUri:(I)Ljava/lang/String;:GetGetNamespaceUri_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getProperty:(Ljava/lang/String;)Ljava/lang/Object;:GetGetProperty_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getTextCharacters:([I)[C:GetGetTextCharacters_arrayIHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_isAttributeDefault:(I)Z:GetIsAttributeDefault_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_next:()I:GetNextHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_nextTag:()I:GetNextTagHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_nextText:()Ljava/lang/String;:GetNextTextHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_nextToken:()I:GetNextTokenHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_require:(ILjava/lang/String;Ljava/lang/String;)V:GetRequire_ILjava_lang_String_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_setFeature:(Ljava/lang/String;Z)V:GetSetFeature_Ljava_lang_String_ZHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_setInput:(Ljava/io/InputStream;Ljava/lang/String;)V:GetSetInput_Ljava_io_InputStream_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_setInput:(Ljava/io/Reader;)V:GetSetInput_Ljava_io_Reader_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_setProperty:(Ljava/lang/String;Ljava/lang/Object;)V:GetSetProperty_Ljava_lang_String_Ljava_lang_Object_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n"

    sput-object v0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->__md_methods:Ljava/lang/String;

    .line 71
    const-string v0, "Android.Runtime.XmlReaderResourceParser, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-class v1, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;

    sget-object v2, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->__md_methods:Ljava/lang/String;

    invoke-static {v0, v1, v2}, Lmono/android/Runtime;->register(Ljava/lang/String;Ljava/lang/Class;Ljava/lang/String;)V

    .line 72
    return-void
.end method

.method public constructor <init>()V
    .locals 3
    .annotation system Ldalvik/annotation/Throws;
        value = {
            Ljava/lang/Throwable;
        }
    .end annotation

    .prologue
    .line 77
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;-><init>()V

    .line 78
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;

    if-ne v0, v1, :cond_0

    .line 79
    const-string v0, "Android.Runtime.XmlReaderResourceParser, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-string v1, ""

    const/4 v2, 0x0

    new-array v2, v2, [Ljava/lang/Object;

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 80
    :cond_0
    return-void
.end method

.method private native n_close()V
.end method

.method private native n_defineEntityReplacementText(Ljava/lang/String;Ljava/lang/String;)V
.end method

.method private native n_getAttributeBooleanValue(IZ)Z
.end method

.method private native n_getAttributeBooleanValue(Ljava/lang/String;Ljava/lang/String;Z)Z
.end method

.method private native n_getAttributeCount()I
.end method

.method private native n_getAttributeFloatValue(IF)F
.end method

.method private native n_getAttributeFloatValue(Ljava/lang/String;Ljava/lang/String;F)F
.end method

.method private native n_getAttributeIntValue(II)I
.end method

.method private native n_getAttributeIntValue(Ljava/lang/String;Ljava/lang/String;I)I
.end method

.method private native n_getAttributeListValue(I[Ljava/lang/String;I)I
.end method

.method private native n_getAttributeListValue(Ljava/lang/String;Ljava/lang/String;[Ljava/lang/String;I)I
.end method

.method private native n_getAttributeName(I)Ljava/lang/String;
.end method

.method private native n_getAttributeNameResource(I)I
.end method

.method private native n_getAttributeNamespace(I)Ljava/lang/String;
.end method

.method private native n_getAttributePrefix(I)Ljava/lang/String;
.end method

.method private native n_getAttributeResourceValue(II)I
.end method

.method private native n_getAttributeResourceValue(Ljava/lang/String;Ljava/lang/String;I)I
.end method

.method private native n_getAttributeType(I)Ljava/lang/String;
.end method

.method private native n_getAttributeUnsignedIntValue(II)I
.end method

.method private native n_getAttributeUnsignedIntValue(Ljava/lang/String;Ljava/lang/String;I)I
.end method

.method private native n_getAttributeValue(I)Ljava/lang/String;
.end method

.method private native n_getAttributeValue(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;
.end method

.method private native n_getClassAttribute()Ljava/lang/String;
.end method

.method private native n_getColumnNumber()I
.end method

.method private native n_getDepth()I
.end method

.method private native n_getEventType()I
.end method

.method private native n_getFeature(Ljava/lang/String;)Z
.end method

.method private native n_getIdAttribute()Ljava/lang/String;
.end method

.method private native n_getIdAttributeResourceValue(I)I
.end method

.method private native n_getInputEncoding()Ljava/lang/String;
.end method

.method private native n_getLineNumber()I
.end method

.method private native n_getName()Ljava/lang/String;
.end method

.method private native n_getNamespace()Ljava/lang/String;
.end method

.method private native n_getNamespace(Ljava/lang/String;)Ljava/lang/String;
.end method

.method private native n_getNamespaceCount(I)I
.end method

.method private native n_getNamespacePrefix(I)Ljava/lang/String;
.end method

.method private native n_getNamespaceUri(I)Ljava/lang/String;
.end method

.method private native n_getPositionDescription()Ljava/lang/String;
.end method

.method private native n_getPrefix()Ljava/lang/String;
.end method

.method private native n_getProperty(Ljava/lang/String;)Ljava/lang/Object;
.end method

.method private native n_getStyleAttribute()I
.end method

.method private native n_getText()Ljava/lang/String;
.end method

.method private native n_getTextCharacters([I)[C
.end method

.method private native n_isAttributeDefault(I)Z
.end method

.method private native n_isEmptyElementTag()Z
.end method

.method private native n_isWhitespace()Z
.end method

.method private native n_next()I
.end method

.method private native n_nextTag()I
.end method

.method private native n_nextText()Ljava/lang/String;
.end method

.method private native n_nextToken()I
.end method

.method private native n_require(ILjava/lang/String;Ljava/lang/String;)V
.end method

.method private native n_setFeature(Ljava/lang/String;Z)V
.end method

.method private native n_setInput(Ljava/io/InputStream;Ljava/lang/String;)V
.end method

.method private native n_setInput(Ljava/io/Reader;)V
.end method

.method private native n_setProperty(Ljava/lang/String;Ljava/lang/Object;)V
.end method


# virtual methods
.method public close()V
    .locals 0

    .prologue
    .line 85
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_close()V

    .line 86
    return-void
.end method

.method public defineEntityReplacementText(Ljava/lang/String;Ljava/lang/String;)V
    .locals 0

    .prologue
    .line 357
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_defineEntityReplacementText(Ljava/lang/String;Ljava/lang/String;)V

    .line 358
    return-void
.end method

.method public getAttributeBooleanValue(IZ)Z
    .locals 1

    .prologue
    .line 133
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeBooleanValue(IZ)Z

    move-result v0

    return v0
.end method

.method public getAttributeBooleanValue(Ljava/lang/String;Ljava/lang/String;Z)Z
    .locals 1

    .prologue
    .line 141
    invoke-direct {p0, p1, p2, p3}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeBooleanValue(Ljava/lang/String;Ljava/lang/String;Z)Z

    move-result v0

    return v0
.end method

.method public getAttributeCount()I
    .locals 1

    .prologue
    .line 93
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeCount()I

    move-result v0

    return v0
.end method

.method public getAttributeFloatValue(IF)F
    .locals 1

    .prologue
    .line 149
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeFloatValue(IF)F

    move-result v0

    return v0
.end method

.method public getAttributeFloatValue(Ljava/lang/String;Ljava/lang/String;F)F
    .locals 1

    .prologue
    .line 157
    invoke-direct {p0, p1, p2, p3}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeFloatValue(Ljava/lang/String;Ljava/lang/String;F)F

    move-result v0

    return v0
.end method

.method public getAttributeIntValue(II)I
    .locals 1

    .prologue
    .line 165
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeIntValue(II)I

    move-result v0

    return v0
.end method

.method public getAttributeIntValue(Ljava/lang/String;Ljava/lang/String;I)I
    .locals 1

    .prologue
    .line 173
    invoke-direct {p0, p1, p2, p3}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeIntValue(Ljava/lang/String;Ljava/lang/String;I)I

    move-result v0

    return v0
.end method

.method public getAttributeListValue(I[Ljava/lang/String;I)I
    .locals 1

    .prologue
    .line 181
    invoke-direct {p0, p1, p2, p3}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeListValue(I[Ljava/lang/String;I)I

    move-result v0

    return v0
.end method

.method public getAttributeListValue(Ljava/lang/String;Ljava/lang/String;[Ljava/lang/String;I)I
    .locals 1

    .prologue
    .line 189
    invoke-direct {p0, p1, p2, p3, p4}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeListValue(Ljava/lang/String;Ljava/lang/String;[Ljava/lang/String;I)I

    move-result v0

    return v0
.end method

.method public getAttributeName(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 197
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeName(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getAttributeNameResource(I)I
    .locals 1

    .prologue
    .line 205
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeNameResource(I)I

    move-result v0

    return v0
.end method

.method public getAttributeNamespace(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 365
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeNamespace(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getAttributePrefix(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 373
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributePrefix(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getAttributeResourceValue(II)I
    .locals 1

    .prologue
    .line 213
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeResourceValue(II)I

    move-result v0

    return v0
.end method

.method public getAttributeResourceValue(Ljava/lang/String;Ljava/lang/String;I)I
    .locals 1

    .prologue
    .line 221
    invoke-direct {p0, p1, p2, p3}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeResourceValue(Ljava/lang/String;Ljava/lang/String;I)I

    move-result v0

    return v0
.end method

.method public getAttributeType(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 381
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeType(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getAttributeUnsignedIntValue(II)I
    .locals 1

    .prologue
    .line 229
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeUnsignedIntValue(II)I

    move-result v0

    return v0
.end method

.method public getAttributeUnsignedIntValue(Ljava/lang/String;Ljava/lang/String;I)I
    .locals 1

    .prologue
    .line 237
    invoke-direct {p0, p1, p2, p3}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeUnsignedIntValue(Ljava/lang/String;Ljava/lang/String;I)I

    move-result v0

    return v0
.end method

.method public getAttributeValue(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 245
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeValue(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getAttributeValue(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;
    .locals 1

    .prologue
    .line 253
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getAttributeValue(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getClassAttribute()Ljava/lang/String;
    .locals 1

    .prologue
    .line 101
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getClassAttribute()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getColumnNumber()I
    .locals 1

    .prologue
    .line 269
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getColumnNumber()I

    move-result v0

    return v0
.end method

.method public getDepth()I
    .locals 1

    .prologue
    .line 277
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getDepth()I

    move-result v0

    return v0
.end method

.method public getEventType()I
    .locals 1

    .prologue
    .line 285
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getEventType()I

    move-result v0

    return v0
.end method

.method public getFeature(Ljava/lang/String;)Z
    .locals 1

    .prologue
    .line 389
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getFeature(Ljava/lang/String;)Z

    move-result v0

    return v0
.end method

.method public getIdAttribute()Ljava/lang/String;
    .locals 1

    .prologue
    .line 109
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getIdAttribute()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getIdAttributeResourceValue(I)I
    .locals 1

    .prologue
    .line 261
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getIdAttributeResourceValue(I)I

    move-result v0

    return v0
.end method

.method public getInputEncoding()Ljava/lang/String;
    .locals 1

    .prologue
    .line 293
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getInputEncoding()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getLineNumber()I
    .locals 1

    .prologue
    .line 317
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getLineNumber()I

    move-result v0

    return v0
.end method

.method public getName()Ljava/lang/String;
    .locals 1

    .prologue
    .line 325
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getName()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getNamespace()Ljava/lang/String;
    .locals 1

    .prologue
    .line 333
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getNamespace()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getNamespace(Ljava/lang/String;)Ljava/lang/String;
    .locals 1

    .prologue
    .line 397
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getNamespace(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getNamespaceCount(I)I
    .locals 1

    .prologue
    .line 405
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getNamespaceCount(I)I

    move-result v0

    return v0
.end method

.method public getNamespacePrefix(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 413
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getNamespacePrefix(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getNamespaceUri(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 421
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getNamespaceUri(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getPositionDescription()Ljava/lang/String;
    .locals 1

    .prologue
    .line 117
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getPositionDescription()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getPrefix()Ljava/lang/String;
    .locals 1

    .prologue
    .line 341
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getPrefix()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getProperty(Ljava/lang/String;)Ljava/lang/Object;
    .locals 1

    .prologue
    .line 429
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getProperty(Ljava/lang/String;)Ljava/lang/Object;

    move-result-object v0

    return-object v0
.end method

.method public getStyleAttribute()I
    .locals 1

    .prologue
    .line 125
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getStyleAttribute()I

    move-result v0

    return v0
.end method

.method public getText()Ljava/lang/String;
    .locals 1

    .prologue
    .line 349
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getText()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getTextCharacters([I)[C
    .locals 1

    .prologue
    .line 437
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_getTextCharacters([I)[C

    move-result-object v0

    return-object v0
.end method

.method public isAttributeDefault(I)Z
    .locals 1

    .prologue
    .line 445
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_isAttributeDefault(I)Z

    move-result v0

    return v0
.end method

.method public isEmptyElementTag()Z
    .locals 1

    .prologue
    .line 301
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_isEmptyElementTag()Z

    move-result v0

    return v0
.end method

.method public isWhitespace()Z
    .locals 1

    .prologue
    .line 309
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_isWhitespace()Z

    move-result v0

    return v0
.end method

.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 525
    iget-object v0, p0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 526
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->refList:Ljava/util/ArrayList;

    .line 527
    :cond_0
    iget-object v0, p0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 528
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 532
    iget-object v0, p0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 533
    iget-object v0, p0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 534
    :cond_0
    return-void
.end method

.method public next()I
    .locals 1

    .prologue
    .line 453
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_next()I

    move-result v0

    return v0
.end method

.method public nextTag()I
    .locals 1

    .prologue
    .line 461
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_nextTag()I

    move-result v0

    return v0
.end method

.method public nextText()Ljava/lang/String;
    .locals 1

    .prologue
    .line 469
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_nextText()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public nextToken()I
    .locals 1

    .prologue
    .line 477
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_nextToken()I

    move-result v0

    return v0
.end method

.method public require(ILjava/lang/String;Ljava/lang/String;)V
    .locals 0

    .prologue
    .line 485
    invoke-direct {p0, p1, p2, p3}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_require(ILjava/lang/String;Ljava/lang/String;)V

    .line 486
    return-void
.end method

.method public setFeature(Ljava/lang/String;Z)V
    .locals 0

    .prologue
    .line 493
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_setFeature(Ljava/lang/String;Z)V

    .line 494
    return-void
.end method

.method public setInput(Ljava/io/InputStream;Ljava/lang/String;)V
    .locals 0

    .prologue
    .line 501
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_setInput(Ljava/io/InputStream;Ljava/lang/String;)V

    .line 502
    return-void
.end method

.method public setInput(Ljava/io/Reader;)V
    .locals 0

    .prologue
    .line 509
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_setInput(Ljava/io/Reader;)V

    .line 510
    return-void
.end method

.method public setProperty(Ljava/lang/String;Ljava/lang/Object;)V
    .locals 0

    .prologue
    .line 517
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderResourceParser;->n_setProperty(Ljava/lang/String;Ljava/lang/Object;)V

    .line 518
    return-void
.end method
