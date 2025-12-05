Imports System.Configuration


Public Class clsNotes

    Dim l_intCodeLang As Integer
    Dim cLang As clsEGSLanguage

    Public Enum enumNotes
        Welcome = 500
        SelectionForMassMutation = 501
        CreateMerchandise = 502
        EditMerchandise = 503
        CreateRecipe = 504
        EditRecipe = 505
        CreateMenu = 506
        EditMenu = 507
        Approval = 508
        Request = 509
        CreateText = 510
        EditText = 511
        CreateProduct = 512
        EditProduct = 513
        CreateInventory = 514
        EditInventory = 515
    End Enum

    Private Sub Init()
        cLang = New clsEGSLanguage(l_intCodeLang)
    End Sub

    Public Sub SetLanguange(ByVal intCodeLang As Integer)
        cLang = New clsEGSLanguage(intCodeLang)
    End Sub

    Public Sub FillTexts(ByVal value As UserRightsFunction, ByRef strHeader As String, _
        ByRef strBody As String, Optional ByVal nOption As Integer = 0, Optional ByVal intLicense As Integer = 28)

        strHeader = String.Empty
        strBody = String.Empty
        Select Case value
            Case UserRightsFunction.AllowPrintShoppingList 'NBG 9.28.2015 add
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.AllowPrintShowppingList) 'NBG 9.28.2015 add
            Case UserRightsFunction.AllowPrintPriceList
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Print) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Price)
            Case UserRightsFunction.AllowPrintNutrientList
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Print) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Nutrient)
            Case UserRightsFunction.AllowPrintList
                If nOption = 0 Then
                    strHeader = cLang.GetString(clsEGSLanguage.CodeType.Print) 'VRP 23.10.2007
                Else
                    strHeader = cLang.GetString(clsEGSLanguage.CodeType.Print) & " - " & cLang.GetString(clsEGSLanguage.CodeType.List)
                End If
            Case UserRightsFunction.AllowPrintDetails
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Print) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Details)
            Case UserRightsFunction.AllowActivation
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Activate)
            Case UserRightsFunction.AllowMoveUp
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.MoveUp)
            Case UserRightsFunction.AllowMoveDown
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Movedown)
            Case UserRightsFunction.AllowUnExpose
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.DoNotExpose)
            Case UserRightsFunction.AllowCreate
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.New_)
            Case UserRightsFunction.AllowMerge
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Merge)
            Case UserRightsFunction.AllowDelete
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.DeleteExistingMarksFirst)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.deleteMarked)
            Case UserRightsFunction.AllowSort
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SortBy)
            Case UserRightsFunction.AllowStandardize
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Standardize)
            Case UserRightsFunction.AllowPurge
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Purge)
            Case UserRightsFunction.AllowTranslate
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Translation)
            Case UserRightsFunction.AllowExecuteActionMark
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.MoreActions)
                '& "..."
            Case UserRightsFunction.AllowEmail
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Email)
            Case UserRightsFunction.AllowExport
                'AGL 2012.11.13 - CWM-2171
                If intLicense = 28 Then 'USA
                    strHeader = cLang.GetString(clsEGSLanguage.CodeType.Export) & "-" & "Excel/TXC/XML" 'cLang.GetString(clsEGSLanguage.CodeType.Export)
                Else
                    strHeader = cLang.GetString(clsEGSLanguage.CodeType.Export)
                End If
            Case UserRightsFunction.AllowPrintList
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Print)
            Case UserRightsFunction.AllowSubmit 'DLS 17.11.08
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Submit) & " - " & cLang.GetString(clsEGSLanguage.CodeType.FORAPPROVAL)
            Case UserRightsFunction.AllowApproveSubmitted 'DLS 17.11.08
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Approve)
            Case UserRightsFunction.AllowSubmitToSystem
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SubmitForGlobalSharing)
            Case UserRightsFunction.AllowTransfer
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Transfer)
            Case UserRightsFunction.AllowPublishOnWeb
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Publishontheweb)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.MakeContentAvailableInKioskBrowser)
            Case UserRightsFunction.AllowConvertToSystem
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.CreateASystemCopy)
            Case UserRightsFunction.AllowExpose
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Global) & " (" & cLang.GetString(clsEGSLanguage.CodeType.ExposeToAllUsers) & ")"
            Case UserRightsFunction.AllowReplace
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.ReplaceIngredient)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ReplacedIngredientUsedInRecipesAndMenus)
            Case UserRightsFunction.AllowSaveMark
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SaveMarkedAs)
                strBody = String.Empty
            Case UserRightsFunction.AllowCreateMenuCard
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.AllowCreateMenuCard)
            Case UserRightsFunction.AllowModify
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Edit)
            Case UserRightsFunction.AllowMassUnpublish
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.DoNotPublish)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.MakeContentUnAvailableInKioskBrowser)
            Case UserRightsFunction.AllowCreateShoppingList
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.AddToShoppingList)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ShoppingListDescription)
            Case UserRightsFunction.AllowMassChangeBrand
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.MoveMarkedToANewBrand)
            Case UserRightsFunction.AllowMassChangeCategory
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.MoveMarkedItemsToNewCategory) 'JTOC 25.01.2013
            Case UserRightsFunction.AllowMassChangeSupplier
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.MoveToNewSupplier)
            Case UserRightsFunction.AllowMassChangeSource
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.MoveMarkedToANewSource)
            Case UserRightsFunction.AllowCopy
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Copy)
            Case UserRightsFunction.AllowTransfer
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Transfer)
            Case UserRightsFunction.AllowAssignKeyword
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Assignunassignkeywords)
            Case UserRightsFunction.AllowAllergenEncoding
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Assignunassignallergens)
            Case UserRightsFunction.AllowAssignOwner 'VRP 09.10.2007
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Ownership) '"Assign Owner" 'cLang.GetString(clsEGSLanguage.CodeType.assignowner)
            Case UserRightsFunction.AllowSharing 'VRP 15.10.2007
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Sharing)
            Case UserRightsFunction.AllowLinktoFinishedGood
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.FinishedGood)
            Case UserRightsFunction.AllowLinktoPOS
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SalesItem)
            Case UserRightsFunction.AllowDeactivate
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Deactivate)
            Case UserRightsFunction.AllowPrintActivate
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Activate) & "-" & cLang.GetString(clsEGSLanguage.CodeType.Print)
            Case UserRightsFunction.AllowPrintDeactivate
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Deactivate) & "-" & cLang.GetString(clsEGSLanguage.CodeType.Print)
            Case UserRightsFunction.AllowPrintLabel 'VRP 24.10.2007
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.PrintLabels)
            Case UserRightsFunction.AllowExportToExcel 'VRP 05.05.2008
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.ExporttoExcel)
            Case UserRightsFunction.AllowModifyMediaFile 'VRP 12.05.2008
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Add) & "/" & cLang.GetString(clsEGSLanguage.CodeType.Edit) & " " & "Media Files"
            Case UserRightsFunction.AllowHaccpEncoding 'VRP 15.05.2008
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.HACCP)
            Case UserRightsFunction.AllowCreateProtectedCopy    'MRC - 09.02.08 - For Protected copies.
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.CreateProtectedCopy)
                strBody = String.Empty
            Case UserRightsFunction.AllowProtect    'MRC - 09.02.08 - For Protected copies.
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Protect)
                strBody = String.Empty
            Case UserRightsFunction.AllowUnprotect    'MRC - 09.02.08 - For Protected copies.
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Unprotect)
                strBody = String.Empty
            Case UserRightsFunction.AllowCopyPrices 'VRP 26.03.2009
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Copy) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Price)
                'MRC - 05.14.09 - Additional Product Action Marks - Will change these to clsEGSLanguage.CodeType ASAP
            Case UserRightsFunction.AllowMassAddSupplier
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.AddSupplierProduct)
            Case UserRightsFunction.AllowMassChangeTax
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetTaxProduct)
            Case UserRightsFunction.AllowMassAddLocation
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.AddLocationProduct)
            Case UserRightsFunction.AllowMassChangeDefaultLocation
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetDefaultLocationProduct)
            Case UserRightsFunction.AllowMassChangeDefaultProductionLocation
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetDefaultProductionLocationProduct)
            Case UserRightsFunction.AllowMassChangeDefaultOutputLocation
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetDefaultOutputLocationProducts)
            Case UserRightsFunction.AllowMassChangeMinMaxQtyInStock
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetProductMinimumMaximumQuantityStock)
            Case UserRightsFunction.AllowMassChangeMinMaxQtyToOrder
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetProductMinimumMaximumQuantityOrder)
            Case UserRightsFunction.AllowMassChangeDefaultQtyToOrder
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetProductDefaultQuantityOrder)
            Case UserRightsFunction.AllowMassChangeInventory
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetProductInventories)
            Case UserRightsFunction.AllowMassChangeRawMaterial
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetProductRawMaterial)
            Case UserRightsFunction.AllowMassChangeUseInputOutput
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetProductUsedInputOutput)
            Case UserRightsFunction.AllowMassChangeAutomaticTransferToOutletBeforeAnOutput
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetProductAutomaticTransferOutletBeforeOutput)
            Case UserRightsFunction.AllowMassChangeExcludeFromAutomaticOutputOperation
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetProductExcludeAutomaticOutputOperation)
            Case UserRightsFunction.AllowMassAddTransferRequestAfterOutputOfSoldItems
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.CreateTransferRequestAfterOutputProduct)
            Case UserRightsFunction.AllowMassChangeDoNotLinkToCalcmenu2009
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.DoNotLinkToCalcmenu2009)
            Case UserRightsFunction.AllowMassSetLastSupplierUsedAsDefaultSupplier
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.SetProductLastSupplierDefaultSupplier)
            Case UserRightsFunction.AllowMassRemoveSupplier
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.RemoveSupplier)

                'MRC 06.08.09 - Product Admin Rights
            Case UserRightsFunction.AllowMassAddDetail
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.AddProductDetailForSite)
            Case UserRightsFunction.AllowMassRemoveDetail
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.RemoveProductDetailForSite)

                'JBB 12.02.2010 - 
            Case UserRightsFunction.AllowCompareRecipe
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Compare)
            Case UserRightsFunction.AllowVersion
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.New_)
            Case UserRightsFunction.AllowVersionCompare
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Version)
            Case UserRightsFunction.AllowCookbook
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.AssignUnassignProject)
            Case UserRightsFunction.AllowExporttoWord
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Export) '+ " Word"
            Case UserRightsFunction.AllowCookmode
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.CookMode)
            Case UserRightsFunction.AllowRatings
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.MovetoNewRating)
            Case UserRightsFunction.AllowMassRecipeandWebStatus, UserRightsFunction.AllowChangeStatus
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.MoveMarkedToNewRecipeStatus)
            Case UserRightsFunction.AllowMassRecipePlacement, UserRightsFunction.AllowPublication 'AGL 2013.08.14 - 7736
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.AssignPublication)
            Case UserRightsFunction.AllowExportListToCSV ' RBAJ-2012.08.20
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.ExportListToCSV)
            Case UserRightsFunction.AllowExportListToExcel ' RBAJ-2012.08.20
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.ExportListToExcel)
            Case UserRightsFunction.AllowExportListToWord ' RBAJ-2012.08.20
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.ExportListToWord)
            Case UserRightsFunction.AllowExportListToPDF ' RBAJ-2012.08.20
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.ExportListToPDF)
            Case UserRightsFunction.AllowBrandSite 'AGL 2013.12.26 - 10307 - Changed AllowMassChangeKiosk to AllowBrandsite 'UserRightsFunction.AllowMassChangeKiosk 'AGL 2012.10.25 - CWM-1772
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.AssignUnassignKiosk)
            Case UserRightsFunction.AllowMoveMarkedItems 'AMTLA 2012.10.22
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.MoveMarkedItems)
            Case UserRightsFunction.AllowPrintOrExport 'AMTLA 2012.10.22
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Print) & " / " & cLang.GetString(clsEGSLanguage.CodeType.Export)
            Case UserRightsFunction.AllowMoreActionSharing 'AMTLA 2012.10.24
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Sharing)
            Case UserRightsFunction.AllowViewHistoryLogs 'AGL 2013.11.29

            Case UserRightsFunction.AllowFinalAndVerified
                'strHeader = "Set to FINAL and VERIFIED"
                strHeader = cLang.GetString(176369)
            Case UserRightsFunction.AllowFinal
                'strHeader = "Set to FINAL"
                strHeader = cLang.GetString(160450).Replace("%s", cLang.GetString(173164))
            Case UserRightsFunction.AllowVerified
                'strHeader = "Set to VERIFIED"
                strHeader = cLang.GetString(160450).Replace("%s", cLang.GetString(172755))
        End Select
    End Sub

    Public Sub FillTexts(ByVal value As enumNotes, ByRef strHeader As String, ByRef strBody As String)

        Select Case value
            Case enumNotes.CreateText
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Text)
                strBody = String.Empty
            Case enumNotes.EditText
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Text)
                strBody = String.Empty
            Case enumNotes.Approval
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Approval)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ItemsWaitingForApproval)
            Case enumNotes.Request
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Requests)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ItemsWaitingForApproval)
            Case enumNotes.Welcome
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Welcome)
                strBody = String.Empty
            Case enumNotes.SelectionForMassMutation
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Confirm)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.MarkedItemsToBeProcessed)
            Case enumNotes.CreateMerchandise
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Create_A_New_Merchandise) 'AGL 2012.10.26 - CWM-1310
                strBody = String.Empty
            Case enumNotes.CreateRecipe
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.CreateANewRecipe)
                strBody = String.Empty
            Case enumNotes.CreateMenu
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.CreateANewMenu)
                strBody = String.Empty
            Case enumNotes.CreateProduct
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.CreateProduct)
                strBody = String.Empty
            Case enumNotes.EditMerchandise
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Modify_Merchandise) 'AGL 2012.10.30 - CWM-1937
                strBody = String.Empty
            Case enumNotes.EditRecipe
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.Modify_Recipe)
                strBody = String.Empty
            Case enumNotes.EditMenu
                strHeader = cLang.GetString(174285)
                strBody = String.Empty
            Case enumNotes.EditProduct
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.ModifyProduct)
                strBody = String.Empty
            Case enumNotes.CreateInventory
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.CreateInventory)
                strBody = String.Empty
            Case enumNotes.EditInventory
                strHeader = cLang.GetString(clsEGSLanguage.CodeType.ModifyInventory)
                strBody = String.Empty
        End Select
    End Sub

    Public Sub FillTexts(ByVal value As MenuType, ByRef strheader As String, ByRef strBody As String, Optional ByVal intLicense As Integer = 28)
        Dim cLi As New EgswKey.clsLicense
        Select Case value
            Case MenuType.Home
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Home)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.WelcomeTo).Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.CalcmenuWeb))
            Case MenuType.ContactUs
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Contact_Us)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ShouldYouHaveQuestions)
            Case MenuType.Merchandise
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Merchandises) 'AGL Merging 2012.09.19
                strBody = cLang.GetString(clsEGSLanguage.CodeType.MerchandiseDescription)
            Case MenuType.Product ' MRC Oct. 18, 2007
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Product)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ProductDescription)
            Case MenuType.Text
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Text)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.TextDescriptionMerch) 'AGL 2012.10.26 - CWM-1310
            Case MenuType.Recipe
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Recipe)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.CreateRecipeDescription)
            Case MenuType.Menu
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Menu)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.MenuDescriptionMerch) 'AGL 2012.10.26 - CWM-1310
            Case MenuType.Configuration
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Configuration)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ConfigurationDescription)
            Case MenuType.SalesItem
                strheader = cLang.GetString(clsEGSLanguage.CodeType.SalesItem)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.SalesItemDesc)
            Case MenuType.Options
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Options)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.CustomizeViewAndSettings)
            Case MenuType.Registration
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Registration)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.RegisterRecipeNetWeb)
            Case MenuType.ManageSystemPref
                strheader = cLang.GetString(clsEGSLanguage.CodeType.WebSiteProfile)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.CustomizeSiteNameANdThemes)
            Case MenuType.ManageApprovalPref
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ApprovalRouting)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ApprovalOfIngredientsRecipesAndOthers)
            Case MenuType.ManageEmailPref
                strheader = cLang.GetString(clsEGSLanguage.CodeType.SMTPSettings)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.SMTPSettingsDescriptin)
            Case MenuType.ManageIPBlockList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.BlockedIPList)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.SetMaxLoginAttempts)
            Case MenuType.ManagePrintProfile
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PrintProfile)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintProfileDescription)
            Case MenuType.ManageTranslate
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Translation)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.LanguageDescForTranslatingIngredientsRecipes)
            Case MenuType.ManageCurrency
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Currency)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.CurrencyDescription)
            Case MenuType.ManageSetOfPrice
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Set_Price)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.SetOfPriceDescIngredientsRecipes)
            Case MenuType.ManageMainGroups
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Property)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PropertiesDescription)
            Case MenuType.ManageGroups
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Site)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.SiteDescription)
            Case MenuType.ManageUsers
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Users)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.UsersDescription).Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.CalcmenuWeb))
            Case MenuType.ManageImagePref
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ImageProcessing)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ImageProcessing)
            Case MenuType.ManageBrand
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Brand)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.BrandDescriptionIngredients)
            Case MenuType.ManageMerchandiseCategory
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Category)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.CategoryDescriptionIngredientRecipe)
            Case MenuType.ManageMerchandiseKeywords
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Keyword)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.KeywordDescriptionIngredientRecipe)
            Case MenuType.ManageNutrients
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Nutrient)
                strBody = Replace(cLang.GetString(clsEGSLanguage.CodeType.NutrientDescriptionUpToNNutrientValues), "%c", ConfigurationSettings.AppSettings("NutrientValuesCount")) 'AGL Merging 2012.09.20
            Case MenuType.ManageNutrientRules
                strheader = cLang.GetString(clsEGSLanguage.CodeType.NutrientRules)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.NutrientRulesDescription)
            Case MenuType.ManageSuppliers
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Supplier)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.SuppliersDescription)
            Case MenuType.ManageUnits
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Units)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.UnitsDescriptionIngredientRecipe).Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.CalcmenuWeb))
            Case MenuType.ManageTax
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Tax_Rates)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.TaxDescription)
            Case MenuType.ManageRecipeCategory
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Category)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.CategoryDescriptionIngredientRecipe)
            Case MenuType.ManageRecipeKeywords
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Keyword)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.KeywordDescriptionIngredientRecipe)
            Case MenuType.ManageSources
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Source)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.SourceDescription)
            Case MenuType.ManageDefaultProcedure 'VRP 25.10.2007
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ProcedureTemplate)
                strBody = String.Empty
            Case MenuType.ManageProductionPlace
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ProductionPlace)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ProductionPlaceDetails)
            Case MenuType.ManageMenuCategory
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Category)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.CategoryDescriptionIngredientRecipe)
            Case MenuType.ManageMenuKeywords
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Keyword)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.KeywordDescriptionIngredientRecipe)
            Case MenuType.ManageImportListe
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Import)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ImportDescriptionIngredientRecipe)
            Case MenuType.ManageConversionRate
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Exchange_Rate)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ExchangeRateDescription)
            Case MenuType.TextPurge
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Purge)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PurgeTextDescription)
            Case MenuType.TextStandardize
                strheader = cLang.GetString(clsEGSLanguage.CodeType.StandardizeTexts)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.FormatAllTexts)
            Case MenuType.MerchandisePrintList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PrintList)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintIngredientListDesc)
            Case MenuType.MerchandisePrintDetails
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PrintDetails)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintIngredientDetailsDesc)
            Case MenuType.RecipePrintDetails
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PrintDetails)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintRecipeDetailsDesc)
            Case MenuType.RecipePrintList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PrintList)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintRecipeListDesc)
            Case MenuType.MenuPrintList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PrintMenuList)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintList)
            Case MenuType.MenuPrintDetails
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PrintDetails)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintMenuDetailsDesc)
            Case MenuType.Recipe_MenuCards
                strheader = cLang.GetString(clsEGSLanguage.CodeType.MenuDetails)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.MenuEngineeringEvaluateRecipe)
            Case MenuType.Menu_MenuCards
                strheader = cLang.GetString(clsEGSLanguage.CodeType.CreateMenuCard)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.MenuEngineeringEvaluateMenu)
            Case MenuType.ManageTranslate
            Case MenuType.RecipeSearch
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Search)
            Case MenuType.ProductSearch
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Search)
            Case MenuType.MenuSearch
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Search)
            Case MenuType.TextSearch
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Search)
            Case MenuType.Recipe_MenuCardSearch, MenuType.Menu_MenuCardSearch
                strheader = cLang.GetString(clsEGSLanguage.CodeType.LoadMenuCardsList)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.EditPreviewMenuCards)
            Case MenuType.RecipeMarks
                strBody = cLang.GetString(clsEGSLanguage.CodeType.Selection)
                strheader = cLang.GetString(clsEGSLanguage.CodeType.MarkedRecipes)
            Case MenuType.MerchandiseMarks
                strBody = cLang.GetString(clsEGSLanguage.CodeType.Selection)
                strheader = cLang.GetString(clsEGSLanguage.CodeType.MarkedMerchandse) 'AGL 2012.10.31
            Case MenuType.MenuMarks
                strBody = cLang.GetString(clsEGSLanguage.CodeType.Selection)
                strheader = cLang.GetString(clsEGSLanguage.CodeType.MarkedMenus)
            Case MenuType.ShoppingListRecipe, MenuType.ShoppingListeMenu
                strheader = cLang.GetString(clsEGSLanguage.CodeType.LoadListofShoppingListSaved)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ShoppingListDescription)
            Case MenuType.main
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Main)
                strBody = String.Empty
            Case MenuType.pictures
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Picture)
                strBody = String.Empty
            Case MenuType.nutrients
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Nutrition) 'cLang.GetString(clsEGSLanguage.CodeType.Nutrient) 'RJL - 7673 12-11-2013
                strBody = String.Empty
            Case MenuType.info1
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Information) 'AGL 2014.09.16 - changed from Info1
                strBody = String.Empty
            Case MenuType.keywords
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Keywords)
                strBody = String.Empty
            Case MenuType.translate
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Translation)
                strBody = String.Empty
            Case MenuType.sharing
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Sharing)
                strBody = String.Empty
            Case MenuType.sharingOwner
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ListOfOwners)
                strBody = String.Empty
            Case MenuType.Ingredient
                strheader = cLang.GetString(clsEGSLanguage.CodeType.MerchandiseAndProcedure) ' RBAJ-2012.10.08 'AGL 2012.09.21
                strBody = String.Empty
                ' Marvin Nov 23 2007 - for recipe encoding module
            Case MenuType.IngredientText
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Ingredient) & "/" & cLang.GetString(clsEGSLanguage.CodeType.Text) ' RBAJ-2012.10.08
                strBody = String.Empty
            Case MenuType.Calculate
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Calculation)
                strBody = String.Empty
            Case MenuType.Note
                'strheader = cLang.GetString(clsEGSLanguage.CodeType.Note)
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Procedure)
                strBody = String.Empty
            Case MenuType.Procedure
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Procedure)
                strBody = String.Empty
            Case MenuType.menuItems
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ItemsAndProcedure)
                strBody = String.Empty
            Case MenuType.SalesConfigList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Sales)
            Case MenuType.SystemConfigList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.System)
            Case MenuType.ToolsConfigList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Tools)
            Case MenuType.UsersConfigList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Accounts)
            Case MenuType.MerchandiseConfigList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Merchandise)
            Case MenuType.SecurityConfigList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Security)
            Case MenuType.RecipeConfigList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Recipe)
            Case MenuType.MenuConfigList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Menu)
            Case MenuType.ManageRoleRights
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Roles)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.CustomizeRights)
            Case MenuType.StandardizeBasic
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Standardize)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.StandardizeDescription)
            Case MenuType.PurgeBasic
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Purge)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PurgeItems)
            Case MenuType.Recalculate 'VRP 28.08.2007
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Recalculate)
                'strBody = String.Empty 'Comment by ADR 04.08.11
                strBody = cLang.GetString(clsEGSLanguage.CodeType.Recalculate) 'ADR 04.08.11
            Case MenuType.MonitorBreadcrumbs
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Breadcrumbs)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.MonitorBreadcrumbs)
            Case MenuType.ExportTCPOS
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Export)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ExportSalesItems)
            Case MenuType.CSVImportOption
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ImportIngredientCSV) & "(Service)"
                strBody = cLang.GetString(clsEGSLanguage.CodeType.CSVImportIngredientOptionDescription)
            Case MenuType.CSVImportTemp
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PendingCSVImportListIngredient)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PendingCSVImportIngredientListDescription)
            Case MenuType.UploadOption
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Upload)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.UploadConfigDefinition)
            Case MenuType.MerchandiseActions
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Actions)
                strBody = String.Empty
            Case MenuType.RecipeActions
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Actions)
                strBody = String.Empty
            Case MenuType.MenuActions
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Actions)
                strBody = String.Empty
            Case MenuType.TextActions
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Actions)
                strBody = String.Empty
            Case MenuType.allergen
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Allergens)
            Case MenuType.ManageAllergen
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Allergens)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.MerchandiseAllergenDescription)
            Case MenuType.ManageSetOfPriceSales
                strheader = cLang.GetString(clsEGSLanguage.CodeType.SellingSetofPrice)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.SellingsetofPriceDescription)
            Case MenuType.ManageTerminals
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Terminal)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ManageTerminalDesc)
            Case MenuType.ManagePOSImportConfig
                strheader = cLang.GetString(clsEGSLanguage.CodeType.POSImportConfig)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ManagePOSImportConfigDesc)
            Case MenuType.ManagePOSTempData
                strheader = cLang.GetString(clsEGSLanguage.CodeType.POSImportFailedData)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ManagePOSImportTempDesc)
            Case MenuType.LinkToPOS
                strheader = cLang.GetString(clsEGSLanguage.CodeType.SalesItem)
                strBody = String.Empty
                'Case MenuType.LinkToFinishedGood
                '    strheader = cLang.GetString(clsEGSLanguage.CodeType.FinishedGood)
                '    strBody = String.Empty
            Case MenuType.CMOnlineTerms
                strheader = cLang.GetString(clsEGSLanguage.CodeType.TermsAndCondition)
                strBody = String.Empty
            Case MenuType.CMOnlineLangPicture
                strheader = cLang.GetString(clsEGSLanguage.CodeType.MyProfile)
                strBody = String.Empty
            Case MenuType.CMOnlineSubscription
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Subscription)
                strBody = String.Empty
            Case MenuType.CMOnlineFAQ
                strheader = cLang.GetString(clsEGSLanguage.CodeType.FAQ)
                strBody = String.Empty
            Case MenuType.CMOnlineFeatures
                strheader = cLang.GetString(clsEGSLanguage.CodeType.CMOFeatures)
                strBody = String.Empty
            Case MenuType.CMOnlineDefaultPrintProfile
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PrintProfile)
                strBody = String.Empty
            Case MenuType.ManageBackupRestoreDbase
                strheader = cLang.GetString(clsEGSLanguage.CodeType.DatabaseMangement)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.BackupRestoreDatabase)
            Case MenuType.ManageBackupRestorePicture
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PicturesManagement)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.BackupRestorePictures)
            Case MenuType.ViewLicense
                strheader = cLang.GetString(clsEGSLanguage.CodeType.License)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.LicensesVersionModules)
            Case MenuType.Detail
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Details)
                strBody = String.Empty
            Case MenuType.LinkToProducts
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Product)
                strBody = String.Empty
            Case MenuType.ProductSupplier
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Supplier)
                strBody = String.Empty
            Case MenuType.ManageLocation
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Location)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ManageLocationDesc)
            Case MenuType.ManageIssuanceType
                strheader = cLang.GetString(clsEGSLanguage.CodeType.IssuanceType)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ManageIssuanceTypeDesc)
            Case MenuType.ManageClients
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Client)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ManageClientDesc)
            Case MenuType.ExportShopList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Export) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Shoppinglist)
                ''AMTLA 2017.01.03 CWA-15388
                If cLi.l_App = EgswKey.clsLicense.enumApp.Elvetino Then
                    strBody = "Elvetino (" & cLang.GetString(clsEGSLanguage.CodeType.Export) & ") " & cLang.GetString(clsEGSLanguage.CodeType.Shoppinglist)
                Else
                    strBody = cLang.GetString(clsEGSLanguage.CodeType.Export) & " " & cLang.GetString(clsEGSLanguage.CodeType.Shoppinglist)
                End If
            Case MenuType.ManageImages  'DLS Jan252007
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ImageManagement)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ImageManagement)
            Case MenuType.SalesTmp
                strheader = cLang.GetString(clsEGSLanguage.CodeType.SalesNotImported)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.SalesRecordsNotSuccessful)
            Case MenuType.SalesHistoryDetails
                strheader = cLang.GetString(clsEGSLanguage.CodeType.History)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ManageSalesHistoryDesc)
            Case MenuType.ManageSupplierContact
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Supplier) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Contact)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ManageSupplierContactDesc)
            Case MenuType.ManageSupplierGroup
                strheader = cLang.GetString(clsEGSLanguage.CodeType.SupplierGroup)
                strBody = String.Empty
            Case MenuType.ManageClientContact
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Client) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Contact)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ManageClientContactDesc)
            Case MenuType.SalesItemLinking
                strheader = cLang.GetString(clsEGSLanguage.CodeType.SalesItemLinking)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.SalesItemLinkingDesc)
            Case MenuType.ManageStudent 'VRP 30.04.2008
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Student)
                strBody = String.Empty
            Case MenuType.Haccp
                strheader = cLang.GetString(clsEGSLanguage.CodeType.HACCP)
                strBody = String.Empty
            Case MenuType.ShowStepOnProcedures ' MRC 16.06.08
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ShowStepsProcedure)
                strBody = String.Empty
            Case MenuType.ManageAutoNumber ' MRC 18.06.08
                strheader = cLang.GetString(clsEGSLanguage.CodeType.AutoNumbering)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.UsedToAutomaticallyGenerateAssignNumbersForMerchandiseAndRecipe) 'JTOC 30.05.2013
            Case MenuType.ManageProcedureStyles 'VRP 30.06.2008
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ProcedureStyles)
                strBody = String.Empty
            Case MenuType.ConvertDB 'VRP 16.09.2008
                strheader = cLang.GetString(clsEGSLanguage.CodeType.UpdateToLatestDatabaseVersion)
                strBody = String.Empty
            Case MenuType.BulkImportation
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ImportMerchandiseCSV)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ImportMerchandiseCSVSupplierNetwork)
            Case MenuType.BulkImportationRecipe
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ImportRecipes)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ImportXMLTXCTXS)
            Case MenuType.ManageCountry 'mrc - 05.26.09
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Country)
                strBody = String.Empty
            Case MenuType.ManageRegion 'mrc - 05.26.09
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Region)
                strBody = String.Empty
            Case MenuType.ManageSubRegion 'mrc - 05.26.09
                strheader = cLang.GetString(clsEGSLanguage.CodeType.SubRegion)
                strBody = String.Empty
            Case MenuType.ManageProducer 'mrc - 05.26.09
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Producer)
                strBody = String.Empty
            Case MenuType.ManageWineType 'mrc - 05.26.09
                strheader = cLang.GetString(clsEGSLanguage.CodeType.WineType)
                strBody = String.Empty
            Case MenuType.CSVExport 'AGL 2015.03.04 'MRC 11.09.10   -   Migros CSV Export for OST
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Export)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.Merchandise) & " " & cLang.GetString(clsEGSLanguage.CodeType.AND_) & " " & cLang.GetString(clsEGSLanguage.CodeType.Recipe)
            Case MenuType.Comments 'JRN 11.30.10 
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Comments)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.Comments)
            Case MenuType.Placement 'JBB 12.09.2010
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Publication) ' RBAJ-2012.10.08
                strBody = cLang.GetString(clsEGSLanguage.CodeType.Publication) ' RBAJ-2012.10.08
                'strheader = cLang.GetString(clsEGSLanguage.CodeType.Promotion)
                'strBody = cLang.GetString(clsEGSLanguage.CodeType.Promotion)
            Case MenuType.ImposedNutrient 'MRC 12.13.2010
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Nutrient)
                strBody = String.Empty
            Case MenuType.WorkFlow ' JBB 01.03.2011
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Status)
                strBody = String.Empty
            Case MenuType.RecipeTime ' JBB 01.18.2011
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Time)
                strBody = String.Empty
            Case MenuType.ManagePlacement  'JBB 01.26.2011
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Publication) ' RBAJ-2012.10.08
                'strheader = cLang.GetString(clsEGSLanguage.CodeType.Promotion)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.UsedToDefineInWhichPublicationsWebsitesOrEventsTheRecipesHaveBeenMadeAvailableTo) 'JTOC 28.05.2013
            Case MenuType.ManageBrandSite  ' JBB 02.04.2011
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Kiosk)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ExternalWebsitesThatWillDisplayTheRecipe) 'JTOC 28.05.2013
            Case MenuType.ManageProject ' JBB 02.08.2011
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Cookbook)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ThisGroupingIsUsedInViewingRecipesInAHierarchicalViewInTheRecipeList)    'JTOC 28.05.2013
            Case MenuType.BrandSite  ' JBB 02.11.2011
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Kiosk)
                strBody = String.Empty
            Case MenuType.Project ' JBB 02.11.2011
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Cookbook)
                strBody = String.Empty
            Case MenuType.Brand  ' JBB 02.11.2011

                strheader = cLang.GetString(clsEGSLanguage.CodeType.Brand)
                strBody = String.Empty
            Case MenuType.Attachment   ' JBB 04.28.2011
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Attachment)
                strBody = String.Empty
            Case MenuType.ManageNutrientSet  ' JBB 05.16.2012
                strheader = cLang.GetString(clsEGSLanguage.CodeType.NutrientSet)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.NutrientGroupForImposedNutrientValues) 'JTOC 28.05.2013
            Case MenuType.ManageTime    ' RDC 02.21.2013 Recipe Time management
                strheader = cLang.GetString(clsEGSLanguage.CodeType.RecipeTime)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.TheTimeForEachStageInPreparingTheRecipeUsersCanAssignTheseTimesToRecipe) 'JTOC 28.05.2013
            Case MenuType.ManageRoles 'AGL 2013.07.02 - for Nestlé
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Roles)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.RoleManagementDescription)
            Case MenuType.ManageDigitalAssets 'MRC 2013.07.05 Digital Asset Management
                strheader = cLang.GetString(clsEGSLanguage.CodeType.DigitalAsset)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.DigitalAssetManagementDescription)
            Case MenuType.ViewHistory 'AGL 2013.11.29
                strheader = cLang.GetString(clsEGSLanguage.CodeType.History)
                strBody = ""
            Case MenuType.ManageAlias
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Alias)
                strBody = ""
            Case MenuType.RecipeAbbreviatedPreparation 'AGL 2014.07.30
                strheader = cLang.GetString(clsEGSLanguage.CodeType.AbbreviatedPreparation)
            Case MenuType.ManagePasswordAndLogin 'AGL 2014.09.11
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PasswordAndLogin)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.EnforceStrongPasswordPolicy)
            Case MenuType.ManageProcedureTemplate 'WVM-2014.10.01
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ProcedureTemplate)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ProcedureTemplateDescription) 'ECAM 08.10.2015 - Added description for Procedure Template
            Case MenuType.ManagePrefix 'WVM-2014.11.18
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Prefix)
                strBody = cLang.GetString(172731)
            Case MenuType.Labels 'WVM-2015.03.24
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Labels)
                strBody = ""
            Case MenuType.RecipeLink 'WVM-2015.03.24
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Recipelink)
                strBody = ""
            Case MenuType.Declaration   'MKAM 2015.02.16
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Declaration)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.Description)
            Case MenuType.MenuPlan
                strheader = cLang.GetString(clsEGSLanguage.CodeType.MenuPlan)
            Case MenuType.MenuPlanPrint
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Print)
            Case MenuType.ShoppingListMenuPlan
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Shoppinglist)
            Case MenuType.ExportRecipeLabel 'Raqi Pinili 2015.12.23
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ExportRecipeLabel)
                strBody = ""
                'NBG 3.17.2016
            Case MenuType.ManageNotes
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Notes)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.NotesDescription)
            Case MenuType.ManagePackagingMethod
                strheader = cLang.GetString(clsEGSLanguage.CodeType.PackagingMethod)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PackagingMethodDescription)
            Case MenuType.ManageStorageInformation
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ConservationTemperature)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ConservationTemperatureDescription)
            Case MenuType.ManageSaleSite
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Shop)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ShopMigros)
            Case MenuType.AlternativeIngredient
                strheader = cLang.GetString(clsEGSLanguage.CodeType.AlternativeIngredient)
            Case MenuType.Supplier
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Supplier)
                'JOP 4-11-2016
            Case MenuType.ManageRecipeWorkflow
                strheader = cLang.GetString(clsEGSLanguage.CodeType.RecipeWorkflow)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.RecipeWorkflowDescription)
            Case MenuType.ProductionLocation
                strheader = cLang.GetString(clsEGSLanguage.CodeType.ProductionLocation)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.ProductionLocationDescription)
            Case MenuType.MenuPlanConfigList
                strheader = cLang.GetString(clsEGSLanguage.CodeType.MenuPlan)
            Case MenuType.ManageRestaurant
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Restaurant) & " / " & cLang.GetString(175335)
            Case MenuType.ManageMenuPlanCategory
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Category)
            Case MenuType.ManageSeason
                strheader = cLang.GetString(172557)
            Case MenuType.ManageTypeofService
                strheader = cLang.GetString(175347)
            Case MenuType.ManagePrinter
                strheader = cLang.GetString(clsEGSLanguage.CodeType.Printer)
                strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintersMigros)
        End Select
    End Sub

    Public Sub FillTexts(ByVal mnuType As MenuType, ByVal right As UserRightsFunction, ByRef strHeader As String, ByRef strBody As String, _
     Optional ByVal nAlternate As Integer = 0) 'VRP 06.03.2008

        Select Case mnuType
            Case MenuType.Merchandise
                Select Case right
                    Case UserRightsFunction.AllowCreate
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.New_)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.CreateMerchandiseDescription) 'AGL 2012.10.26 - CWM-1310
                    Case UserRightsFunction.AllowPrintList
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Print)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintMerchandiseListDesc) 'AGL 2012.10.26 - CWM-1310
                    Case UserRightsFunction.AllowPrintDetails
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.PrintDetails)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintIngredientDetailsDesc)
                    Case UserRightsFunction.AllowLoadMark, UserRightsFunction.AllowMarking
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.LoadASetOfMarks)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.ShowListOfSavedMarks)
                    Case UserRightsFunction.AllowExportListToExcel
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.ExportListToExcel)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.MerchandiseList)
                End Select

            Case MenuType.Recipe
                Select Case right
                    Case UserRightsFunction.AllowCreate
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.New_)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.CreateRecipeDescription)
                    Case UserRightsFunction.AllowPrintList
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Print)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintRecipeListDesc)
                    Case UserRightsFunction.AllowCreateShoppingList
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Shoppinglist)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.ShoppingListDescription)
                    Case UserRightsFunction.AllowLoadMark, UserRightsFunction.AllowMarking
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.LoadASetOfMarks)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.ShowListOfSavedMarks)
                    Case UserRightsFunction.AllowCookbook
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Cookbook)
                        strBody = String.Empty
                End Select

            Case MenuType.Menu
                Select Case right
                    Case UserRightsFunction.AllowCreate
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.New_)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.CreateMenuDescription)
                    Case UserRightsFunction.AllowPrintList
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Print)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintMenuList)
                    Case UserRightsFunction.AllowCreateShoppingList
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Shoppinglist)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.ShoppingListDescription)
                    Case UserRightsFunction.AllowLoadMark, UserRightsFunction.AllowMarking
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.LoadASetOfMarks)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.ShowListOfSavedMarks)
                End Select

            Case MenuType.Text
                Select Case right
                    Case UserRightsFunction.AllowCreate
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.New_)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.CreateTextDescription)
                    Case UserRightsFunction.AllowPurge
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Purge)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.PurgeTextDescription)
                    Case UserRightsFunction.AllowStandardize
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Standardize)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.StandardizeDescription)
                End Select

            Case MenuType.Product ' MRC Oct. 18, 2007
                Select Case right
                    Case UserRightsFunction.AllowCreate
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Create)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.CreateProduct)
                    Case UserRightsFunction.AllowPrintList
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.PrintList)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintIngredientListDesc)
                    Case UserRightsFunction.AllowPrintDetails
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.PrintDetails)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.PrintIngredientDetailsDesc)
                    Case UserRightsFunction.AllowLoadMark, UserRightsFunction.AllowMarking
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.LoadASetOfMarks)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.ShowListOfSavedMarks)

                End Select

            Case MenuType.SalesItem
                Select Case right
                    Case UserRightsFunction.AllowCreate
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Create)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.SalesItemAddDesc)
                    Case UserRightsFunction.AllowLoadMark, UserRightsFunction.AllowMarking
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.LoadASetOfMarks)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.ShowListOfSavedMarks)
                    Case UserRightsFunction.AllowLinktoPOS
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.LinkToProductRecipeMenu)
                        strBody = "XXX"
                End Select

            Case MenuType.MenuPlan
                Select Case right
                    Case UserRightsFunction.AllowCreate
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.New_)
                    Case UserRightsFunction.AllowSearch
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.MenuPlan)
                    Case UserRightsFunction.AllowPrintList
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Print)
                    Case UserRightsFunction.AllowCreateShoppingList
                        strHeader = cLang.GetString(clsEGSLanguage.CodeType.Shoppinglist)
                        strBody = cLang.GetString(clsEGSLanguage.CodeType.ShoppingListDescription)
                End Select
        End Select
    End Sub

    Public Sub FillTexts(ByVal value As enumEgswErrorCode, ByRef strMsg As String)
        Select Case value
            Case enumEgswErrorCode.CannotShareItems
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.ErrorInItemsSharing)
            Case enumEgswErrorCode.CannotSwitch
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.CannotSwitch)
            Case enumEgswErrorCode.CorruptConfig
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.CorruptConfig)
            Case enumEgswErrorCode.DuplicateExistsActive
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.already_exists)
            Case enumEgswErrorCode.DuplicateExistsInactive
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.already_exists)
            Case enumEgswErrorCode.ExecuteProcedure
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.ProcedureError)
            Case enumEgswErrorCode.FistTran
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.FistRan)
            Case enumEgswErrorCode.FK
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Item_Used)
            Case enumEgswErrorCode.GeneralError
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Failed)
            Case enumEgswErrorCode.InsufficientRoleLevel
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Youdonothaverightstoaccessthisfunction)
            Case enumEgswErrorCode.InvalidCodeList
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.InvalidCodeList)
            Case enumEgswErrorCode.InvalidCodeSite
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.InvalidCodeSite)
            Case enumEgswErrorCode.InvalidListType
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.InvalidListType)
            Case enumEgswErrorCode.InvalidRights
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Youdonothaverightstoaccessthisfunction)
            Case enumEgswErrorCode.InvalidStockType
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.InvalidStockType)
            Case enumEgswErrorCode.InvalidSupplier
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.InvalidSupplier)
            Case enumEgswErrorCode.InvalidTranMode
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.InvalidTranMode)
            Case enumEgswErrorCode.ItemClosed
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.ItemClosed)
            Case enumEgswErrorCode.ItemLocked
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.ItemLocked)
            Case enumEgswErrorCode.ItemNoInvent
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.ItemNoInventory)
            Case enumEgswErrorCode.ItemNoLocation
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.ItemNoLocation)
            Case enumEgswErrorCode.MergingMultipleGlobalItem
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.MergingMultipleGlobalItems)
            Case enumEgswErrorCode.NotExists
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Not_Available)
            Case enumEgswErrorCode.OK
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Ok)
            Case enumEgswErrorCode.OneItemNotDeleted
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.OneItemNotDeleted)
            Case enumEgswErrorCode.RequestInProcess
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.RequestInProcess)
            Case enumEgswErrorCode.RequestNotInProcess
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.RequestNotInProcess)
            Case enumEgswErrorCode.UsedAsDirectIO
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.UsedAsDirectIO)
            Case enumEgswErrorCode.NothingDone
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.NothingWasDone)
            Case enumEgswErrorCode.SiteHasNoUser
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.SiteHasNoUser)
            Case enumEgswErrorCode.SalesItemNumberExists
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.SalesItemNumberAlreadyExists)
            Case enumEgswErrorCode.MissingBrand
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Missing) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Brand)
            Case enumEgswErrorCode.MissingCategory
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Missing) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Category)
            Case enumEgswErrorCode.MissingKeyword
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Missing) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Keyword)
            Case enumEgswErrorCode.MissingSource
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Missing) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Source)
            Case enumEgswErrorCode.MissingSupplier
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Missing) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Supplier)
            Case enumEgswErrorCode.MissingUnit
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.Missing) & " - " & cLang.GetString(clsEGSLanguage.CodeType.Unit)
            Case enumEgswErrorCode.PromoCodeError
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.InvalidPromoCode)
            Case enumEgswErrorCode.NotApplicable 'DLS May192009
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.NotApplicable)
            Case enumEgswErrorCode.MerchandiseInUse 'AGL 2012.10.04 - CWM-1330
                strMsg = cLang.GetString(clsEGSLanguage.CodeType.ItemIsBeingUsed)

        End Select
    End Sub

    Public Sub New(ByVal intCodeLang As Integer)
        l_intCodeLang = intCodeLang
        Init()
    End Sub

    Public Sub New(ByVal value As UserRightsFunction, ByRef strHeader As String, ByRef strBody As String, ByVal intCodeLang As Integer, Optional ByVal intLicense As Integer = 28, Optional ByVal intOption As Integer = 0) 'JTOC 10.09.2012 Added intLicense parameter 'JTOC 30.05.2013 intOption CWM-6281 
        l_intCodeLang = intCodeLang
        Init()
        FillTexts(value, strHeader, strBody, nOption:=intOption, intLicense:=intLicense)
    End Sub

    Public Sub New(ByVal value As MenuType, ByRef strheader As String, ByRef strBody As String, ByVal intCodeLang As Integer)
        l_intCodeLang = intCodeLang
        Init()
        FillTexts(value, strheader, strBody)
    End Sub

    Public Sub New(ByVal value As enumNotes, ByRef strheader As String, ByRef strBody As String, ByVal intCodeLang As Integer)
        l_intCodeLang = intCodeLang
        Init()
        FillTexts(value, strheader, strBody)
    End Sub

    Public Sub New(ByVal value As enumEgswErrorCode, ByRef strMsg As String, ByVal intCodeLang As Integer)
        l_intCodeLang = intCodeLang
        Init()
        FillTexts(value, strMsg)
    End Sub

End Class
