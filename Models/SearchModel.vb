Namespace CalcmenuAPI.Models
    Public Class Searchee
        Private m_take As Integer
        Public Property take As Integer
            Get
                Return m_take
            End Get
            Set(value As Integer)
                m_take = value
            End Set
        End Property
        Private m_skip As Integer
        Public Property skip As Integer
            Get
                Return m_skip
            End Get
            Set(value As Integer)
                m_skip = value
            End Set
        End Property
        Private m_listview As Integer
        Public Property listview As Integer
            Get
                Return m_listview
            End Get
            Set(value As Integer)
                m_listview = value
            End Set
        End Property
        Private m_type As Integer
        Public Property type As Integer
            Get
                Return m_type
            End Get
            Set(value As Integer)
                m_type = value
            End Set
        End Property
        Private m_codeuser As Integer
        Public Property codeuser As Integer
            Get
                Return m_codeuser
            End Get
            Set(value As Integer)
                m_codeuser = value
            End Set
        End Property
        Private m_codesite As Integer
        Public Property codesite As Integer
            Get
                Return m_codesite
            End Get
            Set(value As Integer)
                m_codesite = value
            End Set
        End Property
        Private m_codetrans As Integer
        Public Property codetrans As Integer
            Get
                Return m_codetrans
            End Get
            Set(value As Integer)
                m_codetrans = value
            End Set
        End Property
        Private m_codesetprice As Integer
        Public Property codesetprice As Integer
            Get
                Return m_codesetprice
            End Get
            Set(value As Integer)
                m_codesetprice = value
            End Set
        End Property
        Private m_codeset As Integer
        Public Property codeset As Integer
            Get
                Return m_codeset
            End Get
            Set(value As Integer)
                m_codeset = value
            End Set
        End Property
        Private m_namefilter As Integer
        Public Property namefilter As Integer
            Get
                Return m_namefilter
            End Get
            Set(value As Integer)
                m_namefilter = value
            End Set
        End Property
        Private m_name As String
        Public Property name As String
            Get
                Return m_name
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property
        Private m_numberfilter As Integer
        Public Property numberfilter As Integer
            Get
                Return m_numberfilter
            End Get
            Set(value As Integer)
                m_numberfilter = value
            End Set
        End Property
        Private m_number As String
        Public Property number As String
            Get
                Return m_number
            End Get
            Set(value As String)
                m_number = value
            End Set
        End Property

        'NBG FILTER VITAMIX 
        Private m_timesfilter As Integer
        Public Property timesfilter As Integer
            Get
                Return m_timesfilter
            End Get
            Set(value As Integer)
                m_timesfilter = value
            End Set
        End Property
        Private m_time As Integer
        Public Property time As Integer
            Get
                Return m_time
            End Get
            Set(value As Integer)
                m_time = value
            End Set
        End Property
        Private m_datesfilter As Integer
        Public Property datesfilter As Integer
            Get
                Return m_datesfilter
            End Get
            Set(value As Integer)
                m_datesfilter = value
            End Set
        End Property

        Private m_brand As Integer
        Public Property brand As Integer
            Get
                Return m_brand
            End Get
            Set(value As Integer)
                m_brand = value
            End Set
        End Property
        Private m_supplier As Integer
        Public Property supplier As Integer
            Get
                Return m_supplier
            End Get
            Set(value As Integer)
                m_supplier = value
            End Set
        End Property
        Private m_brandsfilter As Integer
        Public Property brandsfilter As Integer
            Get
                Return m_brandsfilter
            End Get
            Set(value As Integer)
                m_brandsfilter = value
            End Set
        End Property
        Private m_brands As List(Of Models.GenericArrays)
        Public Property brands As List(Of Models.GenericArrays)
            Get
                Return m_brands
            End Get
            Set(value As List(Of Models.GenericArrays))
                m_brands = value
            End Set
        End Property
        Private m_primarybrandsfilter As Integer
        Public Property primarybrandsfilter As Integer
            Get
                Return m_primarybrandsfilter
            End Get
            Set(value As Integer)
                m_primarybrandsfilter = value
            End Set
        End Property
        Private m_primarybrands As List(Of Models.GenericArrays)
        Public Property primarybrands As List(Of Models.GenericArrays)
            Get
                Return m_primarybrands
            End Get
            Set(value As List(Of Models.GenericArrays))
                m_primarybrands = value
            End Set
        End Property
        Private m_unwantedprimarybrandsfilter As Integer
        Public Property unwantedprimarybrandsfilter As Integer
            Get
                Return m_unwantedprimarybrandsfilter
            End Get
            Set(value As Integer)
                m_unwantedprimarybrandsfilter = value
            End Set
        End Property
        Private m_unwantedprimarybrands As List(Of Models.GenericArrays)
        Public Property unwantedprimarybrands As List(Of Models.GenericArrays)
            Get
                Return m_unwantedprimarybrands
            End Get
            Set(value As List(Of Models.GenericArrays))
                m_unwantedprimarybrands = value
            End Set
        End Property
        Private m_secondarybrandsfilter As Integer
        Public Property secondarybrandsfilter As Integer
            Get
                Return m_secondarybrandsfilter
            End Get
            Set(value As Integer)
                m_secondarybrandsfilter = value
            End Set
        End Property
        Private m_secondarybrands As List(Of Models.GenericArrays)
        Public Property secondarybrands As List(Of Models.GenericArrays)
            Get
                Return m_secondarybrands
            End Get
            Set(value As List(Of Models.GenericArrays))
                m_secondarybrands = value
            End Set
        End Property
        Private m_unwantedsecondarybrandsfilter As Integer
        Public Property unwantedsecondarybrandsfilter As Integer
            Get
                Return m_unwantedsecondarybrandsfilter
            End Get
            Set(value As Integer)
                m_unwantedsecondarybrandsfilter = value
            End Set
        End Property
        Private m_unwantedsecondarybrands As List(Of Models.GenericArrays)
        Public Property unwantedsecondarybrands As List(Of Models.GenericArrays)
            Get
                Return m_unwantedsecondarybrands
            End Get
            Set(value As List(Of Models.GenericArrays))
                m_unwantedsecondarybrands = value
            End Set
        End Property
        Private m_keywordsfilter As Integer
        Public Property keywordsfilter As Integer
            Get
                Return m_keywordsfilter
            End Get
            Set(value As Integer)
                m_keywordsfilter = value
            End Set
        End Property
        Private m_keywords As List(Of Models.GenericArrays)
        Public Property keywords As List(Of Models.GenericArrays)
            Get
                Return m_keywords
            End Get
            Set(value As List(Of Models.GenericArrays))
                m_keywords = value
            End Set
        End Property
        Private m_unwantedkeywordsfilter As Integer
        Public Property unwantedkeywordsfilter As Integer
            Get
                Return m_unwantedkeywordsfilter
            End Get
            Set(value As Integer)
                m_unwantedkeywordsfilter = value
            End Set
        End Property
        Private m_unwantedkeywords As List(Of Models.GenericArrays)
        Public Property unwantedkeywords As List(Of Models.GenericArrays)
            Get
                Return m_unwantedkeywords
            End Get
            Set(value As List(Of Models.GenericArrays))
                m_unwantedkeywords = value
            End Set
        End Property
        Private m_category As Integer
        Public Property category As Integer
            Get
                Return m_category
            End Get
            Set(value As Integer)
                m_category = value
            End Set
        End Property
        Private m_recipestatus As Integer
        Public Property recipestatus As Integer
            Get
                Return m_recipestatus
            End Get
            Set(value As Integer)
                m_recipestatus = value
            End Set
        End Property
        Private m_image As Integer
        Public Property image As Integer
            Get
                Return m_image
            End Get
            Set(value As Integer)
                m_image = value
            End Set
        End Property
        Private m_allergensfilter As Integer
        Public Property allergensfilter As Integer
            Get
                Return m_allergensfilter
            End Get
            Set(value As Integer)
                m_allergensfilter = value
            End Set
        End Property
        Private m_allergens As List(Of Models.GenericArrays)
        Public Property allergens As List(Of Models.GenericArrays)
            Get
                Return m_allergens
            End Get
            Set(value As List(Of Models.GenericArrays))
                m_allergens = value
            End Set
        End Property
        Private m_unwantedallergensfilter As Integer
        Public Property unwantedallergensfilter As Integer
            Get
                Return m_unwantedallergensfilter
            End Get
            Set(value As Integer)
                m_unwantedallergensfilter = value
            End Set
        End Property
        Private m_unwantedallergens As List(Of Models.GenericArrays)
        Public Property unwantedallergens As List(Of Models.GenericArrays)
            Get
                Return m_unwantedallergens
            End Get
            Set(value As List(Of Models.GenericArrays))
                m_unwantedallergens = value
            End Set
        End Property
        Private m_fulltext As Boolean
        Public Property fulltext As Boolean
            Get
                Return m_fulltext
            End Get
            Set(value As Boolean)
                m_fulltext = value
            End Set
        End Property

        Private m_withoutallergens As Boolean
        Public Property withoutallergens As Boolean
            Get
                Return m_withoutallergens
            End Get
            Set(value As Boolean)
                m_withoutallergens = value
            End Set
        End Property
        Private m_withatleastoneallergen As Boolean
        Public Property withatleastoneallergen As Boolean
            Get
                Return m_withatleastoneallergen
            End Get
            Set(value As Boolean)
                m_withatleastoneallergen = value
            End Set
        End Property
        Private m_language As Integer
        Public Property language As Integer
            Get
                Return m_language
            End Get
            Set(value As Integer)
                m_language = value
            End Set
        End Property
        Private m_verified As Boolean
        Public Property verified As Boolean
            Get
                Return m_verified
            End Get
            Set(value As Boolean)
                m_verified = value
            End Set
        End Property
        Private m_nottranslated As Boolean
        Public Property nottranslated As Boolean
            Get
                Return m_nottranslated
            End Get
            Set(value As Boolean)
                m_nottranslated = value
            End Set
        End Property

        Private m_source As Integer
        Public Property source As Integer
            Get
                Return m_source
            End Get
            Set(value As Integer)
                m_source = value
            End Set
        End Property
        Private m_selfilter As Integer
        Public Property selfilter As Integer
            Get
                Return m_selfilter
            End Get
            Set(value As Integer)
                m_selfilter = value
            End Set
        End Property
        Private m_markeditems As Integer
        Public Property markeditems As Integer
            Get
                Return m_markeditems
            End Get
            Set(value As Integer)
                m_markeditems = value
            End Set
        End Property
        Private m_usedasingredient As Integer
        Public Property usedasingredient As Integer
            Get
                Return m_usedasingredient
            End Get
            Set(value As Integer)
                m_usedasingredient = value
            End Set
        End Property
        Private m_wantedmerchandisefilter As Integer
        Public Property wantedmerchandisefilter As Integer
            Get
                Return m_wantedmerchandisefilter
            End Get
            Set(value As Integer)
                m_wantedmerchandisefilter = value
            End Set
        End Property
        Private m_wantedmerchandise As String
        Public Property wantedmerchandise As String
            Get
                Return m_wantedmerchandise
            End Get
            Set(value As String)
                m_wantedmerchandise = value
            End Set
        End Property
        Private m_unwantedmerchandisefilter As Integer
        Public Property unwantedmerchandisefilter As Integer
            Get
                Return m_unwantedmerchandisefilter
            End Get
            Set(value As Integer)
                m_unwantedmerchandisefilter = value
            End Set
        End Property
        Private m_unwantedmerchandise As String
        Public Property unwantedmerchandise As String
            Get
                Return m_unwantedmerchandise
            End Get
            Set(value As String)
                m_unwantedmerchandise = value
            End Set
        End Property
        Private m_pricefilter As Integer
        Public Property pricefilter As Integer
            Get
                Return m_pricefilter
            End Get
            Set(value As Integer)
                m_pricefilter = value
            End Set
        End Property
        Private m_priceoption As Integer
        Public Property priceoption As Integer
            Get
                Return m_priceoption
            End Get
            Set(value As Integer)
                m_priceoption = value
            End Set
        End Property
        Private m_price1 As String 'gyg 07.15.2015
        Public Property price1 As String 'gyg 07.15.2015
            Get
                Return m_price1
            End Get
            Set(value As String)
                m_price1 = value
            End Set
        End Property
        Private m_price2 As String 'gyg 07.15.2015
        Public Property price2 As String 'gyg 07.15.2015
            Get
                Return m_price2
            End Get
            Set(value As String)
                m_price2 = value
            End Set
        End Property
        Private m_datefilter As Integer
        Public Property datefilter As Integer
            Get
                Return m_datefilter
            End Get
            Set(value As Integer)
                m_datefilter = value
            End Set
        End Property
        Private m_datefrom As DateTime
        Public Property datefrom As DateTime
            Get
                Return m_datefrom
            End Get
            Set(value As DateTime)
                m_datefrom = value
            End Set
        End Property
        Private m_dateto As DateTime
        Public Property dateto As DateTime
            Get
                Return m_dateto
            End Get
            Set(value As DateTime)
                m_dateto = value
            End Set
        End Property
        Private m_publication As Integer
        Public Property publication As Integer
            Get
                Return m_publication
            End Get
            Set(value As Integer)
                m_publication = value
            End Set
        End Property
        Private m_publicationfilter As Integer
        Public Property publicationfilter As Integer
            Get
                Return m_publicationfilter
            End Get
            Set(value As Integer)
                m_publicationfilter = value
            End Set
        End Property

        'AGL 2015.07.11 - added publication date filter
        Private m_publicationdatefilter As Integer
        Public Property publicationdatefilter As Integer
            Get
                Return m_publicationdatefilter
            End Get
            Set(value As Integer)
                m_publicationdatefilter = value
            End Set
        End Property

        Private m_publicationdatefrom As DateTime
        Public Property publicationdatefrom As DateTime
            Get
                Return m_publicationdatefrom
            End Get
            Set(value As DateTime)
                m_publicationdatefrom = value
            End Set
        End Property
        Private m_publicationdateto As DateTime
        Public Property publicationdateto As DateTime
            Get
                Return m_publicationdateto
            End Get
            Set(value As DateTime)
                m_publicationdateto = value
            End Set
        End Property
        Private m_kioskfilter As Integer
        Public Property kioskfilter As Integer
            Get
                Return m_kioskfilter
            End Get
            Set(value As Integer)
                m_kioskfilter = value
            End Set
        End Property
        Private m_kiosks As List(Of Models.GenericArrays)
        Public Property kiosks As List(Of Models.GenericArrays)
            Get
                Return m_kiosks
            End Get
            Set(value As List(Of Models.GenericArrays))
                m_kiosks = value
            End Set
        End Property

        Private m_wanteditems As String
        Public Property wanteditems As String
            Get
                Return m_wanteditems
            End Get
            Set(value As String)
                m_wanteditems = value
            End Set
        End Property
        Private m_unwanteditems As String
        Public Property unwanteditems As String
            Get
                Return m_unwanteditems
            End Get
            Set(value As String)
                m_unwanteditems = value
            End Set
        End Property
        Private m_initialLoad As Integer
        Public Property initialLoad As Integer
            Get
                Return m_initialLoad
            End Get
            Set(value As Integer)
                m_initialLoad = value
            End Set
        End Property
        Private m_status As Integer
        Public Property status As Integer
            Get
                Return m_status
            End Get
            Set(value As Integer)
                m_status = value
            End Set
        End Property
        Private m_issue As Integer
        Public Property issue As Integer
            Get
                Return m_issue
            End Get
            Set(value As Integer)
                m_issue = value
            End Set
        End Property
        Private m_issues As String
        Public Property issues As String
            Get
                Return m_issues
            End Get
            Set(value As String)
                m_issues = value
            End Set
        End Property
        Private m_season As Integer
        Public Property season() As Integer
            Get
                Return m_Season
            End Get
            Set(ByVal value As Integer)
                m_Season = value
            End Set
        End Property
        Private m_serverType As String
        Public Property serviceType() As String
            Get
                Return m_serverType
            End Get
            Set(ByVal value As String)
                m_serverType = value
            End Set
        End Property
    End Class

    Public Class GenericArrays
        Private m_key As Integer
        Public Property key As Integer
            Get
                Return m_key
            End Get
            Set(value As Integer)
                m_key = value
            End Set
        End Property
        Private m_title As String
        Public Property title As String
            Get
                Return m_title
            End Get
            Set(value As String)
                m_title = value
            End Set
        End Property
    End Class
End Namespace