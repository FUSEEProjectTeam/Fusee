.class public Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;
.super Ljava/lang/Object;
.source "XmlReaderPullParser.java"

# interfaces
.implements Lmono/android/IGCUserPeer;
.implements Lorg/xmlpull/v1/XmlPullParser;


# static fields
.field static final __md_methods:Ljava/lang/String;


# instance fields
.field refList:Ljava/util/ArrayList;


# direct methods
.method static constructor <clinit>()V
    .locals 3

    .prologue
    .line 12
    const-string v0, "n_getAttributeCount:()I:GetGetAttributeCountHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getColumnNumber:()I:GetGetColumnNumberHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getDepth:()I:GetGetDepthHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getEventType:()I:GetGetEventTypeHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getInputEncoding:()Ljava/lang/String;:GetGetInputEncodingHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_isEmptyElementTag:()Z:GetIsEmptyElementTagHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_isWhitespace:()Z:GetIsWhitespaceHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getLineNumber:()I:GetGetLineNumberHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getName:()Ljava/lang/String;:GetGetNameHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getNamespace:()Ljava/lang/String;:GetGetNamespaceHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getPositionDescription:()Ljava/lang/String;:GetGetPositionDescriptionHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getPrefix:()Ljava/lang/String;:GetGetPrefixHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getText:()Ljava/lang/String;:GetGetTextHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_defineEntityReplacementText:(Ljava/lang/String;Ljava/lang/String;)V:GetDefineEntityReplacementText_Ljava_lang_String_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeName:(I)Ljava/lang/String;:GetGetAttributeName_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeNamespace:(I)Ljava/lang/String;:GetGetAttributeNamespace_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributePrefix:(I)Ljava/lang/String;:GetGetAttributePrefix_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeType:(I)Ljava/lang/String;:GetGetAttributeType_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeValue:(I)Ljava/lang/String;:GetGetAttributeValue_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getAttributeValue:(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;:GetGetAttributeValue_Ljava_lang_String_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getFeature:(Ljava/lang/String;)Z:GetGetFeature_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getNamespace:(Ljava/lang/String;)Ljava/lang/String;:GetGetNamespace_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getNamespaceCount:(I)I:GetGetNamespaceCount_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getNamespacePrefix:(I)Ljava/lang/String;:GetGetNamespacePrefix_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getNamespaceUri:(I)Ljava/lang/String;:GetGetNamespaceUri_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getProperty:(Ljava/lang/String;)Ljava/lang/Object;:GetGetProperty_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_getTextCharacters:([I)[C:GetGetTextCharacters_arrayIHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_isAttributeDefault:(I)Z:GetIsAttributeDefault_IHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_next:()I:GetNextHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_nextTag:()I:GetNextTagHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_nextText:()Ljava/lang/String;:GetNextTextHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_nextToken:()I:GetNextTokenHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_require:(ILjava/lang/String;Ljava/lang/String;)V:GetRequire_ILjava_lang_String_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_setFeature:(Ljava/lang/String;Z)V:GetSetFeature_Ljava_lang_String_ZHandler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_setInput:(Ljava/io/InputStream;Ljava/lang/String;)V:GetSetInput_Ljava_io_InputStream_Ljava_lang_String_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_setInput:(Ljava/io/Reader;)V:GetSetInput_Ljava_io_Reader_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_setProperty:(Ljava/lang/String;Ljava/lang/Object;)V:GetSetProperty_Ljava_lang_String_Ljava_lang_Object_Handler:Org.XmlPull.V1.IXmlPullParserInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n"

    sput-object v0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->__md_methods:Ljava/lang/String;

    .line 51
    const-string v0, "Android.Runtime.XmlReaderPullParser, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-class v1, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;

    sget-object v2, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->__md_methods:Ljava/lang/String;

    invoke-static {v0, v1, v2}, Lmono/android/Runtime;->register(Ljava/lang/String;Ljava/lang/Class;Ljava/lang/String;)V

    .line 52
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
    .line 57
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    .line 58
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;

    if-ne v0, v1, :cond_0

    .line 59
    const-string v0, "Android.Runtime.XmlReaderPullParser, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-string v1, ""

    const/4 v2, 0x0

    new-array v2, v2, [Ljava/lang/Object;

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 60
    :cond_0
    return-void
.end method

.method private native n_defineEntityReplacementText(Ljava/lang/String;Ljava/lang/String;)V
.end method

.method private native n_getAttributeCount()I
.end method

.method private native n_getAttributeName(I)Ljava/lang/String;
.end method

.method private native n_getAttributeNamespace(I)Ljava/lang/String;
.end method

.method private native n_getAttributePrefix(I)Ljava/lang/String;
.end method

.method private native n_getAttributeType(I)Ljava/lang/String;
.end method

.method private native n_getAttributeValue(I)Ljava/lang/String;
.end method

.method private native n_getAttributeValue(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;
.end method

.method private native n_getColumnNumber()I
.end method

.method private native n_getDepth()I
.end method

.method private native n_getEventType()I
.end method

.method private native n_getFeature(Ljava/lang/String;)Z
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
.method public defineEntityReplacementText(Ljava/lang/String;Ljava/lang/String;)V
    .locals 0

    .prologue
    .line 169
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_defineEntityReplacementText(Ljava/lang/String;Ljava/lang/String;)V

    .line 170
    return-void
.end method

.method public getAttributeCount()I
    .locals 1

    .prologue
    .line 65
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getAttributeCount()I

    move-result v0

    return v0
.end method

.method public getAttributeName(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 177
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getAttributeName(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getAttributeNamespace(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 185
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getAttributeNamespace(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getAttributePrefix(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 193
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getAttributePrefix(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getAttributeType(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 201
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getAttributeType(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getAttributeValue(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 209
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getAttributeValue(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getAttributeValue(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;
    .locals 1

    .prologue
    .line 217
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getAttributeValue(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getColumnNumber()I
    .locals 1

    .prologue
    .line 73
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getColumnNumber()I

    move-result v0

    return v0
.end method

.method public getDepth()I
    .locals 1

    .prologue
    .line 81
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getDepth()I

    move-result v0

    return v0
.end method

.method public getEventType()I
    .locals 1

    .prologue
    .line 89
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getEventType()I

    move-result v0

    return v0
.end method

.method public getFeature(Ljava/lang/String;)Z
    .locals 1

    .prologue
    .line 225
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getFeature(Ljava/lang/String;)Z

    move-result v0

    return v0
.end method

.method public getInputEncoding()Ljava/lang/String;
    .locals 1

    .prologue
    .line 97
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getInputEncoding()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getLineNumber()I
    .locals 1

    .prologue
    .line 121
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getLineNumber()I

    move-result v0

    return v0
.end method

.method public getName()Ljava/lang/String;
    .locals 1

    .prologue
    .line 129
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getName()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getNamespace()Ljava/lang/String;
    .locals 1

    .prologue
    .line 137
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getNamespace()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getNamespace(Ljava/lang/String;)Ljava/lang/String;
    .locals 1

    .prologue
    .line 233
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getNamespace(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getNamespaceCount(I)I
    .locals 1

    .prologue
    .line 241
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getNamespaceCount(I)I

    move-result v0

    return v0
.end method

.method public getNamespacePrefix(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 249
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getNamespacePrefix(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getNamespaceUri(I)Ljava/lang/String;
    .locals 1

    .prologue
    .line 257
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getNamespaceUri(I)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getPositionDescription()Ljava/lang/String;
    .locals 1

    .prologue
    .line 145
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getPositionDescription()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getPrefix()Ljava/lang/String;
    .locals 1

    .prologue
    .line 153
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getPrefix()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getProperty(Ljava/lang/String;)Ljava/lang/Object;
    .locals 1

    .prologue
    .line 265
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getProperty(Ljava/lang/String;)Ljava/lang/Object;

    move-result-object v0

    return-object v0
.end method

.method public getText()Ljava/lang/String;
    .locals 1

    .prologue
    .line 161
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getText()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public getTextCharacters([I)[C
    .locals 1

    .prologue
    .line 273
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_getTextCharacters([I)[C

    move-result-object v0

    return-object v0
.end method

.method public isAttributeDefault(I)Z
    .locals 1

    .prologue
    .line 281
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_isAttributeDefault(I)Z

    move-result v0

    return v0
.end method

.method public isEmptyElementTag()Z
    .locals 1

    .prologue
    .line 105
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_isEmptyElementTag()Z

    move-result v0

    return v0
.end method

.method public isWhitespace()Z
    .locals 1

    .prologue
    .line 113
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_isWhitespace()Z

    move-result v0

    return v0
.end method

.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 361
    iget-object v0, p0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 362
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->refList:Ljava/util/ArrayList;

    .line 363
    :cond_0
    iget-object v0, p0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 364
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 368
    iget-object v0, p0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 369
    iget-object v0, p0, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 370
    :cond_0
    return-void
.end method

.method public next()I
    .locals 1

    .prologue
    .line 289
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_next()I

    move-result v0

    return v0
.end method

.method public nextTag()I
    .locals 1

    .prologue
    .line 297
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_nextTag()I

    move-result v0

    return v0
.end method

.method public nextText()Ljava/lang/String;
    .locals 1

    .prologue
    .line 305
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_nextText()Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public nextToken()I
    .locals 1

    .prologue
    .line 313
    invoke-direct {p0}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_nextToken()I

    move-result v0

    return v0
.end method

.method public require(ILjava/lang/String;Ljava/lang/String;)V
    .locals 0

    .prologue
    .line 321
    invoke-direct {p0, p1, p2, p3}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_require(ILjava/lang/String;Ljava/lang/String;)V

    .line 322
    return-void
.end method

.method public setFeature(Ljava/lang/String;Z)V
    .locals 0

    .prologue
    .line 329
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_setFeature(Ljava/lang/String;Z)V

    .line 330
    return-void
.end method

.method public setInput(Ljava/io/InputStream;Ljava/lang/String;)V
    .locals 0

    .prologue
    .line 337
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_setInput(Ljava/io/InputStream;Ljava/lang/String;)V

    .line 338
    return-void
.end method

.method public setInput(Ljava/io/Reader;)V
    .locals 0

    .prologue
    .line 345
    invoke-direct {p0, p1}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_setInput(Ljava/io/Reader;)V

    .line 346
    return-void
.end method

.method public setProperty(Ljava/lang/String;Ljava/lang/Object;)V
    .locals 0

    .prologue
    .line 353
    invoke-direct {p0, p1, p2}, Lmd52ce486a14f4bcd95899665e9d932190b/XmlReaderPullParser;->n_setProperty(Ljava/lang/String;Ljava/lang/Object;)V

    .line 354
    return-void
.end method
