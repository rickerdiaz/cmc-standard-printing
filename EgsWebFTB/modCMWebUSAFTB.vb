Public Module modCMWebUSAFTB
    'TO GENERATE: EXEC sp_CMWEBGenerateLanguage in EGS_DB Database: By DLS

'english
    Public Function FTBLow1USA(ByVal Code As Integer) As String
        Select Case Code
            Case 1080
                Return "Ingredient cost"
            Case 1081
                Return "Cost of goods"
            Case 1090
                Return "Selling price"
            Case 1145
                Return "Counter"
            Case 1146
                Return "In progress"
            Case 1260
                Return "Ingredient"
            Case 1280
                Return "Remark"
            Case 1290
                Return "Price"
            Case 1300
                Return "Wastage"
            Case 1310
                Return "Quantity"
            Case 1400
                Return "Menu"
            Case 1450
                Return "Category"
            Case 1480
                Return "Imposed price"
            Case 1485
                Return "Calculated price"
            Case 1500
                Return "Date"
            Case 1530
                Return "Unit missing"
            Case 1600
                Return "Modify Menu"
            Case 2430
                Return "&Choose from the list"
            Case 2700
                Return "Print menu list"
            Case 2780
                Return "Shopping list"
            Case 3057
                Return "Database"
            Case 3140
                Return "For"
            Case 3150
                Return "Percentage"
            Case 3161
                Return "Const."
            Case 3195
                Return "Recipe #"
            Case 3200
                Return "Chef"
            Case 3204
                Return "First Name"
            Case 3205
                Return "Name"
            Case 3206
                Return "Translation"
            Case 3215
                Return "Unit price"
            Case 3230
                Return "Picture"
            Case 3234
                Return "List"
            Case 3300
                Return "Menu Card"
            Case 3305
                Return "Reference name"
            Case 3306
                Return "Representative"
            Case 3320
                Return "Do you want to adjust the quantities to the new number of serving(s)?"
            Case 3460
                Return "&Password"
            Case 3680
                Return "Backup"
            Case 3685
                Return "Backup completed"
            Case 3721
                Return "Source"
            Case 3760
                Return "Import"
            Case 3800
                Return "Export"
            Case 4130
                Return "Free space on disk"
            Case 4185
                Return "Product-ID"
            Case 4755
                Return "Start importing"
            Case 4825
                Return "Recipes"
            Case 4832
                Return "Recipe"
            Case 4834
                Return "Recipe Ingredients"
            Case 4854
                Return "Minimum"
            Case 4855
                Return "Maximum"
            Case 4856
                Return "From"
            Case 4860
                Return "File name"
            Case 4862
                Return "Version"
            Case 4865
                Return "Users"
            Case 4867
                Return "Modify"
            Case 4870
                Return "Modify a user"
            Case 4877
                Return "Average"
            Case 4890
                Return "Type of file"
            Case 4891
                Return "Preview"
            Case 5100
                Return "Unit"
            Case 5105
                Return "Format"
            Case 5270
                Return "Ingredient list"
            Case 5350
                Return "Total"
            Case 5390
                Return "serving"
            Case 5500
                Return "Number"
            Case 5530
                Return "Imposed selling price"
            Case 5590
                Return "Ingredients"
            Case 5600
                Return "Preparation"
            Case 5610
                Return "Page"
            Case 5720
                Return "Amount"
            Case 5741
                Return "Gross"
            Case 5795
                Return "per serving"
            Case 5801
                Return "Profit"
            Case 5900
                Return "Ingredient category"
            Case 6000
                Return "Modify category"
            Case 6002
                Return "Name of category"
            Case 6055
                Return "Add text"
            Case 6390
                Return "Currency"
            Case 6416
                Return "Factor"
            Case 6470
                Return "Please wait"
            Case 7010
                Return "No"
            Case 7030
                Return "Printer"
            Case 7073
                Return "Browse"
            Case 7181
                Return "All"
            Case 7183
                Return "Marked"
            Case 7250
                Return "French"
            Case 7260
                Return "German"
            Case 7270
                Return "English"
            Case 7280
                Return "Italian"
            Case 7292
                Return "Japanese"
            Case 7296
                Return "Europe"
            Case 7335
                Return "All marks have been successfully deleted"
            Case 7570
                Return "Sunday"
            Case 7571
                Return "Monday"
            Case 7572
                Return "Tuesday"
            Case 7573
                Return "Wednesday"
            Case 7574
                Return "Thursday"
            Case 7575
                Return "Friday"
            Case 7576
                Return "Saturday"
            Case 7720
                Return "Packaging"
            Case 7725
                Return "Transportation"
            Case 7755
                Return "System"
            Case 8210
                Return "Calculation"
            Case 8220
                Return "Procedure"
            Case 8395
                Return "Add"
            Case 8397
                Return "Delete"
            Case 8514
                Return "New price"
            Case 8913
                Return "None"
            Case 8914
                Return "Decimal"
            Case 8990
                Return "or"
            Case 8994
                Return "Tools"
            Case 9030
                Return "Updating"
            Case 9070
                Return "Not allowed in the demo version"
            Case 9140
                Return "Switzerland"
            Case 9920
                Return "Description"
            Case 10103
                Return "Copy"
            Case 10104
                Return "Text"
            Case 10109
                Return "Options"
            Case 10116
                Return "Note"
            Case 10121
                Return "Search"
            Case 10125
                Return "Note"
            Case 10129
                Return "Selection"
            Case 10130
                Return "On hand"
            Case 10131
                Return "Input"
            Case 10132
                Return "Output"
            Case 10135
                Return "Style"
            Case 10140
                Return "Stock"
            Case 10363
                Return "Tax"
            Case 10369
                Return "Supplier number"
            Case 10370
                Return "In order"
            Case 10399
                Return "Deleted"
            Case 10417
                Return "Failed:"
            Case 10430
                Return "Location"
            Case 10431
                Return "Inventory"
            Case 10447
                Return "Order"
            Case 10468
                Return "Status"
            Case 10513
                Return "Discount"
            Case 10523
                Return "Tel."
            Case 10524
                Return "Fax"
            Case 10554
                Return "CCP Description"
            Case 10555
                Return "Cooling Time"
            Case 10556
                Return "Heating Time"
            Case 10557
                Return "Heating Degree/Temperature"
            Case 10558
                Return "Heating Mode"
            Case 10572
                Return "Nutrient"
            Case 10573
                Return "Info1"
            Case 10970
                Return "Print"
            Case 10990
                Return "Supplier"
            Case 11040
                Return "Restore completed"
            Case 11060
                Return "Directory"
            Case 11280
                Return "Registration"
            Case 12515
                Return "Barcode"
            Case 12525
                Return "Invalid date"
            Case 13060
                Return "Nutrients"
            Case 13065
                Return "Display nutrients"
            Case 13255
                Return "History"
            Case 14070
                Return "Font"
            Case 14090
                Return "Title"
            Case 14110
                Return "Footer"
            Case 14816
                Return "Replace with"
            Case 14819
                Return "Replace"
            Case 14884
                Return "Updated items"
            Case 15360
                Return "Marked Menus"
            Case 15504
                Return "Administrator"
            Case 15510
                Return "Password"
            Case 15615
                Return "Enter your password"
            Case 15620
                Return "Confirmation"
            Case 16010
                Return "Calculation"
            Case 18460
                Return "Saving in progress"
            Case 19330
                Return "Size"
            Case 20122
                Return "Company"
            Case 20200
                Return "Subrecipe"
            Case 20469
                Return "Specify the mailing method"
            Case 20530
                Return "Energy"
            Case 20703
                Return "Main"
            Case 20709
                Return "Units"
            Case 21550
                Return "No dishes found"
            Case 21570
                Return "Print a FAX form"
            Case 21600
                Return "of"
            Case 24002
                Return "Last order"
            Case 24011
                Return "of"
            Case 24016
                Return "Supplier"
            Case 24027
                Return "Calculate"
            Case 24028
                Return "Cancel"
            Case 24044
                Return "Both"
            Case 24050
                Return "New"
            Case 24068
                Return "Margin"
            Case 24075
                Return "Article number"
            Case 24085
                Return "Assign new"
            Case 24087
                Return "No Ingredient found"
            Case 24105
                Return "Display"
            Case 24121
                Return "Abbreviation"
            Case 24129
                Return "Transfer"
            Case 24150
                Return "Edit"
            Case 24152
                Return "Position"
            Case 24153
                Return "City"
            Case 24163
                Return "Default location"
            Case 24260
                Return "This supplier cannot be deleted"
            Case 24268
                Return "Deselect all"
            Case 24269
                Return "Select all"
            Case 24270
                Return "Back"
            Case 24271
                Return "Next"
            Case 24291
                Return "Subtotal"
            Case 26000
                Return "Continue"
            Case 26100
                Return "Product description"
            Case 26101
                Return "Cooking tip/Advise"
            Case 26102
                Return "Refinement"
            Case 26103
                Return "Storage"
            Case 26104
                Return "Yield/Productivity"
            Case 27000
                Return "Ref. name"
            Case 27020
                Return "Address"
            Case 27050
                Return "Phone number"
            Case 27055
                Return "Header Name"
            Case 27056
                Return "and"
            Case 27130
                Return "Payment"
            Case 27135
                Return "Expiry date"
            Case 27220
                Return "Hour"
            Case 27530
                Return "Rate"
            Case 28000
                Return "Error in operation"
            Case 28008
                Return "Invalid directory"
            Case 28420
                Return "No picture available"
            Case 28483
                Return "The record does not exist"
            Case 28655
                Return "No unit has been defined"
            Case 29170
                Return "Not available"
            Case 29771
                Return "Modify Ingredient"
            Case 30210
                Return "The operation failed"
            Case 30240
                Return "Code"
            Case 30270
                Return "not found"
            Case 31085
                Return "Updated successfully"
            Case 31098
                Return "Save"
            Case 31370
                Return "Food cost"
            Case 31375
                Return "FC"
            Case 31380
                Return "Main"
            Case 31462
                Return "Error"
            Case 31492
                Return "Our fax assistance service assures you a reply within one to 24 hours, depending on the problem encountered (except weekends)"
            Case 31700
                Return "Days"
            Case 31732
                Return "Menu plan"
            Case 31755
                Return "Results"
            Case 31758
                Return "To"
            Case 31769
                Return "sold"
            Case 31800
                Return "Day"
            Case 31860
                Return "Period"
            Case 51056
                Return "Product"
            Case 51086
                Return "Language"
            Case 51092
                Return "Unit"
            Case 51097
                Return "EGS Enggist && Grandjean Software SA"
            Case 51098
                Return "Pierre-a-Bot 92"
            Case 51099
                Return "2000 Neuchatel, Switzerland"
            Case 51123
                Return "Details"
            Case 51128
                Return "Recipe Name"
            Case 51129
                Return "Wanted Ingredients"
            Case 51130
                Return "Unwanted Ingredients"
            Case 51131
                Return "Category name"
            Case 51139
                Return "Wanted"
            Case 51157
                Return "Message"
            Case 51174
                Return "Importation Done"
            Case 51178
                Return "Please try again."
            Case 51198
                Return "Connecting to SMTP server"
            Case 51204
                Return "Yes"
            Case 51243
                Return "Margin"
            Case 51244
                Return "Top"
            Case 51245
                Return "Bottom"
            Case 51246
                Return "Left"
            Case 51247
                Return "Right"
            Case 51252
                Return "Download"
            Case 51257
                Return "E-mail"
            Case 51259
                Return "SMTP Server"
            Case 51261
                Return "Username"
            Case 51281
                Return "Ingredients for"
            Case 51294
                Return "Yield"
            Case 51311
                Return "Invalid Unit"
            Case 51323
                Return "Invalid value for 'Yield'"
            Case 51336
                Return "Unwanted"
            Case 51337
                Return "Main"
            Case 51353
                Return "Copyright Agreement"
            Case 51364
                Return "Do you accept the copyright agreement above and wish to proceed with the submission of the recipe?"
            Case 51373
                Return "Please enter all information regarding SMTP, POP, Username and Password"
            Case 51377
                Return "Send E-mail"
            Case 51392
                Return "Yield unit"
            Case 51402
                Return "Are you sure you want to delete"
            Case 51500
                Return "Shopping List Details"
            Case 51502
                Return "Shopping List"
            Case 51532
                Return "Print shopping list"
            Case 51907
                Return "&Show Details"
            Case 52012
                Return "Browse"
            Case 52110
                Return "The selected file will be imported"
            Case 52130
                Return "New recipe"
            Case 52150
                Return "Done"
            Case 52307
                Return "Close"
            Case 52960
                Return "Simple"
            Case 52970
                Return "Complete"
            Case 53250
                Return "Export Selection"
            Case 54210
                Return "Do not change anything"
            Case 54220
                Return "All upper case"
            Case 54230
                Return "All lower case"
            Case 54240
                Return "Capitalize first letter of each word"
            Case 54245
                Return "First letter capitalized"
            Case 54295
                Return "with"
            Case 54710
                Return "Selected Keywords"
            Case 54730
                Return "Keywords"
            Case 55011
                Return "Serving Size"
            Case 55211
                Return "Link"
            Case 55220
                Return "Qty"
            Case 56100
                Return "Your Name"
            Case 56130
                Return "Country"
            Case 56500
                Return "Dictionary"
            Case 101600
                Return "Modify Menu"
            Case 103150
                Return "Percentage"
            Case 103215
                Return "Unit price"
            Case 103305
                Return "Reference name"
            Case 103306
                Return "Representative"
            Case 104829
                Return "List of suppliers"
            Case 104835
                Return "Create a new product"
            Case 104836
                Return "Modify a Product"
            Case 104854
                Return "Minimum"
            Case 104855
                Return "Maximum"
            Case 104862
                Return "Version"
            Case 104869
                Return "New user"
            Case 104870
                Return "Modify a user"
            Case 105100
                Return "Unit"
            Case 105110
                Return "Date"
            Case 105200
                Return "for"
            Case 105360
                Return "Selling price by serving"
            Case 106002
                Return "Name of category"
            Case 107183
                Return "Marked"
            Case 109730
                Return "by"
            Case 110101
                Return "Modify"
            Case 110102
                Return "Delete"
            Case 110112
                Return "Print"
            Case 110114
                Return "Help"
            Case 110129
                Return "Selection"
            Case 110417
                Return "Failed:"
            Case 110447
                Return "Order"
            Case 110524
                Return "Fax"
            Case 113275
                Return "Tax"
            Case 115510
                Return "Password"
            Case 115610
                Return "New password accepted"
            Case 119130
                Return "Search"
            Case 121600
                Return "of"
            Case 124016
                Return "Supplier"
            Case 124024
                Return "Approved by"
            Case 124042
                Return "Type"
            Case 124164
                Return "Inventory adjustments"
            Case 124257
                Return "Outlet"
            Case 127010
                Return "Company"
            Case 127040
                Return "Country"
            Case 127050
                Return "Phone number"
            Case 127055
                Return "Header Name"
            Case 128000
                Return "Error in operation"
            Case 131462
                Return "Error"
            Case 131700
                Return "Days"
            Case 131757
                Return "From"
            Case 132541
                Return "Recipe"
            Case 132552
                Return "Total Tax"
            Case 132553
                Return "Imposed selling price + Tax"
            Case 132554
                Return "Modify Recipe"
            Case 132555
                Return "Add Recipe"
            Case 132557
                Return "Create A New Menu"
            Case 132559
                Return "Create A New Ingredient"
            Case 132561
                Return "Please enter Serial Number, Header Name and Product Key. You will find this information in the documentation provided with CALCMENU."
            Case 132565
                Return "Complement"
            Case 132567
                Return "Ingredient Category"
            Case 132568
                Return "Recipe Category"
            Case 132569
                Return "Menu Category"
            Case 132570
                Return "Unable to delete."
            Case 132571
                Return "Category is being used."
            Case 132586
                Return "Account Information"
            Case 132589
                Return "Maximum Number of Recipes"
            Case 132590
                Return "Current Number of Recipes"
            Case 132592
                Return "Maximum Number of Ingredient"
            Case 132593
                Return "Current Number of Ingredient"
            Case 132597
                Return "Create a New Recipe"
            Case 132598
                Return "Maximum Number of Menus"
            Case 132599
                Return "Current Number of Menus"
            Case 132600
                Return "Assign keyword"
            Case 132601
                Return "Move marked items to new category"
            Case 132602
                Return "Delete Marked"
            Case 132605
                Return "Shopping list"
            Case 132607
                Return "Action Marks"
            Case 132614
                Return "Net Qty"
            Case 132615
                Return "Rights"
            Case 132616
                Return "Owner"
            Case 132617
                Return "ALL CATEGORIES"
            Case 132621
                Return "Modify Source"
            Case 132630
                Return "Autoconversion"
            Case 132638
                Return "User Information"
            Case 132640
                Return "Username already being used."
            Case 132654
                Return "Database Management"
            Case 132657
                Return "&Restore"
            Case 132667
                Return "Merge"
            Case 132668
                Return "Purge"
            Case 132669
                Return "Move Up"
            Case 132670
                Return "Move Down"
            Case 132671
                Return "Standardize"
            Case 132672
                Return "Are you sure you want to delete %n?"
            Case 132678
                Return "HACCP"
            Case 132683
                Return "Previous"
            Case 132706
                Return "Nutrient values are per 100g or 100ml"
            Case 132708
                Return "No Supplier"
            Case 132714
                Return "Please select from the list."
            Case 132719
                Return "Price for same unit already defined."
            Case 132723
                Return "Total wastage cannot be greater than or equal to 100%."
            Case 132736
                Return "Gross Qty"
            Case 132737
                Return "Add New Supplier"
            Case 132738
                Return "Modify Supplier"
            Case 132739
                Return "Supplier Details"
            Case 132740
                Return "State"
            Case 132741
                Return "URL"
            Case 132779
                Return "Keyword being used."
            Case 132783
                Return "Keyword"
            Case 132788
                Return "Nutrient Linking"
            Case 132789
                Return "&Login"
            Case 132793
                Return "Invalid login name and/or password."
            Case 132813
                Return "&Configuration"
            Case 132828
                Return "Recalculate Nutrients"
            Case 132841
                Return "Add Ingredient"
            Case 132846
                Return "Save Marks"
            Case 132847
                Return "Load Marks"
            Case 132848
                Return "Filter"
            Case 132855
                Return "Add Menu"
            Case 132860
                Return "Add Ingredient"
            Case 132861
                Return "Modify Ingredient"
            Case 132864
                Return "Replace Ingredient"
            Case 132865
                Return "Add Separator"
            Case 132877
                Return "Add Item"
            Case 132896
                Return "Standardize Categories"
            Case 132900
                Return "Add Price"
            Case 132912
                Return "Standardize Texts"
            Case 132915
                Return "Standardize Units"
            Case 132924
                Return "Standardize Yield Units"
            Case 132930
                Return "Thumbnail"
            Case 132933
                Return "Recipe List"
            Case 132934
                Return "Last Recipe"
            Case 132937
                Return "Last Menu"
            Case 132939
                Return "Menu List"
            Case 132954
                Return "Set Of Marks"
            Case 132955
                Return "Choose a Mark Name from the list, or type in a new Mark Name to Save In"
            Case 132957
                Return "Save Marked As"
            Case 132967
                Return "Nutrient"
            Case 132971
                Return "Nutrient Summary"
            Case 132972
                Return "Nutrient values are per serving at 100%"
            Case 132974
                Return "Waste"
            Case 132987
                Return "Summary"
            Case 132989
                Return "Display"
            Case 132997
                Return "on or before"
            Case 132998
                Return "on or after"
            Case 132999
                Return "between"
            Case 133000
                Return "greater than"
            Case 133001
                Return "less than"
            Case 133005
                Return "Imposed"
            Case 133023
                Return "Display Options"
            Case 133043
                Return "Local Pictures Transformations"
            Case 133045
                Return "Maximum picture file size"
            Case 133046
                Return "Maximum picture size"
            Case 133047
                Return "Optimization"
            Case 133049
                Return "Activate auto-conversion of pictures for use on the Web site"
            Case 133057
                Return "Upload logo for the Website"
            Case 133060
                Return "Web Colors"
            Case 133075
                Return "New Password"
            Case 133076
                Return "Confirm New Password"
            Case 133078
                Return "Password do not match."
            Case 133080
                Return "Last"
            Case 133081
                Return "First"
            Case 133085
                Return "Document Output"
            Case 133096
                Return "Recipe Preparation"
            Case 133097
                Return "Recipe Costing"
            Case 133099
                Return "Variation"
            Case 133100
                Return "Recipe Details"
            Case 133101
                Return "Menu Details"
            Case 133108
                Return "What to print?"
            Case 133109
                Return "Selection of Ingredient to print"
            Case 133111
                Return "Some Categories"
            Case 133112
                Return "Marked Ingredient"
            Case 133115
                Return "All Recipes"
            Case 133116
                Return "Marked Recipes"
            Case 133121
                Return "Marked Menus"
            Case 133123
                Return "Menu Costing"
            Case 133124
                Return "Menu Description"
            Case 133126
                Return "EGS Standard"
            Case 133127
                Return "EGS Modern"
            Case 133128
                Return "EGS Two Columns"
            Case 133133
                Return "Invalid file name. Please enter a valid file name."
            Case 133144
                Return "Recipe #"
            Case 133147
                Return "liters"
            Case 133161
                Return "Paper Size"
            Case 133162
                Return "Unit for margins"
            Case 133163
                Return "Left Margin"
            Case 133164
                Return "Right Margin"
            Case 133165
                Return "Top Margin"
            Case 133166
                Return "Bottom Margin"
            Case 133168
                Return "Font Size"
            Case 133172
                Return "Small Picture / Quantity - Name"
            Case 133173
                Return "Small Picture / Name - Quantity"
            Case 133174
                Return "Medium Picture / Quantity - Name"
            Case 133175
                Return "Medium Picture / Name - Quantity"
            Case 133176
                Return "Large Picture / Quantity - Name"
            Case 133177
                Return "Large Picture / Name - Quantity"
            Case 133196
                Return "List Options"
            Case 133201
                Return "The following Ingredient are being used and are not deleted."
            Case 133207
                Return "Recipe can be used as subrecipe"
            Case 133208
                Return "Weight"
            Case 133222
                Return "Details Options"
            Case 133230
                Return "The following recipe(s) are being used and are not deleted."
            Case 133241
                Return "Recomputing Prices. Please wait..."
            Case 133242
                Return "Recomputing Nutrient Values. Please wait..."
            Case 133248
                Return "Ingredient"
            Case 133251
                Return "Separator"
            Case 133254
                Return "Sort by"
            Case 133260
                Return "Source being used."
            Case 133266
                Return "Standardize Keywords"
            Case 133286
                Return "Definition"
            Case 133289
                Return "Unit being used."
            Case 133290
                Return "You cannot merge two or more system units."
            Case 133295
                Return "This unit cannot be deleted. " & vbCrLf & "Only user-defined units can be deleted."
            Case 133314
                Return "Only user-defined yield units can be deleted."
            Case 133315
                Return "You cannot merge two or more system yield units."
            Case 133319
                Return "Yield unit being used."
            Case 133325
                Return "Are you sure you want to purge all unused categories?"
            Case 133326
                Return "No Source"
            Case 133328
                Return "Recipe Name"
            Case 133330
                Return "Missing file."
            Case 133334
                Return "Importing %r"
            Case 133349
                Return "Menu #"
            Case 133350
                Return "Items for %y (Net Quantity)"
            Case 133351
                Return "Ingredients for %y" ' at %p% (Net quantity)"
            Case 133352
                Return "Imposed selling price by serving + Tax"
            Case 133353
                Return "Imposed selling price by serving"
            Case 133359
                Return "Sorted by Number"
            Case 133360
                Return "Sorted by Date"
            Case 133361
                Return "Sorted by Category"
            Case 133365
                Return "Selling price + Tax"
            Case 133367
                Return "Sorted by Supplier"
            Case 133405
                Return "Upload Pictures"
            Case 133475
                Return "Image"
            Case 133519
                Return "Select a Color :"
            Case 133590
                Return "&Paste"
            Case 133692
                Return "Suggested price"
            Case 134021
                Return "Inventory Started On"
            Case 134032
                Return "Contact"
            Case 134054
                Return "Personal Information"
            Case 134055
                Return "Purchasing"
            Case 134056
                Return "Sales"
            Case 134061
                Return "Version, Modules & Licenses"
            Case 134083
                Return "Test"
            Case 134111
                Return "Unable to delete marked items."
            Case 134174
                Return "Date Created"
            Case 134176
                Return "Ingredient-Nutrient List"
            Case 134177
                Return "Recipe-Nutrient List"
            Case 134178
                Return "Menu-Nutrient List"
            Case 134182
                Return "Group"
            Case 134194
                Return "Invalid Quantity"
            Case 134195
                Return "Invalid Price"
            Case 134320
                Return "Billing Address"
            Case 134332
                Return "Info"
            Case 134333
                Return "Important"
            Case 134525
                Return "Are you sure you want to cancel the changes made?"
            Case 134571
                Return "Invalid value"
            Case 134826
                Return "Closed"
            Case 135024
                Return "Location"
            Case 135056
                Return "Nutrient Rules"
            Case 135058
                Return "Add Nutrient Rule"
            Case 135059
                Return "Modify Nutrient Rule"
            Case 135070
                Return "Net"
            Case 135100
                Return "Ref. Number"
            Case 135110
                Return "Quantity" & vbCrLf & "Inventory"
            Case 135235
                Return "Stock Value"
            Case 135256
                Return "Quantity Sold"
            Case 135257
                Return "Gross Margin"
            Case 135283
                Return "Last Price"
            Case 135608
                Return "Port"
            Case 135948
                Return "Include subrecipe(s)"
            Case 135951
                Return "Login failed."
            Case 135955
                Return "Invalid numeric value."
            Case 135963
                Return "Database"
            Case 135967
                Return "Replace in recipes."
            Case 135968
                Return "Replace in menus."
            Case 135969
                Return "Are you sure you want to replace %o?"
            Case 135971
                Return "&Connection"
            Case 135978
                Return "New"
            Case 135979
                Return "Rename"
            Case 135985
                Return "Existing"
            Case 135986
                Return "Missing"
            Case 135989
                Return "Items"
            Case 135990
                Return "Refresh"
            Case 136018
                Return "Ownership"
            Case 136025
                Return "Database conversion"
            Case 136030
                Return "Contents"
            Case 136100
                Return "Currently Opened Inventories"
            Case 136110
                Return "Opened On"
            Case 136115
                Return "# of Items"
            Case 136171
                Return "Change Unit"
            Case 136212
                Return "Show List of Adjustments Needed"
            Case 136213
                Return "Add a Product to the current Inventory"
            Case 136214
                Return "Delete a Product from the Inventory"
            Case 136215
                Return "Add a New Location for the Product"
            Case 136216
                Return "Delete the selected location for the product"
            Case 136217
                Return "Delete Quantity for the selected Product-Location"
            Case 136230
                Return "Create a New Inventory"
            Case 136231
                Return "Modify Inventory Info"
            Case 136265
                Return "Subrecipes"
            Case 136432
                Return "Invalid Code"
            Case 136601
                Return "Reset"
            Case 136905
                Return "Currency Symbol"
            Case 137019
                Return "Change"
            Case 137030
                Return "Default"
            Case 137070
                Return "General Settings"
            Case 138030
                Return "Select which products you want for this inventory."
            Case 138031
                Return "All Products for Inventories"
            Case 138032
                Return "Products from marked categories"
            Case 138033
                Return "Products from marked locations"
            Case 138034
                Return "Products from marked suppliers"
            Case 138035
                Return "Products from one or more previous inventories"
            Case 138137
                Return "Deleted"
            Case 138244
                Return "Sales item"
            Case 138402
                Return "All Transfers Successfully Done"
            Case 138412
                Return "<not defined>"
            Case 140056
                Return "File"
            Case 140100
                Return "Backup in progress"
            Case 140101
                Return "Restore in progress"
            Case 140129
                Return "Error while restoring a backup"
            Case 140130
                Return "Error while creating a backup"
            Case 140180
                Return "Path To Save Backup Files"
            Case 143001
                Return "Share"
            Case 143002
                Return "Unshare"
            Case 143003
                Return "Net" & vbCrLf & "Quantity"
            Case 143008
                Return "Waste"
            Case 143013
                Return "Modification"
            Case 143014
                Return "User"
            Case 143508
                Return "Recipe being used as a subrecipe"
            Case 143509
                Return "Line Spacing"
            Case 143981
                Return "Invalid Account Code"
            Case 143987
                Return "Item Type"
            Case 143995
                Return "Action"
            Case 144582
                Return "No Group"
            Case 144591
                Return "Time"
            Case 144682
                Return "Nutrient values are per 100g or 100 ml at 100%"
            Case 144684
                Return "Nutrient values are per 1 yield unit at 100%"
            Case 144685
                Return "per yield unit at 100%"
            Case 144686
                Return "per %Y at 100%"
            Case 144687
                Return "per 100g or 100ml at 100%"
            Case 144688
                Return "N/A"
            Case 144689
                Return "Nutrient values are per 1 yield unit/100g or 100 ml at 100%"
            Case 144716
                Return "History"
            Case 144734
                Return "Sales Item List"
            Case 144738
                Return "Weight per %Y"
            Case 145006
                Return "Transfer"
            Case 146043
                Return "January"
            Case 146044
                Return "February"
            Case 146045
                Return "March"
            Case 146046
                Return "April"
            Case 146047
                Return "May"
            Case 146048
                Return "June"
            Case 146049
                Return "July"
            Case 146050
                Return "August"
            Case 146051
                Return "September"
            Case 146052
                Return "October"
            Case 146053
                Return "November"
            Case 146054
                Return "December"
            Case 146056
                Return "Contribution Margin"
            Case 146067
                Return "Balance"
            Case 146080
                Return "Client(s)"
            Case 146114
                Return "Show to new page if different supplier"
            Case 146211
                Return "Issuance Type"
            Case 147070
                Return "OK"
            Case 147075
                Return "Invalid Date"
            Case 147126
                Return "Delete existing marks first"
            Case 147174
                Return "Open"
            Case 147381
                Return "Inventory Price Used for the Product Previously"
            Case 147441
                Return "This sales item has already been linked."
            Case 147462
                Return "Ratio"
            Case 147520
                Return "Main"
            Case 147647
                Return "SQL Server does not exist, or access denied"
            Case 147652
                Return "Delete"
            Case 147692
                Return "Meal Info"
            Case 147699
                Return "Overwrite"
            Case 147700
                Return "Total Price"
            Case 147703
                Return "Number of portions prepared"
            Case 147704
                Return "Yield Left"
            Case 147706
                Return "Yield Returned"
            Case 147707
                Return "Yield Lost"
            Case 147708
                Return "Yield Sold"
            Case 147710
                Return "Yield Sold Special"
            Case 147713
                Return "EGS Layout"
            Case 147727
                Return "Cost"
            Case 147729
                Return "Rating"
            Case 147733
                Return "Select a Language"
            Case 147737
                Return "Type Quantity and Select Unit"
            Case 147743
                Return "Upload"
            Case 147748
                Return "Anonymous"
            Case 147750
                Return "Comment"
            Case 147753
                Return "Labor Cost"
            Case 147771
                Return "Rate/Hr"
            Case 147772
                Return "Rate/Min"
            Case 147773
                Return "Person"
            Case 147774
                Return "Time (Hour:Minute)"
            Case 149501
                Return "Use Direct Input-Output"
            Case 149513
                Return "Approval"
            Case 149531
                Return "Finished Goods"
            Case 149645
                Return "Linked to"
            Case 149706
                Return "Delete link"
            Case 149761
                Return "Show"
            Case 149766
                Return "Prefix"
            Case 149774
                Return "Clear"
            Case 150009
                Return "Exportation Done. Recipe Successfully Exported."
            Case 150333
                Return "Successfully deleted!"
            Case 150341
                Return "Currency Conversion"
            Case 150353
                Return "Sort"
            Case 150634
                Return "E-mail successfully sent."
            Case 150644
                Return "The SMTP Server is needed to send e-mail from your computer."
            Case 150688
                Return "The license for this application has already expired."
            Case 150707
                Return "Account"
            Case 151011
                Return "Switzerland - Headquarter"
            Case 151019
                Return "Ingredient Keyword"
            Case 151020
                Return "Recipe Keyword"
            Case 151023
                Return "Register"
            Case 151250
                Return "Nothing was changed"
            Case 151286
                Return "Standard"
            Case 151299
                Return "Please enter the required information"
            Case 151322
                Return "Include in Inventory"
            Case 151336
                Return "Load a set of marks"
            Case 151344
                Return "Save marks for Ingredient"
            Case 151345
                Return "Save marks for dishes"
            Case 151346
                Return "Save marks for menus"
            Case 151364
                Return "Select two or more texts"
            Case 151389
                Return "Purge Texts"
            Case 151400
                Return "Ingredient Cost"
            Case 151404
                Return "VAT"
            Case 151424
                Return "Convert to best unit"
            Case 151427
                Return "Sorted by Item Name"
            Case 151435
                Return "Subject"
            Case 151436
                Return "Attachment"
            Case 151437
                Return "CALCMENU"
            Case 151438
                Return "CALCMENU"
            Case 151459
                Return "Your E-mail"
            Case 151499
                Return "Replace Proposal"
            Case 151500
                Return "Proposal"
            Case 151854
                Return "Excel"
            Case 151886
                Return "If you have any questions regarding your registration, please contact us at: %email"
            Case 151890
                Return "Hello %name"
            Case 151906
                Return "E-mail address not found"
            Case 151907
                Return "Please log in your username and password."
            Case 151910
                Return "Sign In"
            Case 151911
                Return "Sign Out"
            Case 151912
                Return "Forgot Your Password?"
            Case 151915
                Return "Please provide the information requested below."
            Case 151916
                Return "Fields with asterisks (*) are required."
            Case 151917
                Return "A confirmation e-mail will be sent to you."
            Case 151918
                Return "Please provide a valid e-mail address."
            Case 151920
                Return "Yes, I wish to receive periodical e-mail messages from EGS about new products or promotions (not more than once a month)."
            Case 151976
                Return "Default Production Location"
            Case 152004
                Return "Tree view"
            Case 152141
                Return "Ingredient Management"
            Case 152146
                Return "Zip"
            Case 155024
                Return "Pictures Management"
            Case 155046
                Return "Translation"
            Case 155050
                Return "ALL KEYWORDS"
            Case 155052
                Return "Submit"
            Case 155118
                Return "Send Shopping List to Pocket"
            Case 155163
                Return "Last Name"
            Case 155170
                Return "Welcome %name!"
            Case 155205
                Return "Home"
            Case 155225
                Return "PDF"
            Case 155236
                Return "Main Language"
            Case 155245
                Return "About Us"
            Case 155260
                Return "Imposed Factor"
            Case 155263
                Return "pixel"
            Case 155264
                Return "Translate"
            Case 155374
                Return "Accounting ID"
            Case 155507
                Return "Enable"
            Case 155575
                Return "Default Automatic Output Location"
            Case 155601
                Return "No Item selected."
            Case 155642
                Return "Recipe Exchange"
            Case 155654
                Return "Ingredients for %s %u at %p%  (Net quantity)"
            Case 155713
                Return "%r exists."
            Case 155731
                Return "CALCMENU Pro"
            Case 155761
                Return "Import Ingredient"
            Case 155763
                Return "Compare by Number"
            Case 155764
                Return "Compare by Name"
            Case 155811
                Return "Gross" & vbCrLf & "Quantity"
            Case 155841
                Return "File to Restore"
            Case 155842
                Return "Persons"
            Case 155861
                Return "Reset Quantity to Zero for Selected Items"
            Case 155862
                Return "per"
            Case 155926
                Return "Export to Excel"
            Case 155927
                Return "ALL SOURCES"
            Case 155942
                Return "Load Shopping Lists Saved"
            Case 155947
                Return "Filter By"
            Case 155967
                Return "Fields separator"
            Case 155994
                Return "Not Active"
            Case 155995
                Return "Checking..."
            Case 155996
                Return "E-mail Address"
            Case 156000
                Return "Move to a new supplier"
            Case 156012
                Return "Support"
            Case 156015
                Return "Contact Us"
            Case 156016
                Return "Main Office"
            Case 156060
                Return "Imposed FC"
            Case 156061
                Return "Imposed Profit"
            Case 156141
                Return "BackUp/Restore Database"
            Case 156337
                Return "Link Nutrient"
            Case 156344
                Return "Invalid Selection"
            Case 156355
                Return "Archives"
            Case 156356
                Return "Include"
            Case 156405
                Return "Please free some space then click Retry"
            Case 156413
                Return "Sub-Recipe Definition"
            Case 156485
                Return "Delete files after importation"
            Case 156542
                Return "Weighted Average Price"
            Case 156552
                Return "BackUp Now"
            Case 156590
                Return "Import Ingredient from CSV File (Excel)"
            Case 156669
                Return "Web site"
            Case 156672
                Return "Used online (for web content)"
            Case 156683
                Return "Original"
            Case 156720
                Return "Number too long"
            Case 156721
                Return "Name too long"
            Case 156722
                Return "Supplier too long"
            Case 156723
                Return "Category too long"
            Case 156725
                Return "Description too long"
            Case 156734
                Return "Two units are identical"
            Case 156742
                Return "Expires after"
            Case 156751
                Return "Direct line: +41 32 544 0017<br><br>24/7 English Customer Support: +1 800 964 9357<br>Sales: +41 848 000 357" & "<br>Fax: +41 32 753 0275"
            Case 156752
                Return "24/7 Toll Free: +1-800-964-9357"
            Case 156753
                Return "Office line +632 687 3179"
            Case 156754
                Return "Filename"
            Case 156784
                Return "Total Errors: %n"
            Case 156825
                Return "Thousand"
            Case 156870
                Return "Are you sure?"
            Case 156892
                Return "Download:"
            Case 156925
                Return "Downloaded OK!"
            Case 156938
                Return "Active"
            Case 156941
                Return "Pocket Kitchen"
            Case 156955
                Return "Private"
            Case 156957
                Return "Hotels"
            Case 156959
                Return "Shared"
            Case 156960
                Return "Submitted"
            Case 156961
                Return "Set Of Price"
            Case 156962
                Return "Not Submitted"
            Case 156963
                Return "Prices"
            Case 156964
                Return "Find in"
            Case 156965
                Return "Yields"
            Case 156966
                Return "Records affected"
            Case 156967
                Return "Please enter the correct date."
            Case 156968
                Return "Invalid image file format"
            Case 156969
                Return "Please enter the image file to upload. Otherwise, leave it blank."
            Case 156970
                Return "Enter Category Information"
            Case 156971
                Return "Enter Set Price Information"
            Case 156972
                Return "Enter Keyword Information"
            Case 156973
                Return "Enter Unit Information"
            Case 156974
                Return "Enter Yield Information"
            Case 156975
                Return "Create new recipes and submit to the main office for use with other hotels."
            Case 156976
                Return "Ingredient is the basic element or item that comprises your recipes."
            Case 156977
                Return "Should you have any inquiries or technical questions about this software."
            Case 156978
                Return "Parent Keyword"
            Case 156979
                Return "Name of Keyword"
            Case 156980
                Return "Configuration"
            Case 156981
                Return "Tax Rates"
            Case 156982
                Return "Search Results"
            Case 156983
                Return "Sorry, no results were found."
            Case 156984
                Return "Invalid username or password."
            Case 156986
                Return "The item already exists."
            Case 156987
                Return "was saved successfully."
            Case 156996
                Return "Copyright © 2004 of EGS Enggist & Grandjean Software SA, Switzerland."
            Case 157002
                Return "Price for the unit is not defined. Please select a unit."
            Case 157020
                Return "Tax used"
            Case 157026
                Return "Medium"
            Case 157033
                Return "The system will update the prices of all Ingredient. Please wait..."
            Case 157034
                Return "Authentication"
            Case 157038
                Return "Month"
            Case 157039
                Return "Year"
            Case 157040
                Return "There's no keyword available."
            Case 157041
                Return "Access denied"
            Case 157049
                Return "Are you sure you want to save?"
            Case 157055
                Return "STUDENT VERSION"
            Case 157056
                Return "Do you want to cancel?"
            Case 157057
                Return "Marked items are now shared."
            Case 157060
                Return "Reference Number"
            Case 157065
                Return "Export to CALCMENU"
            Case 157066
                Return "Export to CALCMENU"
            Case 157076
                Return "Help Summary"
            Case 157079
                Return "The following marked items are not submitted and cannot be transferred:"
            Case 157084
                Return "The following marked items are being used and are not deleted:"
            Case 157125
                Return "Views"
            Case 157130
                Return "Your credit card information has been sent successfully. Your subscription will be processed within three days. Thank you!"
            Case 157132
                Return "Personal (Shared)"
            Case 157133
                Return "Personal (Not Shared)"
            Case 157134
                Return "Visitor"
            Case 157136
                Return "Credits"
            Case 157139
                Return "Worst!"
            Case 157140
                Return "Good!"
            Case 157141
                Return "Fantastic!"
            Case 157142
                Return "Delete unused Ingredient units before import"
            Case 157151
                Return "Other links"
            Case 157152
                Return "User Reviews"
            Case 157153
                Return "The recipient will be prompted to accept these items."
            Case 157154
                Return "The following items cannot be given because they are owned by other users."
            Case 157155
                Return "Someone would like to give you the following recipes:"
            Case 157156
                Return "Promo"
            Case 157157
                Return "User Opinions"
            Case 157158
                Return "Originality"
            Case 157159
                Return "Result"
            Case 157160
                Return "Difficulty"
            Case 157161
                Return "Recipe of the day"
            Case 157164
                Return "Cardholder name"
            Case 157165
                Return "Credit card number"
            Case 157166
                Return "Record Limit"
            Case 157168
                Return "Bank"
            Case 157169
                Return "PayPal"
            Case 157170
                Return "Online ordering is not available in your country."
            Case 157171
                Return "Become a member"
            Case 157172
                Return "Upgrade fee"
            Case 157173
                Return "Subscription fee"
            Case 157174
                Return "Upgrade packs"
            Case 157176
                Return "Total records used"
            Case 157177
                Return "We offer a variety of solutions to fit your needs"
            Case 157178
                Return "Trial user"
            Case 157179
                Return "Tell a Friend"
            Case 157180
                Return "Friend's e-mail address"
            Case 157182
                Return "FAQs"
            Case 157183
                Return "Terms and Condition of Service"
            Case 157214
                Return "Create shopping list for marked recipes only"
            Case 157217
                Return "Create shopping list for marked menus only"
            Case 157226
                Return "Marked recipes have been sent for approval."
            Case 157233
                Return "Wastage cannot be greater than or equal to 100%."
            Case 157268
                Return "Currency used."
            Case 157269
                Return "Set of price is being used."
            Case 157273
                Return "Cannot share the following items because they were neither submitted nor owned."
            Case 157274
                Return "Exchange Rate"
            Case 157275
                Return "All items listed will be merged into one. Please select an item to be used by users. Other items will be deleted from the database."
            Case 157276
                Return "Successfully merged."
            Case 157277
                Return "Total Cost"
            Case 157281
                Return "Price of Default Supplier"
            Case 157297
                Return "Please select at least one item."
            Case 157299
                Return "Edit profile and customize your view."
            Case 157300
                Return "Please enter your new password. A password cannot exceed 20 characters. Click 'Submit' when you are done."
            Case 157301
                Return "Please enter the image file (jpeg/jpg , bmp, etc.) that you want to upload. Otherwise, leave it blank. (Note: GIF file is not supported. All pictures are copied and then converted to normal and thumbnail jpeg format. )"
            Case 157302
                Return "Search ingredient by name or a part of the name (use [*] asterisk). To add quickly, enter [net quantity]_[unit]_[ingredient] like 200 g Oel High Oleic"
            Case 157303
                Return "To add or edit the Ingredient price, enter the new price and define the unit of measurement. Assign the ratio of that unit to the original unit. For example, the original price and unit is US $11 per kilogram (kg). If you want to add the unit bag, you have to define the price of that bag, or define how many kilograms there are in 1 bag (ratio)."
            Case 157304
                Return "Search keywords by name or a part of the name. Use comma [ , ] for multiple keywords. For example, search ""beef, sauce, wedding""."
            Case 157305
                Return "Please select an item"
            Case 157306
                Return "Invalid file type."
            Case 157310
                Return "Ingredient Details"
            Case 157314
                Return "Use main/big unit when adding Ingredient price"
            Case 157320
                Return "Sharing"
            Case 157322
                Return "User Agreement"
            Case 157323
                Return "Give"
            Case 157329
                Return "Terminal"
            Case 157334
                Return "Warning: You might lose all your changes if another user has modified this record. Do you want to refresh this page?"
            Case 157336
                Return "Not Applicable"
            Case 157339
                Return "Messages per Page"
            Case 157340
                Return "Quick browse"
            Case 157341
                Return "on each page"
            Case 157342
                Return "Record was modified by another user.  Click OK to proceed."
            Case 157343
                Return "This record was deleted by another user."
            Case 157345
                Return "Submit to Head Office"
            Case 157346
                Return "Not shared"
            Case 157378
                Return "Member"
            Case 157379
                Return "Subscribe now"
            Case 157380
                Return "Your subscription will expire on %n."
            Case 157381
                Return "Your subscription has expired."
            Case 157382
                Return "Extend my membership using my remaining points (credits)"
            Case 157383
                Return "You've reached your disk space limit. Please delete some of your recipes or Ingredient. Thank you."
            Case 157384
                Return "Invalid transaction"
            Case 157385
                Return "Thank you!"
            Case 157387
                Return "You will be redirected to PayPal to complete your subscription. Please take a moment to choose which currency to use in order to charge you the correct amount. Please choose from the list below."
            Case 157388
                Return "An invitation to join"
            Case 157404
                Return "Pending transaction."
            Case 157405
                Return "For inquiries, please e-mail us at"
            Case 157408
                Return "Only members and trial users can access this page. Do you want to manage your own recipe in Recipe Gallery.com?  Go to the subscription menu and subscribe as a member."
            Case 157435
                Return "Automatic transfer to outlet before an output"
            Case 157437
                Return "Raw Material"
            Case 157446
                Return "Month(s)"
            Case 157515
                Return "Dutch"
            Case 157594
                Return "Accept"
            Case 157595
                Return "Deny"
            Case 157596
                Return "No User Review"
            Case 157604
                Return "E-mail Support"
            Case 157607
                Return "Phone Support"
            Case 157608
                Return "Online Support"
            Case 157616
                Return "USA"
            Case 157617
                Return "ASIA and the Rest of the World"
            Case 157629
                Return "Approve"
            Case 157633
                Return "Disapprove"
            Case 157659
                Return "Lock"
            Case 157660
                Return "Unlock"
            Case 157695
                Return "Accounting Ref."
            Case 157714
                Return "Comments"
            Case 157772
                Return "Optional"
            Case 157793
                Return "About"
            Case 157802
                Return "Confirm Password"
            Case 157901
                Return "Hide existing"
            Case 157926
                Return "Sign Up"
            Case 157985
                Return "You can always change your password by following these steps:"
            Case 157986
                Return "Sign in to EGS Web site at <a href='http://www.eg-software.com'>http://www.eg-software.com</a>."
            Case 157992
                Return "You recently requested the username and password to sign in to your EGS Login account."
            Case 157993
                Return "Please find details below"
            Case 158005
                Return "License"
            Case 158019
                Return "Check Request Status"
            Case 158157
                Return "Ingredients for %y"
            Case 158169
                Return "Kindly choose your payment terms." & vbCrLf & "" & vbCrLf & "Advance Payment via:"
            Case 158170
                Return "Kindly e-mail us your credit card details at <a href='mailto:info@calcmenu.com'>info@calcmenu.com</a>. Credit Card Type (Visa, Mastercard, American Express), Cardholder's Name, Credit Card Number (Please include the 3-digit security code (CVC2/CVV2) which you can find at the back of your card) and Expiry Date."
            Case 158171
                Return "Bank/Wire Transfer"
            Case 158174
                Return "Note: Please advise us once the transfer has been made. It will take 1-2 weeks before we receive our bank confirmation regarding the transfer."
            Case 158186
                Return "Change Password"
            Case 158216
                Return "Centralizing Recipe Management Anytime, Anywhere"
            Case 158220
                Return "Create new Ingredient name with up to 250 characters and include alphanumeric reference number, tax rate, four wastage percentages, category, supplier, and other helpful information such as product description, preparation, cooking tip, refinement methods, and storage."
            Case 158229
                Return "Pictures"
            Case 158230
                Return "Ingredient, Recipes can be searched using their name or reference numbers. You can also search using categories and keywords. For the Ingredient, you can also use supplier, date encoded or last modified, price range, and nutrient values when searching. For the recipes, you can search using items used and not used."
            Case 158232
                Return "Action Marks are shortcuts in performing a similar function that could apply to a marked Ingredient or recipe. You can use action marks to assign Ingredient, or recipe to a category and keywords, delete them, export, send via e-mail, print, share, and unshare to other users without having to repeat them for each item. This saves you a lot of time and effort in performing an action to the marked items."
            Case 158234
                Return "Nutrient Linking and Calculation"
            Case 158238
                Return "Supplier Management"
            Case 158240
                Return "Category, Keywords, Sources Management"
            Case 158243
                Return "Tax Rate Management"
            Case 158246
                Return "Unit Management"
            Case 158249
                Return "Printing, PDF and Excel Export"
            Case 158306
                Return "Select"
            Case 158346
                Return "more"
            Case 158349
                Return "Assigned keyword"
            Case 158350
                Return "Derived keyword"
            Case 158376
                Return "Theoretical imposed selling price"
            Case 158410
                Return "If some products do not have a defined price (price = 0), use the price of the default supplier instead."
            Case 158511
                Return "If you believe this is not the case, please send us an e-mail <a href='mailto:%email'>%email</a>"
            Case 158577
                Return "Site Language"
            Case 158585
                Return "Headoffice"
            Case 158588
                Return "Cannot submit the following items because they are owned by another user."
            Case 158653
                Return "Mobile"
            Case 158677
                Return "Sale Item" & vbCrLf & "number"
            Case 158694
                Return "Change Info"
            Case 158696
                Return "For Philippine Clients only"
            Case 158730
                Return "Exclude"
            Case 158734
                Return "The database version is not compatible with this version of the program."
            Case 158783
                Return "Include recipe(s)/subrecipe(s)"
            Case 158810
                Return "Calculate Price"
            Case 158835
                Return "Sorted by Tax"
            Case 158837
                Return "Sorted by Price"
            Case 158839
                Return "Sorted by Cost of Goods"
            Case 158840
                Return "Sorted by Constant"
            Case 158845
                Return "Sorted by Selling Price"
            Case 158846
                Return "Sorted by Imposed Price"
            Case 158849
                Return "High"
            Case 158850
                Return "Low"
            Case 158851
                Return "Created By"
            Case 158860
                Return "Modify POS System Settings"
            Case 158868
                Return "Chinese"
            Case 158902
                Return "Opening Time"
            Case 158912
                Return "Requests"
            Case 158935
                Return "Total Revenue"
            Case 158946
                Return "Set Quantity On Hand As Quantity Inventory"
            Case 158947
                Return "You will be redirected to Paypal to complete your order."
            Case 158952
                Return "Approved"
            Case 158953
                Return "Not Approved"
            Case 158960
                Return "This function has been disabled. Please contact your head office if you need new recipes."
            Case 158998
                Return "Search Features"
            Case 158999
                Return "Ingredient and recipe lists can be printed together with their details, prices, and nutrient values. Shopping lists or the list of ingredients together with cumulative quantities used in various recipes can also be printed. PDF and Excel files can also be created for the various reports."
            Case 159000
                Return "Set of Price and Multiple Currency Management"
            Case 159009
                Return "Border"
            Case 159035
                Return "Incomplete"
            Case 159064
                Return "Name cannot be blank"
            Case 159082
                Return "Update Products based on Last Date Modified"
            Case 159088
                Return "Send Request for Approval"
            Case 159089
                Return "Cancel Request for Approval"
            Case 159112
                Return "For Approval"
            Case 159113
                Return "Inheritable"
            Case 159133
                Return "Shipping Information"
            Case 159139
                Return "Composition"
            Case 159140
                Return "Unit too long"
            Case 159141
                Return "Unit %n does not exist."
            Case 159142
                Return "%n cannot be blank."
            Case 159144
                Return "Importing File. Please wait..."
            Case 159145
                Return "Saving Items. Please wait..."
            Case 159162
                Return "&Hide Details"
            Case 159168
                Return "Sorted by Net Quantity"
            Case 159169
                Return "Sorted by Gross Quantity"
            Case 159171
                Return "Schedule"
            Case 159181
                Return "Sorted by Amount"
            Case 159264
                Return "Import Ingredient CSV/Supplier Network"
            Case 159273
                Return "Total Contribution Margin"
            Case 159274
                Return "%number only"
            Case 159275
                Return "Limited by licenses"
            Case 159298
                Return "Menu Keyword"
            Case 159349
                Return "Reset Filter"
            Case 159350
                Return "Your Support and Update Plan has expired."
            Case 159360
                Return "Property Chef"
            Case 159361
                Return "Executive Chef"
            Case 159362
                Return "Selected item being used."
            Case 159363
                Return "Enter brand information"
            Case 159364
                Return "Brand"
            Case 159365
                Return "Role"
            Case 159366
                Return "Using SMTP on server"
            Case 159367
                Return "Using SMTP on the network"
            Case 159368
                Return "Logo"
            Case 159369
                Return "Compare by"
            Case 159370
                Return "successfully imported"
            Case 159372
                Return "Global"
            Case 159379
                Return "ascending"
            Case 159380
                Return "descending"
            Case 159381
                Return "Expose to all users"
            Case 159382
                Return "Convert to System Recipe"
            Case 159383
                Return "Do not expose"
            Case 159384
                Return "Property"
            Case 159385
                Return "Submit entry"
            Case 159386
                Return "Prices and nutrients were not recalculated."
            Case 159387
                Return "Prices and nutrients were recalculated."
            Case 159388
                Return "Create a New Menu Card"
            Case 159389
                Return "Modify Menu Card"
            Case 159390
                Return "E-mail sent."
            Case 159391
                Return "Approved Price"
            Case 159424
                Return "This function has been disabled. Please contact your head office if you need new Ingredient."
            Case 159426
                Return "Search ingredient by name or part of the name. To add quickly, enter [net quanitity]_[unit]_[ingredient]."
            Case 159430
                Return "Registration information has been successfully saved."
            Case 159433
                Return "Submit to System"
            Case 159434
                Return "Submitted to System"
            Case 159435
                Return "Move to a new category"
            Case 159436
                Return "E-mail Sender for System Alert Notifications"
            Case 159437
                Return "File was uploaded successfully."
            Case 159444
                Return "Impose Picture Size"
            Case 159445
                Return "Time Zone"
            Case 159446
                Return "Image Processing"
            Case 159457
                Return "SQL Server Full text search has the ability to perform complex queries against character data. Full Text Search allows searching of similar texts. For example, searching ""tomato"" will also yield ""tomatoes."" SQL 2009 provides the ranking of search results based on the matches in the name, note (or procedure), and ingredient of the query."
            Case 159458
                Return "Full population"
            Case 159459
                Return "Full text search"
            Case 159460
                Return "minute"
            Case 159461
                Return "Every"
            Case 159462
                Return "Run"
            Case 159463
                Return "Incremental Population"
            Case 159464
                Return "Language Word breaker"
            Case 159468
                Return "Used as ingredient"
            Case 159469
                Return "Not used as ingredient"
            Case 159471
                Return "IP Address"
            Case 159472
                Return "Blocked IP list"
            Case 159473
                Return "Block IP when login attempts reach"
            Case 159474
                Return "Please enter at least " & vbCrLf & " characters"
            Case 159485
                Return "Submit to Recipe Exchange"
            Case 159486
                Return "Submitted to Recipe Exchange"
            Case 159487
                Return "You have approved this recipe. It can now be seen by all users."
            Case 159488
                Return "Unknown Language"
            Case 159594
                Return "&Add to recipe"
            Case 159607
                Return "Standalone Recipe Management Software"
            Case 159608
                Return "Recipe Management Software for Concurrent Users in a Network"
            Case 159609
                Return "Web Based Recipe Management Software"
            Case 159610
                Return "Inventory and Back Office Management Software"
            Case 159611
                Return "Recipe Viewer for Pocket PC"
            Case 159612
                Return "Order Taking and Nutrient Monitoring Software"
            Case 159613
                Return "E-Cookbook Software"
            Case 159681
                Return "Recipe (%s) has too many ingredients. (Max. is %n)"
            Case 159689
                Return "Posted with picture."
            Case 159690
                Return "Posted without picture."
            Case 159699
                Return "Update Existing Items"
            Case 159700
                Return "&Import Recipe"
            Case 159707
                Return "France"
            Case 159708
                Return "Germany"
            Case 159733
                Return "Article No."
            Case 159751
                Return "Site"
            Case 159778
                Return "Advanced"
            Case 159779
                Return "Basic"
            Case 159782
                Return "Link Sales Items to Products"
            Case 159783
                Return "Link Sales Items to Recipes"
            Case 159795
                Return "POS Import - Configuration"
            Case 159918
                Return "You do not have rights to access this function."
            Case 159924
                Return "Manage"
            Case 159925
                Return "Invalid Conversion"
            Case 159929
                Return "Page Options"
            Case 159934
                Return "Nutrient Information"
            Case 159940
                Return "Export Updates"
            Case 159941
                Return "Export All"
            Case 159942
                Return "Output Directory"
            Case 159943
                Return "Quality"
            Case 159944
                Return "Parent"
            Case 159946
                Return "CALCMENU Web 2008"
            Case 159947
                Return "Select or upload file"
            Case 159949
                Return "Format should not exceed 10 characters."
            Case 159950
                Return "Nutrient name should not exceed 25 characters."
            Case 159951
                Return "Roles"
            Case 159962
                Return "Enter Tax Information"
            Case 159963
                Return "Enter Translation"
            Case 159966
                Return "Move marked items to new brand"
            Case 159967
                Return "Enter default site name:"
            Case 159968
                Return "Enter default Web site theme"
            Case 159969
                Return "Enable grouping sites by property to be managed by property admin:"
            Case 159970
                Return "Require users to submit information to the approver first before it can be used or published:"
            Case 159971
                Return "Enter the translation for each corresponding language or the default text will be used:"
            Case 159973
                Return "Select the sites that should belong to this property"
            Case 159974
                Return "Select available languages to use for translating Ingredient, recipes, and other information"
            Case 159975
                Return "Select one or more price groups to use for assigning prices to your Ingredient, recipe"
            Case 159976
                Return "Check the items to include"
            Case 159977
                Return "List of owners"
            Case 159978
                Return "Choose a format below"
            Case 159979
                Return "Choose basic list to purge"
            Case 159981
                Return "The following are the shared sites for this item"
            Case 159982
                Return "Move marked items to new source"
            Case 159987
                Return "Request Type"
            Case 159988
                Return "Requested by"
            Case 159990
                Return "Change brand"
            Case 159994
                Return "Replace ingredient in menus"
            Case 159997
                Return "Global Sharing"
            Case 160004
                Return "First Level"
            Case 160005
                Return "The selected ingredient should have the following units:"
            Case 160008
                Return "Step"
            Case 160009
                Return "More Actions"
            Case 160012
                Return "This recipe is published on the web."
            Case 160013
                Return "This recipe is not published on the web."
            Case 160014
                Return "Remember me"
            Case 160016
                Return "View Owners"
            Case 160018
                Return "This Ingredient is published on the web."
            Case 160019
                Return "This Ingredient is not published on the web."
            Case 160020
                Return "This Ingredient is exposed."
            Case 160021
                Return "This Ingredient is not exposed."
            Case 160023
                Return "For printing"
            Case 160028
                Return "Not to be published"
            Case 160030
                Return "Add to shopping list"
            Case 160033
                Return "Add keywords"
            Case 160035
                Return "You have attempted to login %n times"
            Case 160036
                Return "This account has been deactivated"
            Case 160037
                Return "Contact your system administrator to reactivate this account."
            Case 160038
                Return "My Profile"
            Case 160039
                Return "Last login"
            Case 160040
                Return "You are not signed in."
            Case 160041
                Return "Page Language"
            Case 160042
                Return "Main Translation"
            Case 160043
                Return "Main Set of Price"
            Case 160045
                Return "Rows Per Page"
            Case 160046
                Return "Default Display"
            Case 160047
                Return "Ingredient Quantities"
            Case 160048
                Return "Last accessed"
            Case 160049
                Return "Received '%f'"
            Case 160050
                Return "Length"
            Case 160051
                Return "Failed to receive '%f'"
            Case 160055
                Return "Quantity must be greater than 0."
            Case 160056
                Return "Create a new sub-recipe"
            Case 160057
                Return "Session has expired."
            Case 160058
                Return "Your login has expired due to inactivity for %n minutes."
            Case 160065
                Return "No name"
            Case 160066
                Return "Are you sure you want to close?"
            Case 160067
                Return "Your entry requires approval"
            Case 160068
                Return "Click the '%s' button to request approval."
            Case 160070
                Return "Marked items to be processed"
            Case 160071
                Return "This entry has been submitted for approval."
            Case 160072
                Return "There is already an existing request for this entry."
            Case 160074
                Return "Select unit"
            Case 160082
                Return "New requests await your approval."
            Case 160085
                Return "Your request has been reviewed."
            Case 160086
                Return "Print Nutrient List"
            Case 160087
                Return "Print List"
            Case 160088
                Return "Print Details"
            Case 160089
                Return "Activate"
            Case 160090
                Return "Create"
            Case 160091
                Return "Delete selected item from the list."
            Case 160093
                Return "Submit to System for global sharing"
            Case 160094
                Return "Make content available on kiosk browser"
            Case 160095
                Return "Create a System copy"
            Case 160096
                Return "Replace  ingredient used in recipes"
            Case 160098
                Return "Do not publish on the web"
            Case 160100
                Return "Create list of ingredients to be purchased"
            Case 160101
                Return "You can use text as ingredients that don't need quantity and price definitions."
            Case 160102
                Return "Create your own recipe database, share it with other users, print it, and even create a shopping list for it."
            Case 160103
                Return "Menu is a list of ingredients or recipes available in a meal."
            Case 160105
                Return "Organize basic information such as those related to users, suppliers, etc."
            Case 160106
                Return "Welcome"
            Case 160107
                Return "Welcome to %s"
            Case 160108
                Return "Customize your view and other settings."
            Case 160109
                Return "Website Profile"
            Case 160110
                Return "Customize Web site's name, themes, etc."
            Case 160111
                Return "Approval Routing"
            Case 160112
                Return "Approval of Ingredient, recipes, and other information."
            Case 160113
                Return "SMTP and Alert Notification Settings"
            Case 160114
                Return "Configure connection to your mail server; enable or disable alerts."
            Case 160115
                Return "Set maximum login attempts and monitor blocked IP addresses."
            Case 160116
                Return "Print Profile"
            Case 160117
                Return "Define multiple printing formats as profiles."
            Case 160118
                Return "Define list of languages for translating Ingredient, recipes, and other information."
            Case 160119
                Return "Available currencies for currency conversion and set of price definition."
            Case 160120
                Return "Work with Ingredient and recipes with multiple sets of prices."
            Case 160121
                Return "Properties are groups of sites."
            Case 160122
                Return "Sites organize users working together on a particular set of recipes."
            Case 160123
                Return "Manage users working on %s"
            Case 160124
                Return "Image Processing Preferences"
            Case 160125
                Return "Define standard picture size for Ingredient and recipes."
            Case 160130
                Return "Trademarks or distinctive names identifying Ingredient."
            Case 160132
                Return "Used to group Ingredient or recipes by common attributes."
            Case 160135
                Return "Keywords provide descriptive details to Ingredient or recipes. Users can assign multiple keywords per Ingredient or recipe."
            Case 160139
                Return "Define up to 34 nutrients values for nutrients like Energy, Carbohydrates, Proteins, and Lipids."
            Case 160141
                Return "Create rules that can be used as an additional filter for searching."
            Case 160151
                Return "List of predefined (or system) units used in defining Ingredient prices as well as in encoding recipes."
            Case 160152
                Return "Users can add to this list."
            Case 160153
                Return "Used in price calculation"
            Case 160154
                Return "Source refers to the origin of a particular recipe. It can be a chef, book, magazine, food service company, organization, or Web site."
            Case 160155
                Return "Import Ingredient or recipes from CALCMENU Pro, CALCMENU Enterprise, and other EGS products."
            Case 160156
                Return "Maintenance of exchange rate for different currencies"
            Case 160157
                Return "Delete unused texts."
            Case 160158
                Return "Format all texts."
            Case 160159
                Return "Print Ingredient list in HTML, Excel, PDF, and RTF formats."
            Case 160160
                Return "Print Ingredient details  in HTML, Excel, PDF, and RTF formats."
            Case 160161
                Return "Print recipe details  in HTML, Excel, PDF, and RTF formats."
            Case 160162
                Return "Print recipe list  in HTML, Excel, PDF, and RTF formats."
            Case 160163
                Return "Print menu details  in HTML, Excel, PDF, and RTF formats."
            Case 160164
                Return "Menu engineering allows you to evaluate current and future recipe pricing and design. Analyze menus and individual menu items to achieve optimum profit. Use Menu Engineering to identify which menu items to retain or drop from your menu."
            Case 160169
                Return "Load Menu Cards List"
            Case 160170
                Return "Modify or preview saved menu cards."
            Case 160175
                Return "Modify, preview or print saved shopping lists."
            Case 160177
                Return "Security"
            Case 160180
                Return "Standardize format of the items"
            Case 160181
                Return "Purge items"
            Case 160182
                Return "Role Rights"
            Case 160184
                Return "TCPOS Export"
            Case 160185
                Return "Export sales item"
            Case 160187
                Return "Create new local Ingredient that can be used as ingredient for your recipes."
            Case 160188
                Return "Show list of saved marks"
            Case 160189
                Return "Show list of items to be purchased."
            Case 160190
                Return "Create your own menus based on the available recipes in your database."
            Case 160191
                Return "Create a text used for recipes and menus."
            Case 160200
                Return "Sorted by Name"
            Case 160202
                Return "Choose from the list"
            Case 160209
                Return "Please enter Serial Number, Header Name and Product Key. You will find this information in the documentation provided with %s."
            Case 160210
                Return "Wanted Items"
            Case 160211
                Return "Unwanted Items"
            Case 160212
                Return "Drafts"
            Case 160217
                Return "Archive Path"
            Case 160218
                Return "Import Ingredient Data with Errors"
            Case 160219
                Return "Pending List of Ingredient that needs to be fixed"
            Case 160220
                Return "Define options for Ingredient import"
            Case 160232
                Return "Export to"
            Case 160237
                Return "Bold"
            Case 160254
                Return "Please restart the windows service %n for your changes to take effect."
            Case 160258
                Return "Currency does not match the chosen set of price."
            Case 160259
                Return "Name or number already exists."
            Case 160260
                Return "Date Imported"
            Case 160262
                Return "Nutrient values are per 1 yield unit"
            Case 160292
                Return "Allergens"
            Case 160293
                Return "List of food allergies or sensitivities associated to Ingredient."
            Case 160295
                Return "This account is currently in use. Please try again later."
            Case 160353
                Return "Purchasing Set of Price"
            Case 160354
                Return "Selling Set of Price"
            Case 160414
                Return "Qty Prev." & vbCrLf & "Inventory"
            Case 160423
                Return "Standalone Recipe Management Software"
            Case 160433
                Return "Consumption within"
            Case 160500
                Return "Text Management"
            Case 160687
                Return "Alternating Item Color"
            Case 160688
                Return "Normal Item Color"
            Case 160690
                Return "Please note that when you restore, it will automatically cut-off users currently using the System."
            Case 160691
                Return "Backup/Restore Pictures"
            Case 160716
                Return "Set items to Global by default"
            Case 160774
                Return "Deactivate"
            Case 160775
                Return "Delete trailing zeroes"
            Case 160776
                Return "Go back to %s"
            Case 160777
                Return "Click here to learn more about CALCMENU."
            Case 160788
                Return "Selected item(s) has been activated."
            Case 160789
                Return "Selected item(s) has been deactivated."
            Case 160790
                Return "Are you sure you want to delete selected item(s)?"
            Case 160791
                Return "Selected item(s) has been successfully deleted."
            Case 160801
                Return "You can only merge two or more similar recipes."
            Case 160802
                Return "Are you sure you want to merge selected items?"
            Case 160803
                Return "Are you sure you want to purge items?"
            Case 160804
                Return "Please fill out the required fields."
            Case 160805
                Return "Select two or more items to merge."
            Case 160806
                Return "Are you sure you want to deactivate selected item(s)?"
            Case 160863
                Return "Ingredient Price List"
            Case 160880
                Return "Recalculate"
            Case 160894
                Return "Silver"
            Case 160940
                Return "Effectivity Date"
            Case 160941
                Return "Linked Sales Item"
            Case 160953
                Return "Factor of Selling Set of Price to Purchasing Set of Price"
            Case 160958
                Return "Work with sales item with multiple selling sets of prices."
            Case 160985
                Return "Not Linked Sales Item"
            Case 160987
                Return "Create sales items and link it to existing recipes."
            Case 160988
                Return "Sales item is used in selling and it is usually linked to a recipe."
            Case 161028
                Return "Are you sure you want to change the nutrient database? This action will change the nutrient definitions you have already set in your Ingredient."
            Case 161029
                Return "Either the Yields or Ingredients check box must be selected."
            Case 161049
                Return "Force deletion of keyword and its sub-keywords"
            Case 161050
                Return "Deleted keywords will also be unassigned from Ingredient/recipe items."
            Case 161051
                Return "Selected keywords and all its sub-keywords are successfully deleted. Deleted keywords are now also unassigned from Ingredient and recipe items."
            Case 161078
                Return "Exact"
            Case 161079
                Return "Starts with"
            Case 161080
                Return "Contains"
            Case 161082
                Return "Second"
            Case 161083
                Return "Third"
            Case 161084
                Return "Fourth"
            Case 161085
                Return "One time only"
            Case 161086
                Return "Daily"
            Case 161087
                Return "Weekly"
            Case 161088
                Return "Monthly"
            Case 161089
                Return "When file changes"
            Case 161090
                Return "When the computer starts"
            Case 161091
                Return "Enter %s information"
            Case 161092
                Return "Supplier Group"
            Case 161093
                Return "Billing Information"
            Case 161094
                Return "Start Date"
            Case 161095
                Return "of the month"
            Case 161096
                Return "POS Import - Failed Data"
            Case 161097
                Return "Organize and maintain information of your suppliers including company contacts, addresses, terms of payment, etc. to ease up the ordering process."
            Case 161098
                Return "Terminal refers to the stations of your POS that are linked to your CALCMENU Web. Add, modify, or delete terminals in this program."
            Case 161099
                Return "Configure the POS import parameters. Set the schedule, location of import files, etc."
            Case 161100
                Return "Products and stock items are kept and circulated at different locations during different times. Maintain control in establishing the possible locations where your products can be found at any given moment."
            Case 161101
                Return "Clients are companies that purchase your products or finished goods. Manage your client list in this program."
            Case 161102
                Return "Client contacts are the persons you are dealing with in a company. Create, modify, and delete client contacts."
            Case 161103
                Return "Fix POS data which are not successfully imported in the system."
            Case 161104
                Return "This refers to the type of issuance transaction from supplies. This may or may not have been actually sold to customers such as employee benefits or giveaways."
            Case 161105
                Return "Sales History quickly shows a lsit of sales transaction and sales item involved"
            Case 161106
                Return "Marked Items"
            Case 161107
                Return "Computed Yield"
            Case 161132
                Return "View My Recipes"
            Case 161147
                Return "Recipe Management (except Menu Planning)"
            Case 161162
                Return "TCPOS"
            Case 161180
                Return "Define automatic upload configuration"
            Case 161181
                Return "Host name"
            Case 161275
                Return "Guideline Daily Amounts"
            Case 161276
                Return "GDA"
            Case 161279
                Return "Without"
            Case 161281
                Return "Power Cook"
            Case 161282
                Return "Propery Admin"
            Case 161283
                Return "System Admin"
            Case 161284
                Return "Corporate Chef"
            Case 161285
                Return "Propery Chef"
            Case 161286
                Return "Cook"
            Case 161287
                Return "Guest"
            Case 161288
                Return "Site Chef"
            Case 161289
                Return "Site Admin"
            Case 161290
                Return "View and Print"
            Case 161291
                Return "Not defined"
            Case 161292
                Return "Defined"
            Case 161294
                Return "Unwanted %s"
            Case 161300
                Return "Main Purchasing Set of Price"
            Case 161333
                Return "Labels"
            Case 161334
                Return "Recipes %x-%y of %z"
            Case 161468
                Return "Validate all"
            Case 161484
                Return "Temperature"
            Case 161485
                Return "Production" & vbCrLf & "Date"
            Case 161486
                Return "Consumption" & vbCrLf & "Date"
            Case 161487
                Return "Daily Product"
            Case 161488
                Return "Consume before"
            Case 161489
                Return "Fresh enjoy freshly-prepared"
            Case 161490
                Return "Info Allergies; contains:"
            Case 161491
                Return "Assigned to all marked"
            Case 161494
                Return "at max. 5°C"
            Case 161538
                Return "Thank you for your interest in EGS Products."
            Case 161554
                Return "You can also find additional information about our products as documents in PDF formats at the <a href=""%url"">Product Resources page</a>. "
            Case 161576
                Return "Unit price"
            Case 161577
                Return "Time"
            Case 161578
                Return "Total Ingredient Cost"
            Case 161579
                Return "calculate"
            Case 161580
                Return "Ingredient Cost"
            Case 161581
                Return "Tax"
            Case 161582
                Return "Grossmargin in Fr."
            Case 161583
                Return "Gross margin in %"
            Case 161584
                Return "Unit."
            Case 161585
                Return "Price/" & vbCrLf & "Unit"
            Case 161710
                Return "Template"
            Case 161766
                Return "Small portion"
            Case 161767
                Return "Large portion"
            Case 161777
                Return "Unassign keyword"
            Case 161778
                Return "Assign/unassign keywords"
            Case 161779
                Return "Breadcrumbs"
            Case 161780
                Return "Monitor Breadcrumbs"
            Case 161781
                Return "Unwanted Keyword"
            Case 161782
                Return "Print Labels"
            Case 161783
                Return "Procedure Template"
            Case 161784
                Return "Student"
            Case 161785
                Return "Ingredient nutrient values per %s"
            Case 161786
                Return "Ingredient nutrient values per 100g/ml"
            Case 161787
                Return "Apply Template"
            Case 161788
                Return "Assigned/Derived Keywords"
            Case 161823
                Return "Add Row(s)"
            Case 161824
                Return "Paste from Clipboard"
            Case 161825
                Return "There is no Ingredient that needs to be linked."
            Case 161826
                Return "Choose Another"
            Case 161827
                Return "Default Price/Unit:"
            Case 161828
                Return "Choose from existing units"
            Case 161829
                Return "Add this as a new unit"
            Case 161830
                Return "Item validated"
            Case 161831
                Return "Let me edit Ingredient before adding"
            Case 161832
                Return "place %s in complement"
            Case 161834
                Return "Please check the prices"
            Case 161835
                Return "Cut"
            Case 161837
                Return "Add to recipe"
            Case 161838
                Return "Replace existing ingredients"
            Case 161839
                Return "No ingredients found"
            Case 161840
                Return ""
            Case 161841
                Return "Link to Ingredient or subrecipe"
            Case 161842
                Return "All items are now linked to Ingredient/subrecipe"
            Case 161843
                Return "Item is now linked to Ingredient/subrecipe"
            Case 161844
                Return "Storing Time"
            Case 161845
                Return "Storing Temperature"
            Case 161851
                Return "Can be ordered"
            Case 161852
                Return "Recipe may contain allergens"
            Case 161853
                Return "Paste"
            Case 161855
                Return "Draft"
            Case 161873
                Return "Log out"
            Case 161899
                Return "Submitted by"
            Case 161902
                Return "Add a comment"
            Case 161955
                Return "Your friend's name"
            Case 161956
                Return "Your friend's e-mail"
            Case 161970
                Return "No review for this recipe. Be the first one to review this recipe."
            Case 161986
                Return "Add Step"
            Case 161987
                Return "Item %n of %p"
            Case 161988
                Return "Linked Products"
            Case 161989
                Return "Not Linked Products"
            Case 162032
                Return "Your email has been sent to your friend"
            Case 162039
                Return "%p users added this recipe to their favorites"
            Case 162054
                Return "Rating of"
            Case 162057
                Return "Empty %c  is not allowed"
            Case 162061
                Return "on"
            Case 162062
                Return "Recipe on"
            Case 162102
                Return "Rating of %p (%r reviews)"
            Case 162198
                Return "The yield has been changed. Click the Calculate button to resize ingredient quantities."
            Case 162199
                Return "The yield has been changed. Do you want to continue saving without calculating ingredient quantities?"
            Case 162203
                Return "Information"
            Case 162205
                Return "Number of bids"
            Case 162208
                Return "Weekly Business Days"
            Case 162211
                Return "Select Language"
            Case 162212
                Return "Business Name"
            Case 162213
                Return "Business Number"
            Case 162214
                Return "Price available"
            Case 162215
                Return "Logo to the server load"
            Case 162216
                Return "Preferences"
            Case 162219
                Return "Back Office"
            Case 162221
                Return "General Configuration"
            Case 162222
                Return "Insert Here"
            Case 162230
                Return "Enter style information"
            Case 162231
                Return "Name of style"
            Case 162232
                Return "Header style options"
            Case 162235
                Return "Did you mean"
            Case 162257
                Return "Date last modified"
            Case 162276
                Return "Import Recipe"
            Case 162282
                Return "Notes"
            Case 162314
                Return "Producer"
            Case 162318
                Return "Alcohol"
            Case 162319
                Return "Vintage"
            Case 162338
                Return "Wine Type"
            Case 162340
                Return "Street"
            Case 162341
                Return "Place"
            Case 162357
                Return "Example"
            Case 162358
                Return "Keep Length of Prefix"
            Case 162361
                Return "Tab"
            Case 162362
                Return "Pipe"
            Case 162363
                Return "Semi-colon"
            Case 162364
                Return "Space"
            Case 162382
                Return "Approve"
            Case 162383
                Return "Approval"
            Case 162386
                Return "Go"
            Case 162387
                Return "Hi Approver,You have received a recipe for approval. [Name of the creator of the item] has submitted this recipe: [...]Please login to the CALCMENU Web site to review and approve the recipe.Regards,EGS Team"
            Case 162388
                Return "Hi,Your newly created recipe has been sent for approval. The recipe will be reviewed and approved first before it can be used online. You have submitted this recipe: [...]Once approved, the recipe will be available online.Regards,EGS Team"
            Case 162389
                Return "Hi Approver,You have approved this recipe: [...]The recipe will be available online.Regards,EGS Team"
            Case 162390
                Return "Hi,The recipe [...] has been approved. You can now use this recipe online.Regards,EGS Team"
            Case 162455
                Return "Login"
            Case 162485
                Return "Send to a Friend"
            Case 162530
                Return "Delete breadcrumbs upon login"
            Case 162596
                Return "Add a Review"
            Case 162631
                Return "Forgot Password?"
            Case 162632
                Return "Enter your Username to receive your password."
            Case 162635
                Return "Answer the following question to receive your password."
            Case 162636
                Return "Question"
            Case 162637
                Return "Answer"
            Case 162638
                Return "Your password has been sent to you."
            Case 162742
                Return "Good"
            Case 162747
                Return "Last Modified:"
            Case 162888
                Return "Please select a file to upload."
            Case 162955
                Return "Net margin in %"
            Case 163032
                Return "Copy Price List"
            Case 163046
                Return "Sorry, Keyword %k%n%u not found. Please press 'Browse Keyword' to select available Keywords."
            Case 163057
                Return "Cost for total %s"
            Case 163058
                Return "Cost for 1 %s"
            Case 163060
                Return "Food Cost in %s"
            Case 163061
                Return "Imposed Food Cost in %s"
            Case 167272
                Return "Product details"
            Case 167346
                Return "Show all"
            Case 167385
                Return "SubTitle"
            Case 167469
                Return "Footnote"
            Case 167719
                Return "Budget"
            Case 168373
                Return "Used online"
            Case 168374
                Return "Reference No1"
            Case 168375
                Return "Reference No2"
            Case 169310
                Return "Degustation/Development"
            Case 169318
                Return "Feedback"
            Case 170155
                Return "Assign Ingredient and recipes to Categories, Keywords and Sources (could be a cookbook, Website, chef, etc.). This allows you to group and organize items in EGS CALCMENU Web. Searching for Ingredient, recipes or menus can be made faster and easier since Categories, Keywords, and Sources are very useful in narrowing down search results."
            Case 170253
                Return "View PDF"
            Case 170283
                Return "For more information, contact us at info@eg-software.com."
            Case 170668
                Return "Best regards,"
            Case 170674
                Return "Access without Login"
            Case 170675
                Return "Courses"
            Case 170770
                Return "Yield to Print"
            Case 170779
                Return "Ingredient List"
            Case 170780
                Return "Ingredient Details"
            Case 170781
                Return "Ingredient Nutrient List"
            Case 170782
                Return "Ingredient Category"
            Case 170783
                Return "Ingredient Keyword"
            Case 170784
                Return "Ingredient Published On The Web"
            Case 170785
                Return "Ingredient Not Published On The Web"
            Case 170786
                Return "Ingredient Cost"
            Case 170801
                Return "Final Composition"
            Case 170849
                Return "Abbreviated Preparation Method"
            Case 170850
                Return "Cook Mode only"
            Case 170851
                Return "None Cook Mode only"
            Case 170852
                Return "Show Off"
            Case 170853
                Return "Quick & Easy"
            Case 170854
                Return "Chef Recommended"
            Case 170855
                Return "Moderate"
            Case 170856
                Return "Challenging"
            Case 170857
                Return "Gold"
            Case 170858
                Return "Unrated"
            Case 170859
                Return "Bronze"
            Case 170860
                Return "Move marked items to new standard"
            Case 171014
                Return "equals to"
            Case 171219
                Return "LeadIn"
            Case 171220
                Return "Number of Servings"
            Case 171221
                Return "Total Yield"
            Case 171231
                Return "Download Barcode Fonts"
            Case 171232
                Return "Media"
            Case 171233
                Return "Print Type"
            Case 171234
                Return "Protected"
            Case 171235
                Return "Auto-calculate"
            Case 171236
                Return "Public"
            Case 171237
                Return "View actual size"
            Case 171238
                Return "Not used online"
            Case 171240
                Return "Unsaved Items"
            Case 171241
                Return "Show percentage translated"
            Case 171242
                Return "Work with protected copies"
            Case 171243
                Return "Include when printing and exporting"
            Case 171244
                Return "Footer Logo and Address for Report"
            Case 171245
                Return "Footer Address"
            Case 171246
                Return "Force delete categories"
            Case 171249
                Return "%s already exists."
            Case 171301
                Return "Preparation Method"
            Case 171302
                Return "Tips"
            Case 171345
                Return "All Courses"
            Case 171346
                Return "All Year"
            Case 171347
                Return "Courses Offered"
            Case 171348
                Return "Course"
            Case 171352
                Return "Invalid username/email address"
            Case 171353
                Return "To retrieve your password, type your username or e-mail address."
            Case 171354
                Return "Enter your username or e-mail address"
            Case 171371
                Return "Show More"
            Case 171372
                Return "Show Less"
            Case 171373
                Return "Please save the Recipe  first."
            Case 171399
                Return "Kiosk for %CM"
            Case 171401
                Return "The recipes visible on this Kiosk are created by %CM."
            Case 171402
                Return "Share this recipe on %p"
            Case 171425
                Return "Powered by"
            Case 171428
                Return "Invalid Parameter. Contact the sender of the recipe or the CALCMENU Cloud Support Team."
            Case 171429
                Return "The link to that recipe/group has expired. Contact the sender of the recipe or the CALCMENU Cloud Support Team."
            Case 171447
                Return "Your e-mail/SMTP have not yet been configured. Configure your e-mail under Configurations menu before using this feature."
            Case 171453
                Return "Unable to send email."
            Case 171501
                Return "If you do not know, please e-mail us your CALCMENU serial number and header name."
            Case 171502
                Return "Kindly use the EGS Website Login details associated with your CALCMENU product keys and serial number."
            Case 171505
                Return "This recipe is encoded in CALCMENU. Visit %link to know more."
            Case 171506
                Return "Use your EGS Website Login details to login."
            Case 171507
                Return "Forgot your username and/or password?"
            Case 171555
                Return "To view this recipe online go to:  %p"
            Case 171557
                Return "Ingredient is the basic element or item that comprises your recipes."
            Case 171558
                Return "Select available languages to use for translating Ingredient, recipes, and other information"
            Case 171559
                Return "Recipecenter is an extensive collection of recipes from around the world -- from amateur to professional chefs and site members who may rate and review recipes online."
            Case 171560
                Return "You cannot merge system units."
            Case 171561
                Return "Adding self as subrecipe is not allowed."
            Case 171586
                Return "Replace ingredient failed because there is no matching unit in the ingredients."
            Case 171588
                Return "Forum Culinaire Courses"
            Case 171589
                Return "Ingredients Management from %c Suppliers"
            Case 171591
                Return "Yield Percentage is required."
            Case 171592
                Return "Supplier Name"
            Case 171593
                Return "Code Supplier"
            Case 171594
                Return "Some allergens have not been flagged. Please select at least one flag for each allergen."
            Case 171595
                Return "Recipes on this website are managed by %Cmcloud , an advanced recipe management & edition tool for food professionals and recipe editors"
            Case 171596
                Return "Click here to return to %RC"
            Case 171597
                Return "Recipe has been checked in by another user and cannot be modified."
            Case 171598
                Return "Yes, I wish to receive information about CALCMENU Cloud, once it is made available"
            Case 171599
                Return "In continuously developing advanced recipe software, we introduce to you the new and improved Recipecenter. The site provides a simple and easy interface for checking recipes online and sharing to your friends through Facebook and Twitter using your mobile phones, iPhone or iPad, Blackberry, and other devices."
            Case 171600
                Return "View, ""fave"", rate, and comment on recipes. If you are already registered in recipecenter.com, you can log-in using the same account details, and your encoded recipes and other information are not lost. Users can also soon have a more advanced recipe management solution to encode, share recipes, and access a vast recipe collection from new contributors - thanks to the integration with the recipe management software - CALCMENU Cloud."
            Case 171601
                Return "Please tell your friends to join our community. We hope you enjoy your visit to our site and come back again soon."
            Case 171602
                Return "Recipes from this website are encoded and managed using the recipe management software: CALCMENU Cloud."
            Case 171605
                Return "Favorites"
            Case 171611
                Return "Fave it"
            Case 171612
                Return "Not Good"
            Case 171614
                Return "Send recipe to a friend:"
            Case 171615
                Return "Connect with us"
            Case 171616
                Return "Placement"
            Case 171617
                Return "Publication"
            Case 171618
                Return "Digital Asset"
            Case 171619
                Return "Brand Site"
            Case 171620
                Return "External web site"
            Case 171621
                Return "To retrieve your password, enter the e-mail address for your account below."
            Case 171622
                Return "%c Item Number"
            Case 171628
                Return "Featured recipes by our contributors"
            Case 171631
                Return "Lot"
            Case 171649
                Return "Lot Number"
            Case 171650
                Return "Prep Time"
            Case 171651
                Return "Cook Time"
            Case 171652
                Return "Marinate Time"
            Case 171653
                Return "Stand Time"
            Case 171654
                Return "Chill Time"
            Case 171655
                Return "Brew Time"
            Case 171656
                Return "Freeze Time"
            Case 171657
                Return "ReadyIn"
            Case 171658
                Return "second"
            Case 171662
                Return "Ingredients and Procedure"
            Case 171663
                Return "Add Related Products"
            Case 171664
                Return "Checked-out Items"
            Case 171665
                Return "All marks have been successfully deleted"
            Case 171666
                Return "Profile"
            Case 171667
                Return "Fully Translated"
            Case 171668
                Return "Recipe Status"
            Case 171669
                Return "Web Status"
            Case 171670
                Return "Primary Brand"
            Case 171671
                Return "Cost per %s"
            Case 171672
                Return "Secondary Brand"
            Case 171673
                Return "Create a new Ingredient"
            Case 171674
                Return "Maximum Number of Ingredients"
            Case 171675
                Return "Current Number of Ingredients"
            Case 171676
                Return "Move marked to new category"
            Case 171677
                Return "Selection of ingredient to print"
            Case 171678
                Return "Marked Ingredient"
            Case 171679
                Return "The following ingredients are being used and are not deleted."
            Case 171680
                Return "Recipe can be used as sub-recipe"
            Case 171681
                Return "Upload Digital assets"
            Case 171682
                Return "Sub-Recipes"
            Case 171683
                Return "Recipe being used as a sub-recipe"
            Case 171684
                Return "Delete existing marks first"
            Case 171685
                Return "Delete link"
            Case 171686
                Return "Save marks for ingredient"
            Case 171687
                Return "Please log in your appropriate username and password."
            Case 171688
                Return "Ingredient Management"
            Case 171689
                Return "No selected Item."
            Case 171690
                Return "Sub-Recipe Definition"
            Case 171691
                Return "Import Ingredient from CSV File (Excel)"
            Case 171692
                Return "Ingredient is the basic element or item that comprises your recipes."
            Case 171693
                Return "The system will update the prices of all ingredient. Please wait…"
            Case 171694
                Return "Delete unused ingredient units before import"
            Case 171696
                Return "To add or edit the ingredient price, enter the new price and define the unit of measurement. Assign the ratio of that unit to the original unit. For example, the original price and unit is US $11 per kilogram (kg). If you want to add the unit bag, you have to define the price of that bag, or define how many kilograms there are in 1 bag (ratio)."
            Case 171697
                Return "Use main/big unit when adding ingredient price"
            Case 171698
                Return "You've reached your disk space limit. Please delete some of your recipes or ingredients. Thank you."
            Case 171699
                Return "Create new ingredient name with up to 250 characters and include alphanumeric reference number, tax rate, four wastage percentages, category, supplier, and other helpful information such as product description, preparation, cooking tip, refinement methods, and storage."
            Case 171700
                Return "Project Name"
            Case 171701
                Return "Ingredients, and Recipes can be searched using their name or reference numbers. You can also search using categories and keywords. For the ingredient, you can also use supplier, date encoded or last modified, price range, and nutrient values when searching. For the recipes, you can search using items used and not used."
            Case 171702
                Return "Action Marks are shortcuts in performing a similar function that could apply to a marked ingredient or recipe. You can use action marks to assign ingredient or recipe to a category and keywords, delete them, export, send via e-mail, print, share, and unshare to other users without having to repeat them for each item. This saves you a lot of time and effort in performing an action to the marked items."
            Case 171703
                Return "Ingredient and recipe lists can be printed together with their details, prices, and nutrient values. Shopping lists or the list of ingredients together with cumulative quantities used in various recipes can also be printed. PDF and Excel files can also be created for the various reports."
            Case 171704
                Return "Import Ingredient CSV/Supplier Network"
            Case 171705
                Return "This function has been disabled. Please contact your head office if you need new ingredients."
            Case 171706
                Return "CALCMENU Web"
            Case 171707
                Return "Enable grouping sites by property to be managed by admin:"
            Case 171708
                Return "Select available languages to use for translating ingredients, recipes, and other information"
            Case 171709
                Return "Select one or more price groups to use for assigning prices to your ingredient and recipe"
            Case 171710
                Return "This ingredient is published on the web."
            Case 171711
                Return "This ingredient is not published on the web."
            Case 171712
                Return "This ingredient is exposed."
            Case 171713
                Return "This ingredient is not exposed."
            Case 171714
                Return "Delete selected item from the list."
            Case 171715
                Return "Approval of ingredients, recipes, and other information."
            Case 171716
                Return "View Pictures"
            Case 171717
                Return "Define list of languages for translating ingredients, recipes, and other information."
            Case 171718
                Return "Work with ingredients and recipes with multiple sets of prices."
            Case 171719
                Return "Recipe Time"
            Case 171720
                Return "Define standard picture size for ingredients and recipes."
            Case 171721
                Return "Trademarks or distinctive names identifying ingredients."
            Case 171722
                Return "Used to group ingredients or recipes by common attributes."
            Case 171723
                Return "Delete Version"
            Case 171724
                Return "Keywords provide descriptive details to ingredients or recipes. Users can assign multiple keywords per ingredient or recipe."
            Case 171725
                Return "Define up to %c nutrients values for nutrients like Energy, Carbohydrates, Proteins, and Lipids."
            Case 171726
                Return "List of predefined (or system) units used in defining ingredient prices as well as in encoding recipes."
            Case 171727
                Return "Import ingredients or recipes from CALCMENU Pro, CALCMENU Enterprise, and other EGS products."
            Case 171728
                Return "Print ingredient list in HTML, Excel, PDF, and RTF formats."
            Case 171729
                Return "Print ingredient details in HTML, Excel, PDF, and RTF formats."
            Case 171730
                Return "Create new local ingredient that can be used as ingredient for your recipes."
            Case 171731
                Return "Import Ingredient Data with Errors"
            Case 171732
                Return "Legacy"
            Case 171733
                Return "Pending list of ingredients that need to be fixed"
            Case 171734
                Return "Define options for ingredient import"
            Case 171735
                Return "List of food allergies or sensitivities associated to ingredient."
            Case 171736
                Return "Delete trailing zeroes"
            Case 171737
                Return "Are you sure you want to delete selected item(s)?"
            Case 171738
                Return "Selected item(s) has been successfully deleted."
            Case 171739
                Return "Ingredient Price List"
            Case 171740
                Return "Are you sure you want to change the nutrient database? This action will change the nutrient definitions you have already set in your ingredients."
            Case 171741
                Return "Deleted keywords will also be unassigned from ingredient/recipe items."
            Case 171742
                Return "Selected keywords and all its sub-keywords are successfully deleted. Deleted keywords are now also unassigned from ingredient and recipe items."
            Case 171743
                Return "Import Ingredient"
            Case 171744
                Return "Total Ingredient Cost"
            Case 171745
                Return "There is no ingredient that needs to be linked."
            Case 171746
                Return "Let me edit ingredient before adding"
            Case 171747
                Return "Link to ingredient or sub-recipe"
            Case 171748
                Return "All items are now linked to ingredient/sub-recipe"
            Case 171749
                Return "Item is now linked to ingredient/sub-recipe"
            Case 171750
                Return "Delete breadcrumbs upon login"
            Case 171751
                Return "Delete a Product from the Inventory"
            Case 171752
                Return "Delete Quantity for the selected Product-Location"
            Case 171753
                Return "Delete the selected location for the product"
            Case 171754
                Return "Assign ingredients and recipes to Categories, Keywords and Sources (could be a cookbook, Website, chef, etc.). This allows you to group and organize items in EGS CALCMENU Web. Searching for ingredients or recipes can be made faster and easier since Categories, Keywords, and Sources are very useful in narrowing down search results."
            Case 171755
                Return "Project"
            Case 171756
                Return "Nutrient Set"
            Case 171758
                Return "Please enter recipe title"
            Case 171759
                Return "Please enter a valid url."
            Case 171760
                Return "Upload limit exceeded!"
            Case 171761
                Return "Executable file not permitted"
            Case 171762
                Return "List Format"
            Case 171763
                Return "Bullet"
            Case 171764
                Return "Serve with"
            Case 171765
                Return "Seq"
            Case 171767
                Return "Author for the Web"
            Case 171768
                Return "Updated By"
            Case 171769
                Return "Date last tested"
            Case 171770
                Return "Date Developed"
            Case 171771
                Return "Date Final Edited"
            Case 171772
                Return "Development Purpose"
            Case 171773
                Return "Set recipe date visibility"
            Case 171774
                Return "Nutrition"
            Case 171775
                Return "Imposed Nutrient Type"
            Case 171776
                Return "Display Nutrition"
            Case 171777
                Return "Imposed Nutrients"
            Case 171778
                Return "Calculated Nutrients"
            Case 171779
                Return "Nutritional Basis"
            Case 171780
                Return "Assigned Brands"
            Case 171781
                Return "Unassigned Brands from ingredients"
            Case 171782
                Return "List of Project"
            Case 171783
                Return "Selected Project"
            Case 171785
                Return "Posted By"
            Case 171786
                Return "Date Posted"
            Case 158997
                Return "Costing"
        End Select
    End Function

 
'german

    Public Function FTBLow2USA(ByVal Code As Integer) As String
        Select Case Code
            Case 1080
                Return "Zutatkosten"
            Case 1081
                Return "Zutatkosten"
            Case 1090
                Return "Verkaufspreis"
            Case 1145
                Return "Zähler"
            Case 1146
                Return "In Bearbeitung"
            Case 1260
                Return "Ware(n)"
            Case 1280
                Return "Bemerkung"
            Case 1290
                Return "Preis"
            Case 1300
                Return "Verlust"
            Case 1310
                Return "Menge"
            Case 1400
                Return "Menü"
            Case 1450
                Return "Kategorie"
            Case 1480
                Return "Festgesetzter Preis"
            Case 1485
                Return "Kalkulierter Preis"
            Case 1500
                Return "Datum"
            Case 1530
                Return "Einheit fehlt"
            Case 1600
                Return "Menü ändern"
            Case 2430
                Return "&In der Liste auszuwählen"
            Case 2700
                Return "Menüliste drucken"
            Case 2780
                Return "Bedarfsliste"
            Case 3057
                Return "Datenbank"
            Case 3140
                Return "Für"
            Case 3150
                Return "Prozentsatz"
            Case 3161
                Return "Faktor"
            Case 3195
                Return "Rezept Nr."
            Case 3200
                Return "Koch"
            Case 3204
                Return "Vorname"
            Case 3205
                Return "Name"
            Case 3206
                Return "Übersetzung"
            Case 3215
                Return "Einheitspreis"
            Case 3230
                Return "Bild"
            Case 3234
                Return "Liste"
            Case 3300
                Return "Menükarte"
            Case 3305
                Return "Referenzname"
            Case 3306
                Return "Vertreter"
            Case 3320
                Return "Möchten Sie die Mengen jetzt der neuen Anzahl Portionen anpassen?"
            Case 3460
                Return "&Passwort"
            Case 3680
                Return "Backup"
            Case 3685
                Return "Backup beendet"
            Case 3721
                Return "Quelle"
            Case 3760
                Return "Import"
            Case 3800
                Return "Export"
            Case 4130
                Return "Freiraum auf Festplat"
            Case 4185
                Return "Produktkennung"
            Case 4755
                Return "Import starten"
            Case 4825
                Return "Rezepte"
            Case 4832
                Return "Rezept"
            Case 4834
                Return "Rezeptzutaten"
            Case 4854
                Return "Minimum"
            Case 4855
                Return "Maximum"
            Case 4856
                Return "Seit dem"
            Case 4860
                Return "Dateiname"
            Case 4862
                Return "Version"
            Case 4865
                Return "Benutzer"
            Case 4867
                Return "Abändern"
            Case 4870
                Return "Benutzer abändern"
            Case 4877
                Return "Durchschnitt"
            Case 4890
                Return "Dateityp"
            Case 4891
                Return "Ansicht"
            Case 5100
                Return "Einheit"
            Case 5105
                Return "Format"
            Case 5270
                Return "Zutatliste" '"Zutatliste"
            Case 5350
                Return "Summe"
            Case 5390
                Return "Personen"
            Case 5500
                Return "Nummer"
            Case 5530
                Return "Festgesetzter Verkaufspreis"
            Case 5590
                Return "Zutaten"
            Case 5600
                Return "Zubereitung"
            Case 5610
                Return "Seite"
            Case 5720
                Return "Betrag"
            Case 5741
                Return "Brutto"
            Case 5795
                Return "pro Portion"
            Case 5801
                Return "Br. Erfolg"
            Case 5900
                Return "Zutatkategorie" '"Zutatkategorie"
            Case 6000
                Return "Änderung der Kategorie"
            Case 6002
                Return "Name der Kategorie"
            Case 6055
                Return "Text hinzufügen"
            Case 6390
                Return "Währung"
            Case 6416
                Return "Faktor"
            Case 6470
                Return "Bitte warten Sie einen Augenblick"
            Case 7010
                Return "Nein"
            Case 7030
                Return "Drucker"
            Case 7073
                Return "Durchsuchen"
            Case 7181
                Return "Alles"
            Case 7183
                Return "Markiert"
            Case 7250
                Return "Französisch"
            Case 7260
                Return "Deutsch"
            Case 7270
                Return "Englisch"
            Case 7280
                Return "Italienisch"
            Case 7292
                Return "japanisch"
            Case 7296
                Return "Europa"
            Case 7335
                Return "Alle Markierungen wurden erfolgreich gelöscht"
            Case 7570
                Return "Sonntag"
            Case 7571
                Return "Montag"
            Case 7572
                Return "Dienstag"
            Case 7573
                Return "Mittwoch"
            Case 7574
                Return "Donnerstag"
            Case 7575
                Return "Freitag"
            Case 7576
                Return "Samstag"
            Case 7720
                Return "Verpackung"
            Case 7725
                Return "Transport"
            Case 7755
                Return "System"
            Case 8210
                Return "Berechnung"
            Case 8220
                Return "Rezeptzubereitung"
            Case 8395
                Return "Hinzufügen"
            Case 8397
                Return "Löschen"
            Case 8514
                Return "Neuer Preis"
            Case 8913
                Return "Keine"
            Case 8914
                Return "Dezimal"
            Case 8990
                Return "oder"
            Case 8994
                Return "Werkzeuge"
            Case 9030
                Return "Aufarbeiten"
            Case 9070
                Return "In der Demoversion nicht erlaubt"
            Case 9140
                Return "Schweiz"
            Case 9920
                Return "Beschreibung"
            Case 10103
                Return "Kopieren"
            Case 10104
                Return "Text"
            Case 10109
                Return "Optionen"
            Case 10116
                Return "Notiz"
            Case 10121
                Return "Suchen"
            Case 10125
                Return "Zubereitung"
            Case 10129
                Return "Auswahl"
            Case 10130
                Return "an Lager"
            Case 10131
                Return "Eingang"
            Case 10132
                Return "Ausgang"
            Case 10135
                Return "Stil"
            Case 10140
                Return "Lager"
            Case 10363
                Return "Steuer"
            Case 10369
                Return "Lieferantennummer"
            Case 10370
                Return "In Bestellung"
            Case 10399
                Return "löschen"
            Case 10417
                Return "Fehler:"
            Case 10430
                Return "Ort"
            Case 10431
                Return "Inventur"
            Case 10447
                Return "Bestellung"
            Case 10468
                Return "Status"
            Case 10513
                Return "Rabatt"
            Case 10523
                Return "Tel."
            Case 10524
                Return "Fax"
            Case 10554
                Return "CCP-Beschreibung"
            Case 10555
                Return "Abkühlzeit"
            Case 10556
                Return "Erhitzungszeit"
            Case 10557
                Return "Erhitzungsgrad/Temperatur"
            Case 10558
                Return "Erhitzungsart"
            Case 10572
                Return "Nährwerte"
            Case 10573
                Return "Infos1"
            Case 10970
                Return "Drucken"
            Case 10990
                Return "Lieferant"
            Case 11040
                Return "Rückgabe beendet"
            Case 11060
                Return "Verzeichnis"
            Case 11280
                Return "Registrierung"
            Case 12515
                Return "Strichcode"
            Case 12525
                Return "Ungültiges Datum"
            Case 13060
                Return "Nährstoffe"
            Case 13065
                Return "Nährwerte anzeigen"
            Case 13255
                Return "Geschichte"
            Case 14070
                Return "Schrift"
            Case 14090
                Return "Titel"
            Case 14110
                Return "Seitenende"
            Case 14816
                Return "Ersetzen durch"
            Case 14819
                Return "Ersetzen"
            Case 14884
                Return "Aufgearbeitete Zutat"
            Case 15360
                Return "Markierte Menüs"
            Case 15504
                Return "Verwalter"
            Case 15510
                Return "Paßwort"
            Case 15615
                Return "Geben Sie Ihr Passwort ein"
            Case 15620
                Return "Bestätigung"
            Case 16010
                Return "Kalkulation"
            Case 18460
                Return "Speicherung im Gange"
            Case 19330
                Return "Grösse"
            Case 20122
                Return "Betrieb"
            Case 20200
                Return "Unterrezept"
            Case 20469
                Return "Geben Sie die Versandweise an."
            Case 20530
                Return "Energie"
            Case 20703
                Return "Hauptinfos"
            Case 20709
                Return "Einheiten"
            Case 21550
                Return "Kein Rezept gefunden"
            Case 21570
                Return "FAX-Formular drucken"
            Case 21600
                Return "zu"
            Case 24002
                Return "Letzte Bestellung"
            Case 24011
                Return "von"
            Case 24016
                Return "Lieferant"
            Case 24027
                Return "Rechnen"
            Case 24028
                Return "Annulieren"
            Case 24044
                Return "Beide"
            Case 24050
                Return "Neu"
            Case 24068
                Return "Spielraum"
            Case 24075
                Return "Artikel- nummer"
            Case 24085
                Return "Neue Anweisung"
            Case 24087
                Return "Keine Ware gefunden"
            Case 24105
                Return "Anzeigen"
            Case 24121
                Return "Abk."
            Case 24129
                Return "Übertragen"
            Case 24150
                Return "Ändern"
            Case 24152
                Return "Funktion"
            Case 24153
                Return "Ort"
            Case 24163
                Return "Standardlagerort"
            Case 24260
                Return "Dieser Lieferant kann nicht gelöscht werden"
            Case 24268
                Return "Alle Markierungen aufheben"
            Case 24269
                Return "Alles markieren"
            Case 24270
                Return "Zurück"
            Case 24271
                Return "Nächstes"
            Case 24291
                Return "Subtotal"
            Case 26000
                Return "Weiterfahren"
            Case 26100
                Return "Produktbeschreibung"
            Case 26101
                Return "Kochhinweis/ Beratung"
            Case 26102
                Return "Verfeinerung"
            Case 26103
                Return "Lagerung"
            Case 26104
                Return "Ertrag/Produktivität"
            Case 27000
                Return "Referenzname"
            Case 27020
                Return "Anschrift"
            Case 27050
                Return "Telefonnummer"
            Case 27055
                Return "Benutzertitel"
            Case 27056
                Return "und"
            Case 27130
                Return "Zahlung"
            Case 27135
                Return "Verfalldatum"
            Case 27220
                Return "Zeit"
            Case 27530
                Return "Satz"
            Case 28000
                Return "Fehler in der Ausführung"
            Case 28008
                Return "Ungültiges Verzeichnis"
            Case 28420
                Return "Kein Bild erhältlich"
            Case 28483
                Return "Die Speicherung existiert nicht"
            Case 28655
                Return "Keine Einheit wurde bestimmt"
            Case 29170
                Return "Nicht verfügbar"
            Case 29771
                Return "Zutat ändern"
            Case 30210
                Return "Die Operation ist misslungen"
            Case 30240
                Return "Code"
            Case 30270
                Return "nicht gefunden"
            Case 31085
                Return "Aufarbeitung erfolgreich ausgeführt"
            Case 31098
                Return "Speichern"
            Case 31370
                Return "Zutatkosten (%)"
            Case 31375
                Return "WK"
            Case 31380
                Return "Hauptinfos"
            Case 31462
                Return "Fehler"
            Case 31492
                Return "Dieser Hilfe-Service per Fax sichert Ihnen je nach Problem eine Antwort innert 1 bis 24 Stunden zu (Wochenends nicht garantiert)"
            Case 31700
                Return "Tage"
            Case 31732
                Return "Menüplan"
            Case 31755
                Return "Resultate"
            Case 31758
                Return "Bis"
            Case 31769
                Return "verkauft"
            Case 31800
                Return "Tag"
            Case 31860
                Return "Zeitspanne"
            Case 51056
                Return "Produkt"
            Case 51086
                Return "Sprache"
            Case 51092
                Return "Einheit"
            Case 51097
                Return "EGS Enggist && Grandjean Software SA"
            Case 51098
                Return "Pierre-à-Bot 92"
            Case 51099
                Return "2000 Neuchâtel, Schweiz"
            Case 51123
                Return "Details"
            Case 51128
                Return "Rezepttitel"
            Case 51129
                Return "Gewünschte Zutaten"
            Case 51130
                Return "Ungewünschte Zutaten"
            Case 51131
                Return "Kategoriename"
            Case 51139
                Return "Gewünscht"
            Case 51157
                Return "Meldung"
            Case 51174
                Return "Import fertig!"
            Case 51178
                Return "Nachmals versuchen."
            Case 51198
                Return "Verbinden mit SMTP Server"
            Case 51204
                Return "Ja"
            Case 51243
                Return "Rand"
            Case 51244
                Return "Oben"
            Case 51245
                Return "Unten"
            Case 51246
                Return "Links"
            Case 51247
                Return "Rechts"
            Case 51252
                Return "Herunterladen"
            Case 51257
                Return "E-mail"
            Case 51259
                Return "SMTP Server"
            Case 51261
                Return "Benutzername"
            Case 51281
                Return "Zutaten für"
            Case 51294
                Return "Für"
            Case 51311
                Return "Ungültige Einheit"
            Case 51323
                Return "Ungültiges Wert für 'Ergibt'"
            Case 51336
                Return "Unerwünscht"
            Case 51337
                Return "Haupt"
            Case 51353
                Return "Copyright Einverständnis"
            Case 51364
                Return "Akzeptieren Sie die hier obengenannten Copyright Vereinbarung und sind Sie mit der Veröffentlichung Ihres Rezeptes einverstanden?"
            Case 51373
                Return "Bitte alle Informationen über SMTP, POP, Benutzername und Passwort"
            Case 51377
                Return "Email senden"
            Case 51392
                Return "Ertragseinheit"
            Case 51402
                Return "Sind Sie sicher das Sie löschen wollen"
            Case 51500
                Return "Einkaufsliste Details"
            Case 51502
                Return "Einkaufsliste"
            Case 51532
                Return "Zutatliste drucken"
            Case 51907
                Return "&Details anzeigen"
            Case 52012
                Return "Blättern"
            Case 52110
                Return "Die gewählte Datei wird importiert"
            Case 52130
                Return "Neues Rezept"
            Case 52150
                Return "Fertig"
            Case 52307
                Return "Schliessen"
            Case 52960
                Return "Einfach"
            Case 52970
                Return "Komplett"
            Case 53250
                Return "Auswahl exportieren"
            Case 54210
                Return "Nichts ändern"
            Case 54220
                Return "Alles in Großbuchstaben"
            Case 54230
                Return "Alles in Kleinbuchstaben"
            Case 54240
                Return "Schreiben Sie den Anfangsbuchstaben von jedem Wort groß"
            Case 54245
                Return "Erster Buchstabe großgeschrieben"
            Case 54295
                Return "mit"
            Case 54710
                Return "Ausgewählte Schlüsselworte"
            Case 54730
                Return "Schlüsselwörter"
            Case 55011
                Return "Portion"
            Case 55211
                Return "Verbindung"
            Case 55220
                Return "Menge"
            Case 56100
                Return "Ihr Name"
            Case 56130
                Return "Land"
            Case 56500
                Return "Wörterbuch"
            Case 101600
                Return "Menü ändern"
            Case 103150
                Return "Prozentsatz"
            Case 103215
                Return "Einheitspreis"
            Case 103305
                Return "Referenzname"
            Case 103306
                Return "Vertreter"
            Case 104829
                Return "Lieferantenliste"
            Case 104835
                Return "Neues Produkt erstellen"
            Case 104836
                Return "Produkt modifizieren"
            Case 104854
                Return "Minimum"
            Case 104855
                Return "Maximum"
            Case 104862
                Return "Version"
            Case 104869
                Return "Neuer Benutzer"
            Case 104870
                Return "Benutzer abändern"
            Case 105100
                Return "Einheit"
            Case 105110
                Return "Datum"
            Case 105200
                Return "für"
            Case 105360
                Return "Verkaufspreis pro Portion"
            Case 106002
                Return "Name der Kategorie"
            Case 107183
                Return "Markiert"
            Case 109730
                Return "durch"
            Case 110101
                Return "Ändern"
            Case 110102
                Return "Löschen"
            Case 110112
                Return "Drucken"
            Case 110114
                Return "Hilfe"
            Case 110129
                Return "Auswahl"
            Case 110417
                Return "Fehlgeschlagen"
            Case 110447
                Return "Bestellung"
            Case 110524
                Return "Fax"
            Case 113275
                Return "Steuer"
            Case 115510
                Return "Paßwort"
            Case 115610
                Return "Neues Paßwort akzeptiert"
            Case 119130
                Return "Suche"
            Case 121600
                Return "zu"
            Case 124016
                Return "Lieferant"
            Case 124024
                Return "Genehmigt von"
            Case 124042
                Return "Typ"
            Case 124164
                Return "Inventur anpassen"
            Case 124257
                Return "Verkaufsstellen"
            Case 127010
                Return "Firma"
            Case 127040
                Return "Land"
            Case 127050
                Return "Telefonnummer"
            Case 127055
                Return "Benutzertitel"
            Case 128000
                Return "Fehler in der Ausführung"
            Case 131462
                Return "Fehler"
            Case 131700
                Return "Tage"
            Case 131757
                Return "von"
            Case 132541
                Return "Rezept"
            Case 132552
                Return "Gesamtsteuerbetrag"
            Case 132553
                Return "Festgesetzter Verkaufspreis + Steuer"
            Case 132554
                Return "Rezept modifizieren"
            Case 132555
                Return "Rezept hinzufügen"
            Case 132557
                Return "Neues Menü erstellen"
            Case 132559
                Return "Eine neue Ware erstellen"
            Case 132561
                Return "Bitte geben Sie Seriennummer, Titelnamen und Produktkennung ein. Diese Information finden Sie in der zu CALCMENU mitgelieferteten Dokumentation."
            Case 132565
                Return "Zusatz"
            Case 132567
                Return "Zutatkategorie"
            Case 132568
                Return "Rezeptekategorie"
            Case 132569
                Return "Menükategorie"
            Case 132570
                Return "Konnte nicht löschen."
            Case 132571
                Return "Die Kategorie wird benutzt."
            Case 132586
                Return "Kontoinformation"
            Case 132589
                Return "Maximale Anzahl Rezepte"
            Case 132590
                Return "Aktuelle Anzahl Rezepte"
            Case 132592
                Return "Maximale Anzahl Zutat"
            Case 132593
                Return "Aktuelle Anzahl Zutat"
            Case 132597
                Return "Ein neues Rezept erstellen"
            Case 132598
                Return "Maximale Anzahl Menüs"
            Case 132599
                Return "Aktuelle Anzahl Menüs"
            Case 132600
                Return "Schlüsselwort zuweisen"
            Case 132601
                Return "Markierte zu einer neuen Kategorie übergeben"
            Case 132602
                Return "Markiertes löschen"
            Case 132605
                Return "Einkaufsliste"
            Case 132607
                Return "Aktionsmarkierungen"
            Case 132614
                Return "Nettomenge"
            Case 132615
                Return "Rechte"
            Case 132616
                Return "Eigentümer"
            Case 132617
                Return "ALLE KATEGORIEN"
            Case 132621
                Return "Quelle modifizieren"
            Case 132630
                Return "Automatische Konvertierung"
            Case 132638
                Return "Benutzerinformation"
            Case 132640
                Return "Der Benutzername wird bereits benutzt."
            Case 132654
                Return "Datenbankverwaltung"
            Case 132657
                Return "&Restaurieren"
            Case 132667
                Return "Zusammenführen"
            Case 132668
                Return "Löschen"
            Case 132669
                Return "Nach oben bewegen"
            Case 132670
                Return "Nach unten bewegen"
            Case 132671
                Return "Standardisieren"
            Case 132672
                Return "Sind Sie sicher, daß Sie %n löschen möchten?"
            Case 132678
                Return "HACCP"
            Case 132683
                Return "Zurück"
            Case 132706
                Return "Nährwerte pro 100g oder 100ml"
            Case 132708
                Return "Kein Lieferant"
            Case 132714
                Return "Bitte wählen Sie aus der Liste."
            Case 132719
                Return "Der Preis für die gleiche Einheit ist bereits definiert."
            Case 132723
                Return "Der Gesamtausschuß kann nicht größer/gleich 100% sein."
            Case 132736
                Return "Bruttomenge"
            Case 132737
                Return "Neuen Lieferanten hinzufügen"
            Case 132738
                Return "Lieferanten modifizieren"
            Case 132739
                Return "Lieferanteneinzelheiten"
            Case 132740
                Return "Bundesland"
            Case 132741
                Return "URL"
            Case 132779
                Return "Das Schlüsselwort wird verwendet."
            Case 132783
                Return "Schlüsselwort"
            Case 132788
                Return "Nährstoff verknüpfen"
            Case 132789
                Return "&Login"
            Case 132793
                Return "Invalid Loginname und/oder Paßwort"
            Case 132813
                Return "&Konfiguration"
            Case 132828
                Return "&Nährstoffe neu kalkulieren"
            Case 132841
                Return "Zutat hinzufügen"
            Case 132846
                Return "Markierungen speichern"
            Case 132847
                Return "Markierungen laden"
            Case 132848
                Return "Filter"
            Case 132855
                Return "Menü hinzufügen"
            Case 132860
                Return "Zutat hinzufügen"
            Case 132861
                Return "Zutat modifizieren"
            Case 132864
                Return "Zutat ersetzen"
            Case 132865
                Return "Separator hinzufügen"
            Case 132877
                Return "Posten hinzufügen"
            Case 132896
                Return "Kategorien standardisieren"
            Case 132900
                Return "Preis hinzufügen"
            Case 132912
                Return "Texte standardisieren"
            Case 132915
                Return "Einheiten standardisieren"
            Case 132924
                Return "Ertragseinheiten standardisieren"
            Case 132930
                Return "Index-Kleinbilder"
            Case 132933
                Return "Rezeptliste"
            Case 132934
                Return "Letztes Rezept"
            Case 132937
                Return "Letztes Menü"
            Case 132939
                Return "Menüliste"
            Case 132954
                Return "Markensatz"
            Case 132955
                Return "Einen Markennamen aus der Liste wählen oder einen neuen Markennamen zum Abspeichern eintippen"
            Case 132957
                Return "Speichern markiert als"
            Case 132967
                Return "Nährstoff"
            Case 132971
                Return "Nährstoffübersicht"
            Case 132972
                Return "Die Nährwerte sind pro Portion auf 100%"
            Case 132974
                Return "Ausschuß"
            Case 132987
                Return "Übersicht"
            Case 132989
                Return "Zeigen"
            Case 132997
                Return "an oder vor"
            Case 132998
                Return "am oder nach"
            Case 132999
                Return "zwischen"
            Case 133000
                Return "größer als"
            Case 133001
                Return "weniger als"
            Case 133005
                Return "Festgesetzt"
            Case 133023
                Return "Optionen anzeigen"
            Case 133043
                Return "Lokale Bildtransformationen"
            Case 133045
                Return "Maximale Bilddateigröße"
            Case 133046
                Return "Maximale Bildgröße"
            Case 133047
                Return "Optimierung"
            Case 133049
                Return "Automatische Bildkonvertierung für die Verwendung auf der Website aktivieren"
            Case 133057
                Return "Logo für die Webseite hochladen"
            Case 133060
                Return "Webfarben"
            Case 133075
                Return "Neues Paßwort"
            Case 133076
                Return "Neues Paßwort bestätigen"
            Case 133078
                Return "Die Paßwörter stimmen nicht überein."
            Case 133080
                Return "Letztes"
            Case 133081
                Return "Erstes"
            Case 133085
                Return "Dokumentausgabe"
            Case 133096
                Return "Rezeptvorbereitung"
            Case 133097
                Return "Rezeptkostenermittlung"
            Case 133099
                Return "Variante"
            Case 133100
                Return "Rezepteinzelheiten"
            Case 133101
                Return "Menüeinzelheiten"
            Case 133108
                Return "Was soll gedruckt werden?"
            Case 133109
                Return "Auswahl der zu druckenden Zutat"
            Case 133111
                Return "Einige Kategorien"
            Case 133112
                Return "Markierte Zutat"
            Case 133115
                Return "Alle Rezepte"
            Case 133116
                Return "Markierte Rezepte"
            Case 133121
                Return "Markierte Menüs"
            Case 133123
                Return "Menüpreisermittlung"
            Case 133124
                Return "Menübeschreibung"
            Case 133126
                Return "EGS Standard"
            Case 133127
                Return "EGS Modern"
            Case 133128
                Return "EGS Zwei Spalten"
            Case 133133
                Return "Ungültiger Dateiname. Geben Sie bitte einen gültigen Namen ein."
            Case 133144
                Return "Rezept Nr."
            Case 133147
                Return "Liter"
            Case 133161
                Return "Papiergrösse"
            Case 133162
                Return "Einheiten für Randgröße"
            Case 133163
                Return "Linker Rand"
            Case 133164
                Return "Rechter Rand"
            Case 133165
                Return "Oberer Rand"
            Case 133166
                Return "Unterer Rand"
            Case 133168
                Return "Schriftgrösse"
            Case 133172
                Return "Kleines Bild / Menge - Name"
            Case 133173
                Return "Kleines Bild / Name - Menge"
            Case 133174
                Return "Mittleres Bild / Menge - Name"
            Case 133175
                Return "Mittleres Bild / Name - Menge"
            Case 133176
                Return "Großes Bild / Menge - Name"
            Case 133177
                Return "Großes Bild / Name - Menge"
            Case 133196
                Return "Listenoptionen"
            Case 133201
                Return "Die folgende(n) Ware(n)  wird/werden verwendet und wurde(n) nicht gelöscht."
            Case 133207
                Return "Das Rezept kann als Unterrezept verwendet werden."
            Case 133208
                Return "Gewicht"
            Case 133222
                Return "Detail Optionen"
            Case 133230
                Return "Das/die folgende(n) Rezept(e)  wird/werden verwendet und wurde(n) nicht gelöscht."
            Case 133241
                Return "Die Preisen werden neu kalkuliert. Bitte warten..."
            Case 133242
                Return "Die Nährwerte werden neu kalkuliert. Bitte warten..."
            Case 133248
                Return "Zutat"
            Case 133251
                Return "Trennzeichen"
            Case 133254
                Return "Sortieren nach"
            Case 133260
                Return "Die Quelle wird verwendet."
            Case 133266
                Return "Schlüsselworte standardisieren"
            Case 133286
                Return "Definition"
            Case 133289
                Return "Die Einheit wird verwendet."
            Case 133290
                Return "Sie können zwei oder mehrere Systemeinheiten nicht zusammenführen."
            Case 133295
                Return "Diese Einheit kann nicht gelöscht werden. " & vbCrLf & "Nur benutzerdefinierte Einheiten können gelöscht werden."
            Case 133314
                Return "Nur benutzerdefinierte Ertragseinheiten können gelöscht werden."
            Case 133315
                Return "Sie können nicht zwei oder mehrere Systemertragseinheiten zusammenführen."
            Case 133319
                Return "Die Ertragseinheit wird verwendet."
            Case 133325
                Return "Sind Sie sicher, daß Sie alle unbenutzten Kategorien beseitigen möchten?"
            Case 133326
                Return "Keine Quelle"
            Case 133328
                Return "Rezeptname"
            Case 133330
                Return "Fehlende Datei"
            Case 133334
                Return "%r wird importiert"
            Case 133349
                Return "Menü-Nr."
            Case 133350
                Return "Posten für %y (Nettomenge)"
            Case 133351
                Return "Zutaten für %y" '  zu %p% (Nettomenge)"
            Case 133352
                Return "Festgelegter Verkaufspreis pro Portion + Steuer"
            Case 133353
                Return "Festgelegter Verkaufspreis pro Portion"
            Case 133359
                Return "Sortiert nach Nummer"
            Case 133360
                Return "Sortiert nach Datum"
            Case 133361
                Return "Sortiert nach Kategorie"
            Case 133365
                Return "Verkaufspreis + Steuer"
            Case 133367
                Return "Sortiert nach Lieferanten"
            Case 133405
                Return "Bilder hochladen"
            Case 133475
                Return "Bild"
            Case 133519
                Return "Eine Farbe wählen:"
            Case 133590
                Return "&Einfügen"
            Case 133692
                Return "Empfohlener Preis"
            Case 134021
                Return "Inventuraufnahme begonnen am"
            Case 134032
                Return "Kontakt"
            Case 134054
                Return "Persönliche Informationen"
            Case 134055
                Return "Einkauf"
            Case 134056
                Return "Verkauf"
            Case 134061
                Return "Version, Module und Lizenzen"
            Case 134083
                Return "Test"
            Case 134111
                Return "Konnte die markierten Posten nicht löschen."
            Case 134174
                Return "Erstellungsdatum"
            Case 134176
                Return "Zutat-Nährstoffe-Liste"
            Case 134177
                Return "Rezepte-Nährstoffe-Liste"
            Case 134178
                Return "Menü-Nährstoffe-Liste"
            Case 134182
                Return "Gruppe"
            Case 134194
                Return "Ungültige Mengenangabe"
            Case 134195
                Return "Ungültige Preisangabe"
            Case 134320
                Return "Anschrift für Rechnungszusendung"
            Case 134332
                Return "Information"
            Case 134333
                Return "Wichtig"
            Case 134525
                Return "Sind Sie sicher, daß Sie die gemachten Änderungen verwerfen möchten?"
            Case 134571
                Return "Ungültiger Wert"
            Case 134826
                Return "Geschlossen"
            Case 135024
                Return "Standort"
            Case 135056
                Return "Nährstoffregeln"
            Case 135058
                Return "Nährstoffregel hinzufügen"
            Case 135059
                Return "Nährstoffregel modifizieren"
            Case 135070
                Return "Netto"
            Case 135100
                Return "Referenznummer"
            Case 135110
                Return "Menge" & vbCrLf & "Inventur"
            Case 135235
                Return "Lagerbestandswert"
            Case 135256
                Return "Menge verkauft"
            Case 135257
                Return "Brutto Marge"
            Case 135283
                Return "Zuletzt gewesener Preis"
            Case 135608
                Return "Port"
            Case 135948
                Return "Unterrezept(e) mit einbeziehen"
            Case 135951
                Return "Einloggen ist fehlgeschlagen."
            Case 135955
                Return "Ungültiger numerischer Wert."
            Case 135963
                Return "Datenbank"
            Case 135967
                Return "In Rezepten ersetzen."
            Case 135968
                Return "In Menüs ersetzen."
            Case 135969
                Return "Sind Sie sicher, daß Sie %o ersetzen möchten?"
            Case 135971
                Return "&Verbindung"
            Case 135978
                Return "Neue Version"
            Case 135979
                Return "Umbenennen"
            Case 135985
                Return "bestehend"
            Case 135986
                Return "Fehlt"
            Case 135989
                Return "Artikel"
            Case 135990
                Return "Aktualisieren"
            Case 136018
                Return "Besitz"
            Case 136025
                Return "Datenbank Änderung"
            Case 136030
                Return "Inhalt"
            Case 136100
                Return "Geöffnete Inventare"
            Case 136110
                Return "Geöffnet am"
            Case 136115
                Return "Anzahl Artikel"
            Case 136171
                Return "Einheit ändern"
            Case 136212
                Return "zeig Liste der benötigten Ergänzungen"
            Case 136213
                Return "füge ein Produkt dem derzeitigen Inventar hinzu"
            Case 136214
                Return "Produkt vom Inventar löschen"
            Case 136215
                Return "füge eine neue Stelle für das Produkt hinzu"
            Case 136216
                Return "Lösche den gewählten Lagerort für das Produkt"
            Case 136217
                Return "Nimm die Menge für den gewünschten Produkt-Standort weg"
            Case 136230
                Return "errichte ein neues Inventar"
            Case 136231
                Return "modifiziere Inventars Info"
            Case 136265
                Return "Unterrezepte"
            Case 136432
                Return "Ungültiger Code"
            Case 136601
                Return "Zurückstellen"
            Case 136905
                Return "Währungssymbol"
            Case 137019
                Return "Ändern"
            Case 137030
                Return "Standardwert"
            Case 137070
                Return "Allgemeine Einstellungen"
            Case 138030
                Return "Wählen Sie aus, welche Produkte Sie für dieses Inventar möchten"
            Case 138031
                Return "Alle Produkte für Inventare"
            Case 138032
                Return "Produkte der markierten Kategorien"
            Case 138033
                Return "Produkte der markierten Orte"
            Case 138034
                Return "Produkte der markierten Lieferanten"
            Case 138035
                Return "Produkte aus einem oder mehrerer vorangegangener Inventare"
            Case 138137
                Return "Gelöscht"
            Case 138244
                Return "Verkaufsartikel"
            Case 138402
                Return "Alle Übertragungen erfolgreich durchgeführt."
            Case 138412
                Return "<nicht definiert>"
            Case 140056
                Return "Datei"
            Case 140100
                Return "Backup in Arbeit"
            Case 140101
                Return "Wiederherstellung am Laufen"
            Case 140129
                Return "Fehler beim Wiedererstellen eines BackUp"
            Case 140130
                Return "Fehler beim Erstellen eines Backup"
            Case 140180
                Return "Pfad um BackUp files zu schützen"
            Case 143001
                Return "Teilen"
            Case 143002
                Return "nicht teilen"
            Case 143003
                Return "Netto-" & vbCrLf & "menge"
            Case 143008
                Return "Verlust"
            Case 143013
                Return "Änderung"
            Case 143014
                Return "Benutzer"
            Case 143508
                Return "Rezept wird als Unterrezept angewendet"
            Case 143509
                Return "Zeilenabstand"
            Case 143981
                Return "ungültiger Buchungscode"
            Case 143987
                Return "Postenart"
            Case 143995
                Return "Los!"
            Case 144582
                Return "keine Gruppe"
            Case 144591
                Return "Zeit"
            Case 144682
                Return "Nährwerte pro 100g oder 100ml auf 100%"
            Case 144684
                Return "Nährwerte sind pro Ertragseinheit auf 100%"
            Case 144685
                Return "pro Rezepteinheit zu100%"
            Case 144686
                Return "pro %Y zu 100%"
            Case 144687
                Return "pro 100g oder 100ml zu 100%"
            Case 144688
                Return "N/A"
            Case 144689
                Return "Nährwerte sind für 1 Rezepteinheit/100g oder 100ml zu 100%"
            Case 144716
                Return "Vorgeschichte"
            Case 144734
                Return "POS(Kassen) Artikelliste"
            Case 144738
                Return "Gewicht per %Y"
            Case 145006
                Return "Übertragung"
            Case 146043
                Return "Januar"
            Case 146044
                Return "Februar"
            Case 146045
                Return "März"
            Case 146046
                Return "April"
            Case 146047
                Return "May"
            Case 146048
                Return "Juni"
            Case 146049
                Return "July"
            Case 146050
                Return "August"
            Case 146051
                Return "September"
            Case 146052
                Return "Oktober"
            Case 146053
                Return "November"
            Case 146054
                Return "Dezember"
            Case 146056
                Return "Deckungsbeitrag"
            Case 146067
                Return "Saldo"
            Case 146080
                Return "Kunde(n)"
            Case 146114
                Return "Neue Seite anzeigen bei verschiedene Lieferanten"
            Case 146211
                Return "Ausgaben"
            Case 147070
                Return "OK"
            Case 147075
                Return "Ungültiges Datum"
            Case 147126
                Return "zuerst bestehende Markierungen entfernen"
            Case 147174
                Return "Offen"
            Case 147381
                Return "Inventarpreis benutzt für vorangegangene Produkte"
            Case 147441
                Return "Dieser Verkaufsartikel wurde schon verbunden"
            Case 147462
                Return "Verhältnis"
            Case 147520
                Return "Haupt"
            Case 147647
                Return "SQL Server existiert nicht, oder Zugang verwehrt"
            Case 147652
                Return "Entfernen"
            Case 147692
                Return "Mahlzeit Information"
            Case 147699
                Return "Überschreiben"
            Case 147700
                Return "Gesamtpreis"
            Case 147703
                Return "Anzahl zubereitete Portionen"
            Case 147704
                Return "Übriggebliebene Menge"
            Case 147706
                Return "Zurückgegebene Menge"
            Case 147707
                Return "Verlorene Menge"
            Case 147708
                Return "Verkaufte Menge"
            Case 147710
                Return "Verkauft Menge (Spezial)"
            Case 147713
                Return "EGS Stil"
            Case 147727
                Return "Kosten"
            Case 147729
                Return "Evaluation"
            Case 147733
                Return "Wählen Sie eine Sprache"
            Case 147737
                Return "Menge eingeben und Einheit wählen"
            Case 147743
                Return "Rezept zum Server laden"
            Case 147748
                Return "Anonym"
            Case 147750
                Return "Kommentare"
            Case 147753
                Return "Arbeitskosten"
            Case 147771
                Return "Preis/Std"
            Case 147772
                Return "Preis/Min"
            Case 147773
                Return "Person"
            Case 147774
                Return "Zeit (Stunden:Minuten)"
            Case 149501
                Return "Direkter Eingang-Ausgang verwenden"
            Case 149513
                Return "Zustimmung"
            Case 149531
                Return "Fertigprodukte"
            Case 149645
                Return "Verbunden mit"
            Case 149706
                Return "Verbindung löschen"
            Case 149761
                Return "Zeigen"
            Case 149766
                Return "Prefix"
            Case 149774
                Return "Löschen"
            Case 150009
                Return "Export fertig. Rezept-Export erfolgreich durchgeführt."
            Case 150333
                Return "Erfolgreich gelöscht!"
            Case 150341
                Return "Währungsumstellung"
            Case 150353
                Return "Sortieren"
            Case 150634
                Return "E-Mail erfolgreich versandt."
            Case 150644
                Return "Der SMTP Server wird zum versenden der E-Mail benutzt."
            Case 150688
                Return "Die Lizenz für dieses Programm ist ausgelaufen"
            Case 150707
                Return "Firma"
            Case 151011
                Return "Schweiz - Hauptsitz"
            Case 151019
                Return "Ware Schlüsselwörter"
            Case 151020
                Return "Rezept Schlüsselwort"
            Case 151023
                Return "Registrieren"
            Case 151250
                Return "Nichts hat geändert"
            Case 151286
                Return "Standard"
            Case 151299
                Return "Bitte erfordliche Information eingeben"
            Case 151322
                Return "In Inventar eingeben."
            Case 151336
                Return "Markensatz laden"
            Case 151344
                Return "Zutatmarken speichern"
            Case 151345
                Return "Rezeptmarken speichern"
            Case 151346
                Return "Menümarken speichern"
            Case 151364
                Return "Wählen Sie ein oder mehere Texte"
            Case 151389
                Return "Texte Bereinigen"
            Case 151400
                Return "Zutatkosten"
            Case 151404
                Return "MwSt"
            Case 151424
                Return "Konvertiere zu beste Einheit"
            Case 151427
                Return "Nach Name sortiert"
            Case 151435
                Return "Betreff"
            Case 151436
                Return "Anhang"
            Case 151437
                Return "CALCMENU"
            Case 151438
                Return "CALCMENU"
            Case 151459
                Return "Eigene E-Mail"
            Case 151499
                Return "Angebot ersetzen"
            Case 151500
                Return "Angebot"
            Case 151854
                Return "Excel"
            Case 151886
                Return "enn Sie irgendwelche Fragen über die Anmeldung haben, bitte E-Mail senden an: %email"
            Case 151890
                Return "Hallo %name"
            Case 151906
                Return "E-Mail Adresse nicht gefunden"
            Case 151907
                Return "Bitte Login votre Benutzername und Kennwort."
            Case 151910
                Return "Anmelden"
            Case 151911
                Return "Abmelden"
            Case 151912
                Return "Kennwort vergessen?"
            Case 151915
                Return "Bitte geben Sie die unten abgefragten Informationen ein."
            Case 151916
                Return "Felder mit * sind notwendig."
            Case 151917
                Return "Eine Bestätigung wird Ihnen per E-Mail gesendet."
            Case 151918
                Return "Geben Sie bitte eine gültige E-Mail Adresse an."
            Case 151920
                Return "Ja möchte ich periodische E-mail von EGS über neue Produkte oder Promotionen (nicht mehr als einmal im Monat) empfangen."
            Case 151976
                Return "Standard Produktionsort"
            Case 152004
                Return "Verzweigung"
            Case 152141
                Return "Zutatverwaltung"
            Case 152146
                Return "Plz"
            Case 155024
                Return "Bildverwaltung"
            Case 155046
                Return "Übersetzung"
            Case 155050
                Return "ALLE SCHLÜSSELWÖRTER"
            Case 155052
                Return "Übermitteln"
            Case 155118
                Return "Einkaufsliste in Pocken PC senden"
            Case 155163
                Return "Familienname"
            Case 155170
                Return "Willkommen %name!"
            Case 155205
                Return "Hauptseite"
            Case 155225
                Return "PDF"
            Case 155236
                Return "Hauptsprache"
            Case 155245
                Return "Über uns"
            Case 155260
                Return "Fester Faktor"
            Case 155263
                Return "Pixel"
            Case 155264
                Return "Übersetzen"
            Case 155374
                Return "Buchhaltungsnummer"
            Case 155507
                Return "Erlauben"
            Case 155575
                Return "Standard Lagerort für auto. Ausg."
            Case 155601
                Return "Nichts ausgewählt."
            Case 155642
                Return "Die Rezeptbörse (Recipe Exchange)"
            Case 155654
                Return "Zutaten für %s %u zu %p% (Netto Menge)"
            Case 155713
                Return "%r bestehend"
            Case 155731
                Return "CALCMENU Pro"
            Case 155761
                Return "Ware importieren"
            Case 155763
                Return "Vergleich mit Nummer"
            Case 155764
                Return "Vergleich mit Name"
            Case 155811
                Return "Brutto-" & vbCrLf & "menge"
            Case 155841
                Return "Datei zum Wiederhestellen"
            Case 155842
                Return "Personenanzahl"
            Case 155861
                Return "Alle Mengen der ausgewählten Artikel auf Null zurücksetzen"
            Case 155862
                Return "pro"
            Case 155926
                Return "Export zu Excel"
            Case 155927
                Return "Alle Quellen"
            Case 155942
                Return "Liste der gespeicherten Einkaufslisten laden"
            Case 155947
                Return "Gefiltert nach"
            Case 155967
                Return "Datei Separator"
            Case 155994
                Return "Nicht aktiv"
            Case 155995
                Return "Kontrolle..."
            Case 155996
                Return "E-Mail Adresse"
            Case 156000
                Return "Zu neuem Lieferant verschieben"
            Case 156012
                Return "Unterstützung"
            Case 156015
                Return "Kontakt"
            Case 156016
                Return "Hauptbüro"
            Case 156060
                Return "Festgesetzte WK"
            Case 156061
                Return "Fest. Brutto Erfolg"
            Case 156141
                Return "Backup/Rückgewinnung einer Datenbank"
            Case 156337
                Return "Nährwerteverbindung"
            Case 156344
                Return "Ungültige Auswahl"
            Case 156355
                Return "Archiven"
            Case 156356
                Return "Einschliessen"
            Case 156405
                Return "Ungenügend PLatz auf der Festplatte. Bitte machen Sie Platz und klicken auf Wiederholen."
            Case 156413
                Return "Unterrezept Definition"
            Case 156485
                Return "Lösche Dateien nach Import"
            Case 156542
                Return "Gewogener Durchschnittspreis"
            Case 156552
                Return "Jetzt sichern"
            Case 156590
                Return "Importiere Zutat von CSV Datei (Excel)"
            Case 156669
                Return "Web Site"
            Case 156672
                Return "Online benutzt (Für Web Inhalt)"
            Case 156683
                Return "Ursprünglich"
            Case 156720
                Return "Zu lange Zahl"
            Case 156721
                Return "Zu langer Name"
            Case 156722
                Return "Zu langer Lieferant"
            Case 156723
                Return "Zu lange Kategorie"
            Case 156725
                Return "Zu lange Beschreibung"
            Case 156734
                Return "Zwei Einheiten sind identisch"
            Case 156742
                Return "Läuft danach ab"
            Case 156751
                Return "Direct line: +41 32 544 0017" & "<br><br>24/7 English Customer Support: +1 800 964 9357<br><br>Sales: +41 848 000 357" & "<br>Fax: +41 32 753 0275"
            Case 156752
                Return "24/7 Toll Free: +1-800-964-9357"
            Case 156753
                Return "Office line +632 687 3179"
            Case 156754
                Return "Dateiname"
            Case 156784
                Return "Gesamt Störungen: %n"
            Case 156825
                Return "Tausend"
            Case 156870
                Return "Sind Sie sicher?"
            Case 156892
                Return "Download:"
            Case 156925
                Return "Download OK!"
            Case 156938
                Return "Aktiv"
            Case 156941
                Return "Pocket Kitchen"
            Case 156955
                Return "Privat"
            Case 156957
                Return "Hotels"
            Case 156959
                Return "Geteilt"
            Case 156960
                Return "Eingereicht"
            Case 156961
                Return "Preissätze"
            Case 156962
                Return "Nicht Eingereicht"
            Case 156963
                Return "Preise"
            Case 156964
                Return "Suchen in"
            Case 156965
                Return "Ergebnisse"
            Case 156966
                Return "Aufzeichnungen beeinflußt"
            Case 156967
                Return "Korrektes Datum bitte einegeben"
            Case 156968
                Return "Unzulässiges Bilddatei Format"
            Case 156969
                Return "Die Bilddatei zum hochladen eingeben. Andernfalls freilassen."
            Case 156970
                Return "Kategorie Informationen Eintragen"
            Case 156971
                Return "Gesetzte Preis- Informationen Eintragen"
            Case 156972
                Return "Schlüsselwort- Informationen Eintragen"
            Case 156973
                Return "Einheit Informationen Eintragen"
            Case 156974
                Return "Ergebnis Eintragen"
            Case 156975
                Return "Neue Rezepte erstellen und  beim Hauptbüro für Gebrauch mit anderen Hotels einreichen."
            Case 156976
                Return "Zutat sind das grundlegende Element von CALCMENU Web. Dies sind die Einzelteile, die Ihre Rezepte enthalten."
            Case 156977
                Return "Wenn Sie Anfragen oder  Fachfragen über diese Software haben."
            Case 156978
                Return "Elternteil- Schlüsselwort"
            Case 156979
                Return "Name  des Schlüsselwortes"
            Case 156980
                Return "Konfiguration"
            Case 156981
                Return "Steuersätze"
            Case 156982
                Return "Such resultate"
            Case 156983
                Return "Keine Resultate gefunden."
            Case 156984
                Return "Unzulässiges Benutzername oder Kennwort."
            Case 156986
                Return "besteht bereits."
            Case 156987
                Return "wurde erfolgreich gespeichert."
            Case 156996
                Return "Copyright © 2004 of EGS Enggist & Grandjean Software SA, Schweiz."
            Case 157002
                Return "Preis für die Maßeinheit nicht definiert. Eine Einheit bitte vorwählen."
            Case 157020
                Return "Angewendete Steuer"
            Case 157026
                Return "Mittel"
            Case 157033
                Return "Das System  aktualisiert die Preise  aller Zutat. Bitte warten..."
            Case 157034
                Return "Authentisierung"
            Case 157038
                Return "Monatl."
            Case 157039
                Return "Yährl."
            Case 157040
                Return "Es gibt kein Schlüsselwort."
            Case 157041
                Return "Zugriff verweigert"
            Case 157049
                Return "Sind Sie sicher dass Sie speichern möchten ?"
            Case 157055
                Return "STUDENTENVERSION"
            Case 157056
                Return "Wollen Sie wirklich annulieren?"
            Case 157057
                Return "Die markierten Einträge sind jetzt freigegeben."
            Case 157060
                Return "Referenznummer"
            Case 157065
                Return "Export zu CALCMENU"
            Case 157066
                Return "Export zu CALCMENU"
            Case 157076
                Return "Inhalt der Hilfe"
            Case 157079
                Return "Die folgenden markierten Einträge sind nicht versandt und können nicht übertragen werden:"
            Case 157084
                Return "Die folgenden markierten Einträge werden benutzt und können nicht gelöscht werden:"
            Case 157125
                Return "Betrachten"
            Case 157130
                Return "Ihre Kreditkarteninformationen wurden erfolgreich übertragen. Ihre Bestellung wird innerhalb von drei Tagen bearbeitet. Vielen Dank!"
            Case 157132
                Return "Persönlich (Freigegeben)"
            Case 157133
                Return "Persönlich (Nicht freigegeben)"
            Case 157134
                Return "Besucher"
            Case 157136
                Return "Verweise"
            Case 157139
                Return "Schlecht!"
            Case 157140
                Return "Gut!"
            Case 157141
                Return "Fantastisch!"
            Case 157142
                Return "Ungenutzte Einheiten vor dem Import löschen"
            Case 157151
                Return "Andere Verknüpfungen"
            Case 157152
                Return "Benutzerbesprechungen"
            Case 157153
                Return "Der Empfänger wird aufgefordert, die Einträge zu akzeptieren."
            Case 157154
                Return "Die folgenden Einträge können nicht vergeben werden, da Sie anderen Benutzern gehören"
            Case 157155
                Return "Jemand möchte Ihnen die folgenden Rezepte geben:"
            Case 157156
                Return "Promo"
            Case 157157
                Return "Benutzermeinungen"
            Case 157158
                Return "Originalität"
            Case 157159
                Return "Ergebnis"
            Case 157160
                Return "Schwierigkeit"
            Case 157161
                Return "Rezept des Tages"
            Case 157164
                Return "Name des Karteninhabers"
            Case 157165
                Return "Kreditkartennummer"
            Case 157166
                Return "Datensatzbegrenzung"
            Case 157168
                Return "Bank"
            Case 157169
                Return "PayPal"
            Case 157170
                Return "Die Online-Bestellung ist nicht in Ihrem Land vorhanden."
            Case 157171
                Return "Werden Sie Mitglied"
            Case 157172
                Return "Upgrade-Kosten"
            Case 157173
                Return "Mitgliedsbeitrag"
            Case 157174
                Return "Upgrade-Pakete"
            Case 157176
                Return "Summe der benutzten Datensätze"
            Case 157177
                Return "Wir bieten eine Reihe von Lösungen, die zu Ihren Anforderungen passen"
            Case 157178
                Return "Probebenutzer"
            Case 157179
                Return "Einem Bekannten mitteilen"
            Case 157180
                Return "E-Mail-Adresse eines Freundes"
            Case 157182
                Return "FAQs"
            Case 157183
                Return "Allgemeine Geschäftsbedingungen"
            Case 157214
                Return "Erzeugen einer Einkaufsliste für markierte Rezepte allein"
            Case 157217
                Return "Erzeugen einer Einkaufsliste für markierte Menüs allein"
            Case 157226
                Return "Die markierten Rezepte wurden zur Genehmigung versandt."
            Case 157233
                Return "Der Gesamtausschuß kann nicht größer/gleich 100% sein."
            Case 157268
                Return "Benutzte Währung."
            Case 157269
                Return "Es werden Preisgruppen benutzt"
            Case 157273
                Return "Kann die folgenden Einträge nicht freigeben, da sie nicht übermittelt sind und Ihnen nicht gehören."
            Case 157274
                Return "Wechselkurs"
            Case 157275
                Return "Alle angezeigten Einträge werden zu einem Eintrag zusammengefasst. Bitte wählen Sie einen Eintrag aus, der von den Benutzern genutzt wird. Andere Einträge werden aus der Datenbank entfernt."
            Case 157276
                Return "Erfolgreich verbunden."
            Case 157277
                Return "Summe der Kosten"
            Case 157281
                Return "Preis des Standardlieferanten"
            Case 157297
                Return "Bitte wählen Sie mindestens einen Eintrag aus."
            Case 157299
                Return "Bearbeiten Sie das Profil, und passen Sie Ihre Anzeige an."
            Case 157300
                Return "Bitte geben Sie Ihr neues Kennwort ein. Ein Kennwort kann nicht länger als 20 Zeichen sein. Klicken Sie auf 'Senden', wenn Sie fertig sind."
            Case 157301
                Return "Bitte geben Sie den Namen der Bilddatei (jpeg/jpg, bmp etc.) ein, die Sie hochladen wollen. Lassen Sie das Feld anderenfalls frei. (Hinweis: GIF-Dateien werden nicht unterstützt. Alle Bilder werden kopiert und dann in das normale und Vorschau.-JPEG-Format konvertiert. )"
            Case 157302
                Return "Suchen Sie Zutaten nach dem Namen oder den Anfangsbuchstaben. Um Zutat schnell hinzuzufügen, geben sie Nettomenge, Einheit und Namen ein.  z. B. 200 g Oel High Oleic"
            Case 157303
                Return "Um den Zutatpreis hinzuzufügen oder auszudrucken, geben Sie den neuen Preis ein und definieren Sie die Masseinheit. Sie müssen das Verhältnis dieser Masseinheit der ursprünglichen Masseinheit zuweisen. Z.B. der ursprüngliche Preis und die Masseinheit beträgt US$ 11.00 pro Kilogramm. Wenn Sie z.B. einen Beutel hinzufügen möchten, müssen Sie den Preis dieses Beutels definieren, oder die Menge in Kilogramm, die im Beutel enthalten sind, definieren"
            Case 157304
                Return "Suchen Sie Schlüsselbegriffe nach dem ganzen oder einem teilweisen Namen. Benutzen Sie ein Komma [ , ], um mehrere Schlüsselworte zu suchen. Ein Beispiel: Suchen Sie ""Fleisch, Soße, Hochzeit""."
            Case 157305
                Return "Bitte wählen Sie einen Eintrag aus."
            Case 157306
                Return "Ungültiger Dateityp"
            Case 157310
                Return "Zutatdetails"
            Case 157314
                Return "Beim Hinzufügen von Zutatpreisen die Haupteinheit benutzen"
            Case 157320
                Return "Freigeben"
            Case 157322
                Return "Lizenzvereinbarung"
            Case 157323
                Return "Geben"
            Case 157329
                Return "Terminal"
            Case 157334
                Return "Warnung: Sie verlieren möglicherweise alle Änderungen, wenn ein anderen Benutzer diesen Datensatz gelöscht hat. Wollen Sie diese Seite aktualisieren?"
            Case 157336
                Return "Nicht anwendbar"
            Case 157339
                Return "Nachrichten pro Seite"
            Case 157340
                Return "Quick-Browse"
            Case 157341
                Return "auf jeder Seite"
            Case 157342
                Return "Der Datensatz wurde von einem anderen Benutzer verändert. Klicken Sie auf OK, um weiterzumachen."
            Case 157343
                Return "Dieser Datensatz wurde von einem anderen Benutzer gelöscht."
            Case 157345
                Return "An Hauptbüro senden"
            Case 157346
                Return "Nicht freigegeben"
            Case 157378
                Return "Mitglied"
            Case 157379
                Return "Jetzt abonnieren"
            Case 157380
                Return "Ihre Mitgliedschaft läuft am %n ab."
            Case 157381
                Return "Ihre Mitgliedschaft ist abgelaufen."
            Case 157382
                Return "Erweitern Sie meine Mitgliedschaft mit meinen restlichen Creditpoints."
            Case 157383
                Return "Ungenügend PLatz auf der Festplatte. Bitte machen Sie Platz und klicken auf Wiederholen."
            Case 157384
                Return "Ungültige Transaktion"
            Case 157385
                Return "Vielen Dank!"
            Case 157387
                Return "Sie werden zu Paypal weitergeleitet, um die Bestellung abzuschließen. Bitte wählen Sie an diesem Punkt die Währung, damit die Bestellung in der richtigen Höhe berechnet werden kann. Bitte wählen Sie aus der Liste unten."
            Case 157388
                Return "Eine Einladung zum Beitritt."
            Case 157404
                Return "Laufende Transaktion."
            Case 157405
                Return "Für eine Anfrage senden Sie uns bitte eine E-Mail nach"
            Case 157408
                Return "Nur Mitglieder und Nutzer der Demoversion können auf diese Seite zugreifen. Wollen Sie Ihre eigenen Rezepte auf RecipeGallery.com verwalten? Gehen Sie zum Bestellmenü und beantragen Sie die Mitgliedschaft."
            Case 157435
                Return "Automatischer Transfer zur Verkaufsstelle vor der Ausgabe"
            Case 157437
                Return "Rohmaterial"
            Case 157446
                Return "Monat"
            Case 157515
                Return "Niederländisch"
            Case 157594
                Return "Akzeptieren"
            Case 157595
                Return "Ablehnen"
            Case 157596
                Return "Keine Benutzerbesprechung"
            Case 157604
                Return "E-Mail-Support"
            Case 157607
                Return "Telefon-Support"
            Case 157608
                Return "Online-Support"
            Case 157616
                Return "USA"
            Case 157617
                Return "ASIEN und den Rest der Welt"
            Case 157629
                Return "Bewerten"
            Case 157633
                Return "Verwerfen"
            Case 157659
                Return "Sperren"
            Case 157660
                Return "Entsperren"
            Case 157695
                Return "Rechn. Nr."
            Case 157714
                Return "Kommentare"
            Case 157772
                Return "Optional"
            Case 157793
                Return "Info"
            Case 157802
                Return "Kennwort bestätigen"
            Case 157901
                Return "Verstecke Bestehendes"
            Case 157926
                Return "Registrieren"
            Case 157985
                Return "Sie können das Kennwort entsprechend der folgenden Arbeitsanweisungen ändern:"
            Case 157986
                Return "Melden Sie sich auf der EGS-Website unter <a href='http://www.eg-software.com/d/'>http://www.eg-software.com/d/</a> an."
            Case 157992
                Return "Sie haben kürzlich den Benutzernamen und das Kennwort angefordert, um sich auf Ihrem EGS-Benutzerkonto anmelden zu können."
            Case 157993
                Return "Bitte beachten Sie die unten stehenden Details"
            Case 158005
                Return "Lizenzen"
            Case 158019
                Return "Status der Anfrage überprüfen"
            Case 158157
                Return "Zutaten für %y"
            Case 158169
                Return "Bitte wählen Sie das Zahlungsverfahren aus." & vbCrLf & "" & vbCrLf & "Zahlung mit/über:"
            Case 158170
                Return "Senden Sie uns bitte die Kreditkartendetails per E-Mail an<a href='mailto:info@calcmenu.com'>info@calcmenu.com</a>. Kreditkartentyp (Visa, Mastercard/Eurocard, American Express), Name auf Kreditkarte, Kreditkartennummer und Ablaufdatum."
            Case 158171
                Return "Überweisung"
            Case 158174
                Return "Hinweis: Bitte informieren Sie uns, sobald die Überweisung ausgeführt wurde. Es dauert 1-2 Wochen, bevor wir von der Bank Informationen über die Überweisung bekommen."
            Case 158186
                Return "Kennwort ändern"
            Case 158216
                Return "Webbasiertes Rezeptverwaltungssystem"
            Case 158220
                Return "Erzeugen Sie neue Zutat mit einem Namen aus bis zu 250 Zeichen, und fügen Sie eine alphanumerische Referenz, den Steuersatz, vier Verbrauchsprozentsätze, Kategorie, Lieferant und weitere nützliche Informationen wie Produktbeschreibung, Zubereitung, Kochtipps, Verbesserungsvorschläge und Lagerung hinzu."
            Case 158229
                Return "Bilder"
            Case 158230
                Return "Ware, Rezepte und Menüs werden anhand ihres Namens oder ihrer Referenznummer gesucht. Außerdem sind die Kategorien und Schlüsselworte durchsuchbar. Bei Zutat wird zusätzlich nach Lieferanten, dem Datum der Eingabe oder letzten Änderung, dem Preisbereich und der Nährstoffwerte gesucht. Bei Rezepten und Menüs suchen Sie nach den benutzten oder unbenutzten Filtern."
            Case 158232
                Return "Aktionsmarkierungen sind Abkürzungen für ähnliche Funktionen, die auf markierte Zutat, Rezepte oder Menüs angewendet werden können. Sie nutzen Aktionsmarkierungen, um Zutat, Rezepten oder Menüs eine Kategorie und Schlüsselworte zuzuweisen, zu löschen, zu exportieren, per E-Mail, zu drucken, an andere Benutzer freizugeben und die Freigabe wieder aufzuheben ohne das diese Aktion für alle Objekte wiederholt werden muss. Dies spart eine Menge Zeit und Aufwand beim Ausführen von Aktionen für die markierten Einträge."
            Case 158234
                Return "Verknüpfung mit Nährstoffen und Nährstoffberechnung"
            Case 158238
                Return "Lieferantenverwaltung"
            Case 158240
                Return "Kategorie, Schlüsselworte, Quellenverwaltung"
            Case 158243
                Return "Steuersatzverwaltung"
            Case 158246
                Return "Einheitenverwaltung"
            Case 158249
                Return "Drucken und Exportieren von PDF- und Excel-Dateien"
            Case 158306
                Return "Wählen Sie aus"
            Case 158346
                Return "mehr"
            Case 158349
                Return "Zugeordnetes Schlüsselwort"
            Case 158350
                Return "Abgeleitetes Schlüsselwort"
            Case 158376
                Return "Theoretisch auferlegter Verkaufspreis"
            Case 158410
                Return "Wenn einige Produkte keinen definierten Preis haben (Preis = 0), benutzen Sie den Preis des Standardlieferanten."
            Case 158511
                Return "Wenn Sie vermuten, das dies nicht der Fall ist, senden Sie uns bitte eine E-Mail <a href='mailto:%email'>%email</a>"
            Case 158577
                Return "Site-Sprache"
            Case 158585
                Return "Hauptbüro"
            Case 158588
                Return "Sie können die folgende Elemente nicht senden, weil Sie nicht der Eigentümer sind"
            Case 158653
                Return "Handy"
            Case 158677
                Return "Verkaufseintrag" & vbCrLf & "Nummer"
            Case 158694
                Return "Info ändern"
            Case 158696
                Return "Nur für Kunden aus den Philippinen"
            Case 158730
                Return "Ausschließen"
            Case 158734
                Return "Die Datenbankversion ist nicht kompatibel mit dieser Programmversion."
            Case 158783
                Return "Rezept(e)/Unterrezept(e) einschließen"
            Case 158810
                Return "Preis berechnen"
            Case 158835
                Return "Sortiert nach Steuer"
            Case 158837
                Return "Sortiert nach Preis"
            Case 158839
                Return "Sortiert nach Kosten der Zutat"
            Case 158840
                Return "Sortiert nach Konstante"
            Case 158845
                Return "Sortiert nach Verkaufspreis"
            Case 158846
                Return "Sortiert nach auferlegtem Preis"
            Case 158849
                Return "Hoch"
            Case 158850
                Return "Niedrig"
            Case 158851
                Return "Erzeugt von"
            Case 158860
                Return "POS-Einstellung ändern"
            Case 158868
                Return "Chinesisch"
            Case 158902
                Return "Öffnungszeit"
            Case 158912
                Return "Anforderungen"
            Case 158935
                Return "Summe der Erlöse"
            Case 158946
                Return "Verfügbare Menge als Inventurmenge setzen"
            Case 158947
                Return "Sie werden zu Paypal weitergeleitet, um die Bestellung abzuschließen."
            Case 158952
                Return "Geprüft"
            Case 158953
                Return "Nicht geprüft"
            Case 158960
                Return "iese Funktion wurde abgeschaltet. Bitte nehmen Sie mit Ihrem Hauptsitz Kontakt auf, wenn Sie neue Rezepte benötigen."
            Case 158998
                Return "Suchfunktionen"
            Case 158999
                Return "Zutat, Rezepte und Menülisten werden auf Wunsch zusammen mit den Details, Preisen und Nährstoffwerten ausgedruckt. Einkaufslisten oder eine Liste der Zutaten können ebenfalls ausgedruckt werden, zusammen mit den addierten Mengen der verschiedenen Rezepte. Darüber hinaus können verschiedene Berichte als PDF- und Excel-Dateien erzeugt werden."
            Case 159000
                Return "Preisgruppen und Verwaltung mehrerer Währungen"
            Case 159009
                Return "Rahmen"
            Case 159035
                Return "Unvollständig"
            Case 159064
                Return "Name darf nicht leer sein"
            Case 159082
                Return "Aktualisiere Produkte, die auf dem letzten geänderten Datum basieren"
            Case 159088
                Return "Senden Sie Antrag für Zustimmung"
            Case 159089
                Return "Annullieren Sie Antrag für Zustimmung"
            Case 159112
                Return "Zur Prüfung"
            Case 159113
                Return "Vererbbar"
            Case 159133
                Return "Lieferinformationen"
            Case 159139
                Return "Komposition"
            Case 159140
                Return "Einheit zu lang"
            Case 159141
                Return "Einheit %n existiert nicht."
            Case 159142
                Return "%n darf nicht leer sein"
            Case 159144
                Return "Datei importieren. Bitte warten ..."
            Case 159145
                Return "Speichern der Einträge. Bitte warten ..."
            Case 159162
                Return "&Details verstecken"
            Case 159168
                Return "Sortiert nach Nettomenge"
            Case 159169
                Return "Sortiert nach Bruttomenge"
            Case 159171
                Return "Zeitplan"
            Case 159181
                Return "Sortiert nach Anzahl"
            Case 159264
                Return "Zutat-/Lieferantennetzwerk importieren"
            Case 159273
                Return "Summe der Bruttogewinne"
            Case 159274
                Return "Nur %number"
            Case 159275
                Return "Durch Lizenzen begrenzt"
            Case 159298
                Return "Menüschlüsselwort"
            Case 159349
                Return "Filter zurücksetzen"
            Case 159350
                Return "Ihr Support- und Update-Plan ist abgelaufen."
            Case 159360
                Return "Region-Chef"
            Case 159361
                Return "Küchenchef"
            Case 159362
                Return "Ausgewählter Eintrag wird genutzt."
            Case 159363
                Return "Markeninformation eingeben"
            Case 159364
                Return "Marke"
            Case 159365
                Return "Rolle"
            Case 159366
                Return "Verwenden der SMTP auf dem Server"
            Case 159367
                Return "Verwenden der SMTP im Netzwerk"
            Case 159368
                Return "Logo"
            Case 159369
                Return "Vergleich mit"
            Case 159370
                Return "erfolgreich importiert"
            Case 159372
                Return "Global"
            Case 159379
                Return "aufsteigend"
            Case 159380
                Return "absteigend"
            Case 159381
                Return "An alle Benutzer"
            Case 159382
                Return "Zu Systemrezept konvertieren"
            Case 159383
                Return "Nicht ausstellen"
            Case 159384
                Return "Region"
            Case 159385
                Return "Eintrag absenden"
            Case 159386
                Return "Preise und Nährstoffe werden nicht neu berechnet."
            Case 159387
                Return "Preise und Nährstoffe werden neu berechnet."
            Case 159388
                Return "Erzeugen einer neuen Menükarte"
            Case 159389
                Return "Menükarte ändern."
            Case 159390
                Return "E-mail gesendet."
            Case 159391
                Return "Geprüfter Preis"
            Case 159424
                Return "iese Funktion wurde abgeschaltet. Bitte nehmen Sie mit Ihrem Hauptsitz Kontakt auf, wenn Sie neue Zutat benötigen."
            Case 159426
                Return "Suchen Sie Zutaten nach dem Namen oder den Anfangsbuchstaben. Um Zutat schnell hinzuzufügen, geben sie Nettomenge, Einheit und Namen ein."
            Case 159430
                Return "Registrierungsinformation wurde erfolgreich gesichert."
            Case 159433
                Return "An System senden"
            Case 159434
                Return "An System gesendet"
            Case 159435
                Return "Markierte zu einer neuen Kategorie übergeben"
            Case 159436
                Return "E-Mail an Absender für System-Warnmeldungen"
            Case 159437
                Return "Datei wurde erfolgreich hochgeladen."
            Case 159444
                Return "Bildgröße festlegen"
            Case 159445
                Return "Zeitzone"
            Case 159446
                Return "Bildbearbeitung"
            Case 159457
                Return "SQL Server -Ganztextsuche hat die Fähigkeit, komplizierte Suchen mit Textdaten durchzuführen, die auch eine Bedeutungsklassifizierung einschließen (wenn die Wörter ähnlich sind). Ein Beispiel: Die Suche nach ""Tomate"" findet auch ""Tomatensuppe"". SQL 2009 bietet eine Sortierung der Suche nach Name, Notiz, Anleitung und Zutaten."
            Case 159458
                Return "Gesamtbevölkerung"
            Case 159459
                Return "Ganztextsuche"
            Case 159460
                Return "Minute"
            Case 159461
                Return "Jede"
            Case 159462
                Return "Starten Sie"
            Case 159463
                Return "Zuwachs der  Bevölkerung"
            Case 159464
                Return "Wortunterbrecher"
            Case 159468
                Return "Als Zutat genutzt"
            Case 159469
                Return "Nicht als Zutat genutzt"
            Case 159471
                Return "IP Adresse"
            Case 159472
                Return "Liste der blockierten IP-Adressen"
            Case 159473
                Return "IP blockieren, wenn max. Login-Versuche erreicht"
            Case 159474
                Return "Bitte geben Sie einen Benutzertitel mit mindestens  Schriftzeichen ein"
            Case 159485
                Return "An Recipe Exchange senden"
            Case 159486
                Return "Eingereicht an Rezeptbörse (Recipe Exchange)"
            Case 159487
                Return "Dieses Rezept wurde geprüft. Es kann jetzt von allen Benutzer gesehen werden."
            Case 159488
                Return "Unbekannte Sprache"
            Case 159594
                Return "Zum Rezept &hinzufügen"
            Case 159607
                Return "Einzelplatz-Rezeptverwaltungs-Software"
            Case 159608
                Return "Rezeptverwaltungs-Software für mehrere Benutzer in einem Netzwerk"
            Case 159609
                Return "Webbasiertes Rezeptverwaltungssoftware"
            Case 159610
                Return "Software für Inventur und Backoffice-Verwaltung"
            Case 159611
                Return "Rezept-Viewer für Pocket PC"
            Case 159612
                Return "Software zur Bestellaufnahme und Nährstoffüberwachung"
            Case 159613
                Return "E-Kochbuch-Software"
            Case 159681
                Return "Rezept (%s) hat zu viele Zutaten. (Max. %n)"
            Case 159689
                Return "Mit Bild gesendet."
            Case 159690
                Return "Ohne Bild gesendet."
            Case 159699
                Return "Existierende Einträge aktualisieren"
            Case 159700
                Return "&Rezept importieren"
            Case 159707
                Return "Frankreich"
            Case 159708
                Return "Deutschland"
            Case 159733
                Return "Artikel Nr."
            Case 159751
                Return "Standort"
            Case 159778
                Return "Erweitert"
            Case 159779
                Return "Basis"
            Case 159782
                Return "Verkaufseinträge mit Produkten verknüpfen"
            Case 159783
                Return "Verkaufseinträge mit Rezepten/Menüs verknüpfen"
            Case 159795
                Return "Kassen Import - Konfiguration"
            Case 159918
                Return "Sie haben keine Zugriffsrechte für diese Funktion. Bitte setzen Sie sich mit Ihrem Administrator in Verbindung, um Ihre Rechte zu ändern."
            Case 159924
                Return "Verwalten"
            Case 159925
                Return "Ungültige Konvertierung"
            Case 159929
                Return "Seitenoptionen"
            Case 159934
                Return "Nährwertinformation mit einbeziehen"
            Case 159940
                Return "Update exportieren"
            Case 159941
                Return "Alle exportieren"
            Case 159942
                Return "Ausgabeordner"
            Case 159943
                Return "Qualität"
            Case 159944
                Return "Übergeordnet"
            Case 159946
                Return "CALCMENU Web 2008"
            Case 159947
                Return "Dateien auswählen oder hochladen"
            Case 159949
                Return "Format sollte nicht länger als 10 Zeichen sein."
            Case 159950
                Return "Nährstoffname sollte nicht länger als 25 Zeichen sein."
            Case 159951
                Return "Rollen"
            Case 159962
                Return "Steuerinformation eingeben"
            Case 159963
                Return "Übersetzung eingeben"
            Case 159966
                Return "Markierte Einträge zu neuer Marke verschieben"
            Case 159967
                Return "Name der Standard-Site eingeben:"
            Case 159968
                Return "Standardthema der Website eingeben:"
            Case 159969
                Return "Ermöglichen der Gruppierung der Standorte nach Region durch den Regionsverwalter:"
            Case 159970
                Return "Fordert Benutzerinformation für den Prüfer an, bevor es genutzt oder veröffentlicht wird:"
            Case 159971
                Return "Übersetzung für jede der entsprechenden Sprachen eingeben oder der Standardtext wird genutzt:"
            Case 159973
                Return "Wählen Sie die Standorte, die zu dieser Region gehören."
            Case 159974
                Return "Wählen Sie die verfügbaren Sprachen, die für das Übersetzen der Zutat, Rezepte, Menüs und anderen Informationen genutzt werden."
            Case 159975
                Return "Wählen Sie ein oder mehrere Preisgruppen für die Zuordnung zu Ihren Zutat, Rezepten und Menüs"
            Case 159976
                Return "Wählen Sie Einträge zum Einfügen"
            Case 159977
                Return "Liste der Besitzer"
            Case 159978
                Return "Wählen Sie unten ein Format"
            Case 159979
                Return "Wählen Sie die Liste zum Löschen"
            Case 159981
                Return "Das Folgende sind die freigegebenen Standorte für diesen Eintrag"
            Case 159982
                Return "Markierte Einträge zu neuer Quelle verschieben"
            Case 159987
                Return "Typ anfordern"
            Case 159988
                Return "Angefordert von"
            Case 159990
                Return "Marke ändern"
            Case 159994
                Return "Einen Posten im Menü ersetzen"
            Case 159997
                Return "Globale Freigabe"
            Case 160004
                Return "Erste Ebene"
            Case 160005
                Return "Die ausgewählte Zutat hat die folgenden Einheiten:"
            Case 160008
                Return "Schritt"
            Case 160009
                Return "Weitere Aktion"
            Case 160012
                Return "Dieses Rezept/Menü ist im Web veröffentlicht."
            Case 160013
                Return "Dieses Rezept/Menü ist nicht im Web veröffentlicht."
            Case 160014
                Return "Erinnerung"
            Case 160016
                Return "Besitzer anzeigen"
            Case 160018
                Return "Diese Ware ist im Web veröffentlicht."
            Case 160019
                Return "Diese Ware ist nicht im Web veröffentlicht."
            Case 160020
                Return "Diese Ware ist ausgestellt."
            Case 160021
                Return "Diese Ware ist nicht ausgestellt."
            Case 160023
                Return "Zum Druck"
            Case 160028
                Return "Nicht zur Veröffentlichung"
            Case 160030
                Return "Zu Einkaufsliste hinzufügen"
            Case 160033
                Return "Schlüsselworte hinzufügen"
            Case 160035
                Return "Sie haben %n Login-Versuche gehabt"
            Case 160036
                Return "Dieser Zugang wurde deaktiviert"
            Case 160037
                Return "Bitte verständigen Sie Ihren Systemadministrator, um den Zugang erneut zu aktivieren."
            Case 160038
                Return "Mein Profil"
            Case 160039
                Return "Letzte Anmeldung"
            Case 160040
                Return "Sie sind nicht angemeldet."
            Case 160041
                Return "Seitensprache"
            Case 160042
                Return "Hauptübersetzung"
            Case 160043
                Return "Hauptpreisgruppe"
            Case 160045
                Return "Zeilen pro Seite"
            Case 160046
                Return "Standardanzeige"
            Case 160047
                Return "Zutatenmengen"
            Case 160048
                Return "Zuletzt zugegriffen"
            Case 160049
                Return "'%f' erhalten"
            Case 160050
                Return "Länge"
            Case 160051
                Return "'%f' nicht erhalten"
            Case 160055
                Return "Menge muss größer als 0 sein."
            Case 160056
                Return "Erzeugen eines neuen Unterrezepts"
            Case 160057
                Return "Sitzung ist abgelaufen."
            Case 160058
                Return "Anmeldung ist abgelaufen, da Sie länger als %n Minuten inaktiv zutat."
            Case 160065
                Return "Kein Name"
            Case 160066
                Return "Sind Sie sicher, dass Sie schließen wollen?"
            Case 160067
                Return "Der Eintrag erfordert eine Prüfung"
            Case 160068
                Return "Klicken Sie auf die Schaltfläche '%s', um eine Prüfung anzufordern."
            Case 160070
                Return "Die markierten Einträge werden bearbeitet."
            Case 160071
                Return "Dieser Eintrag wurde zur Prüfung angemeldet."
            Case 160072
                Return "Für diesen Eintrag gibt es bereits eine Anforderung."
            Case 160074
                Return "Einheit auswählen"
            Case 160082
                Return "Sie haben neue Anforderungen zur Prüfung."
            Case 160085
                Return "Ihre Anforderung wurde geprüft."
            Case 160086
                Return "Nährstoffliste drucken"
            Case 160087
                Return "Liste drucken"
            Case 160088
                Return "Details drucken"
            Case 160089
                Return "Aktivieren"
            Case 160090
                Return "Erstellen"
            Case 160091
                Return "Entfernt den markierten Eintrag aus der Liste."
            Case 160093
                Return "Für globale Freigabe an System senden"
            Case 160094
                Return "Inhalte für Kiosk-Browser verfügbar machen"
            Case 160095
                Return "Erzeugen einer Systemkopie"
            Case 160096
                Return "In Rezepten und Menüs benutzte Zutat ersetzen"
            Case 160098
                Return "Nicht im Web veröffentlichen"
            Case 160100
                Return "Erzeugen einer Liste der einzukaufenden Zutaten"
            Case 160101
                Return "Um bei den Zutaten Text ohne Mengen- und Preisdefinitionen einzugeben."
            Case 160102
                Return "Sie können Ihre eigene Rezeptdatenbank erzeugen, sie gemeinsam mit anderen Benutzern einsetzen, drucken und Einkaufslisten erzeugen."
            Case 160103
                Return "Ein Menü besteht aus mehreren Gerichten oder Rezepten, die Sie hier zu einer Mahlzeit zusammenstellen können."
            Case 160105
                Return "Verwalten Sie Grundinformationen wie Benutzer, Lieferanten etc."
            Case 160106
                Return "Willkommen"
            Case 160107
                Return "Willkommen bei %s"
            Case 160108
                Return "Bearbeiten Sie die Ansicht und andere Einstellungen."
            Case 160109
                Return "Website-Profil"
            Case 160110
                Return "Anpassen von Name, Thema etc. Website"
            Case 160111
                Return "Zustimmung Austeilung"
            Case 160112
                Return "Prüfung von Zutat, Rezepten und anderen Information."
            Case 160113
                Return "Einstellungen für SMTP und Alarmmeldungen"
            Case 160114
                Return "Konfigurieren der Verbindung mit Ihrem Mailserver; Meldungen aktivieren oder deaktivieren."
            Case 160115
                Return "Maximale Anzahl an Anmeldeversuchen einstellen und blockierte IP-Adressen überwachen."
            Case 160116
                Return "Druck Profil"
            Case 160117
                Return "Definieren Sie mehrere Druckformate für Profile."
            Case 160118
                Return "Definieren Sie die verfügbaren Sprachen, die für das Übersetzen der Zutat, Rezepte, Menüs und anderen Informationen genutzt werden."
            Case 160119
                Return "Für die Währungsumwandlung verfügbare Währungen und Preisdefinitionen."
            Case 160120
                Return "Arbeiten Sie mit Zutat, Rezepten und Menüs mit mehreren Preisgruppen."
            Case 160121
                Return "Regionen sind Gruppen von Restaurants."
            Case 160122
                Return "Standort verwalten Benutzer, die gemeinsam an einem Rezept arbeiten."
            Case 160123
                Return "Benutzer verwalten, die mit %s arbeiten"
            Case 160124
                Return "Einstellungen für Bildverarbeitung"
            Case 160125
                Return "Standardbildgröße für Zutat, Rezepte und Menüs definieren."
            Case 160130
                Return "Handelsmarke oder Eigenname, der eine Ware identifiziert."
            Case 160132
                Return "Benutzt, um Zutat, Rezepte oder Menüs nach allgemeinen Eigenschaften zu gruppieren"
            Case 160135
                Return "Schlüsselworte bieten beschreibende Details für Zutat, Rezepte oder Menüs. Benutzer können Zutat, Rezepten oder Menüs mehrere Schlüsselworte zuordnen."
            Case 160139
                Return "Definieren Sie bis zu 34 Nährstoffwerte für Energie, Kohlenhydrate, Protein und Fett."
            Case 160141
                Return "Erzeugen Sie Regeln, die als zusätzlicher Filter für die Suche genutzt werden."
            Case 160151
                Return "%s besteht aus einer Liste an vordefinierten (oder System-) Einheiten, die bei der Definition von Zutatpreisen und für Rezepte und Menüs genutzt werden."
            Case 160152
                Return "Benutzer können in diese Liste einfügen."
            Case 160153
                Return "Bei der Preisberechnung eingesetzt."
            Case 160154
                Return "Der Begriff ""Quelle"" bezieht sich auf die Herkunft eines bestimmten Rezepts. Dies kann ein Küchenchef, ein Buch oder Magazin, eine Firma, Organisation oder Website sein."
            Case 160155
                Return "Importieren Sie Zutat, Rezepte oder Menüs aus CALCMENU Pro, CALCMENU Enterprise, und anderen EGS-Produkten."
            Case 160156
                Return "Wartung von Wechselkursen bei bestimmten Währungen"
            Case 160157
                Return "Ungenutzte Texte löschen."
            Case 160158
                Return "Alle Texte formatieren."
            Case 160159
                Return "Drucker der Zutatliste in HTML, Excel, PDF und RTF."
            Case 160160
                Return "Drucken von Zutatdetails in HTML, Excel, PDF und RTF."
            Case 160161
                Return "Drucken von Rezeptdetails in HTML, Excel, PDF und RTF."
            Case 160162
                Return "Drucken der Rezeptliste in HTML, Excel, PDF und RTF."
            Case 160163
                Return "Drucken von Menüdetails in HTML, Excel, PDF und RTF."
            Case 160164
                Return "Menu-Engineering erlaubt das Bewerten des aktuellen und der zukünftigen Preisberechnung und Gestaltung von Rezepten. Benutzen Sie Menu-Engineering zur Identifikation von Menüs, die von der Speiskarte entfernt werden müssen."
            Case 160169
                Return "Liste der Menükarten laden"
            Case 160170
                Return "Gespeicherte Menükarten verändern oder anzeigen."
            Case 160175
                Return "Gespeicherte Einkaufslisten verändern oder anzeigen."
            Case 160177
                Return "Sicherheit"
            Case 160180
                Return "Format der Einträge standardisieren"
            Case 160181
                Return "Einträge löschen"
            Case 160182
                Return "Rollenrechte"
            Case 160184
                Return "TCPOS-Export"
            Case 160185
                Return "Exportieren der Verkaufseinträge"
            Case 160187
                Return "Erzeugen von neuen lokalen Zutat, die als Zutat für Ihre Rezepte genutzt werden."
            Case 160188
                Return "Liste der gespeicherten Markierungen anzeigen."
            Case 160189
                Return "Erzeugen einer Liste der einzukaufenden Einträgen"
            Case 160190
                Return "Erzeugen Sie Ihre eigenen Menüs, die auf den Rezepten in Ihrer Datenbank basieren."
            Case 160191
                Return "Erzeugen Texten für Rezepte und Menüs."
            Case 160200
                Return "Sortiert nach Menüname"
            Case 160202
                Return "In der Liste auszuwählen"
            Case 160209
                Return "Bitte geben Sie Seriennummer, Titelnamen und Produktkennung ein. Diese Information finden Sie in der zu %s mitgelieferten Dokumentation."
            Case 160210
                Return "Erwünschte Einträge"
            Case 160211
                Return "Unerwünschte Einträge"
            Case 160212
                Return "Entwürfe"
            Case 160217
                Return "Archivpfad"
            Case 160218
                Return "Fehler beim Import von CSV-Daten"
            Case 160219
                Return "Liste mit Zutat, die bearbeitet werden müssen"
            Case 160220
                Return "Optionen für Zutatimport definieren"
            Case 160232
                Return "Export zu"
            Case 160237
                Return "Fett"
            Case 160254
                Return "Bitte starten Sie den Windows-Dienst %n neu, damit Ihre Änderungen wirksam werden."
            Case 160258
                Return "Die Währung entspricht nicht dem gewählten Preissatz."
            Case 160259
                Return "Name oder Nummer existiert bereits"
            Case 160260
                Return "Datum des Imports"
            Case 160262
                Return "Nährwerte sind pro Ertragseinheit auf 100%"
            Case 160292
                Return "Allergene"
            Case 160293
                Return "Liste der Nahrungsmittelallergien oder -empfindlichkeit"
            Case 160295
                Return "Konto wird im Moment bereits benutzt. Bitte später nochmals probieren"
            Case 160353
                Return "Einkaufspreissatz"
            Case 160354
                Return "Verkaufspreissatz"
            Case 160414
                Return "Inventur Menge" & vbCrLf & "IVorgehend"
            Case 160423
                Return "Einzelplatz-Rezeptverwaltungs-Software"
            Case 160433
                Return "Verbrauch innerhalb"
            Case 160500
                Return "Text Verwaltung"
            Case 160687
                Return "Wechselnde Farben der Zutaten"
            Case 160688
                Return "Normale Farbe der Zutaten"
            Case 160690
                Return "Bitte beachten Sie, dass wenn Sie umspeichern alle Benutzer automatisch ausgeschaltet werden."
            Case 160691
                Return "Backup/Restore Pictures"
            Case 160716
                Return "Setze Artikel standard auf Global"
            Case 160774
                Return "Deaktivieren"
            Case 160775
                Return "Folgende Nullen entfernen."
            Case 160776
                Return "Zurück nach %s"
            Case 160777
                Return "Klicken Sie hier, um mehr über CALCMENU Online zu erfahren."
            Case 160788
                Return "Ausgewählte  Ware(n) ist/sind aktiviert worden."
            Case 160789
                Return "Ausgewählte Ware(n) ist/sind deaktiviert worden."
            Case 160790
                Return "Sind Sie sicher, die ausgewählte Ware(n) entfernen zu wollen?"
            Case 160791
                Return "Ausgewählte Ware(n) ist/sind erfolgreich entfernt worden."
            Case 160801
                Return "Sie können nur 2, oder mehr ähnliche Rezepte zusammenführen."
            Case 160802
                Return "Sind Sie sicher die ausgewählten Zutat zusammenführen zu wollen?"
            Case 160803
                Return "Sind Sie sicher die Ware(n) löschen zu wollen?"
            Case 160804
                Return "Bitte füllen Sie die angegebenen Felder aus."
            Case 160805
                Return "Wählen Sie zwei oder mehr Zutat aus um sie zusammenzufügen."
            Case 160806
                Return "Sind Sie sicher die ausgewählte(n) Ware(n) deaktivieren zu wollen?"
            Case 160863
                Return "Zutatpreisliste"
            Case 160880
                Return "Neuberechnen"
            Case 160894
                Return "Silber"
            Case 160940
                Return "Effektivität Datum"
            Case 160941
                Return "Verkaufsartikel verbunden"
            Case 160953
                Return "Ratio zwischen Verkaufpreisliste und Einkaufspreisliste"
            Case 160958
                Return "Arbeit mit Verkaufsartikeln mit mehreren Verkaufspreislisten"
            Case 160985
                Return "Nicht verbundene Verkaufsartikeln"
            Case 160987
                Return "Verkaufsartikel erstellen und mit bestehende Rezepte verbinden"
            Case 160988
                Return "Verkaufsartikel wird im Verkauf benutzt und wird grundsätzlich mit einem Rezept verbunden"
            Case 161028
                Return "Sind Sie sicher, dass Sie die Nährstoffdatenbank ändern wollen? Diese Aktion ändert die Nährstoffdefinitionen, die Sie bereits für Ihre Zutat festgelegt haben."
            Case 161029
                Return "Wählen Sie entweder das Kontrollkästchen für die Mengen oder die Zutaten."
            Case 161049
                Return "Löschen des Schlüsselworts und seiner Unterschlüssel erzwingen"
            Case 161050
                Return "Gelöschte Schlüsselworte werden ausserdem aus den Zutat/Rezepten/Menüs entfernt."
            Case 161051
                Return "Ausgewählte Schlüsselworte und alle Unterschlüsselworte wurden erfolgreich gelöscht. Gelöschte Schlüsselworte sind jetzt aus den Zutat/Rezepten/Menüs entfernt."
            Case 161078
                Return "Genau"
            Case 161079
                Return "Anfang"
            Case 161080
                Return "Enthält"
            Case 161082
                Return "2."
            Case 161083
                Return "3."
            Case 161084
                Return "4."
            Case 161085
                Return "Nur 1. Mal"
            Case 161086
                Return "Täglich"
            Case 161087
                Return "Wochentlich"
            Case 161088
                Return "Monatlich"
            Case 161089
                Return "Wenn die Datei ändert"
            Case 161090
                Return "Wenn der Computer startet"
            Case 161091
                Return "Information % eingeben"
            Case 161092
                Return "Lieferantengruppe"
            Case 161093
                Return "Rechnungsinformation"
            Case 161094
                Return "Anfangsdatum"
            Case 161095
                Return "des Monates"
            Case 161096
                Return "POS Import - Datenfehler"
            Case 161097
                Return "Organisieren und Überprüfen der Informationen über Ihre Lieferanten wie zum Beispiel der Firmenkontakte, Adressen, Zahlungsvereinbarungen usw., um den Bestellprozess zu vereinfachen."
            Case 161098
                Return "Das Wort ""Terminal"" bezieht sich auf die Stationen Ihres POS, die mit CALCMENU Web verknüpft sind. Hinzufügen, Ändern oder Löschen von Terminals in diesem Programm."
            Case 161099
                Return "Konfigurieren Sie die POS-Importparameter. Stellen Sie die Termine, den Ort der Importdateien usw. ein."
            Case 161100
                Return "Produkte und im Lagerbestand bereitgehaltene Artikel werden zu verschiedenen Zeiten an verschiedenen Orten aufbewahrt bzw. in Umlauf gebracht. Um die Übersicht zu behalten, müssen Sie die Orte ausfindig machen, an denen Ihre Produkte zu jedem Zeitpunkt zu finden sind."
            Case 161101
                Return "Klienten sind diejenigen Firmen, die Ihre Produkte oder Fertigprodukte kaufen. Verwalten Sie Ihre Klienten mit diesem Programm."
            Case 161102
                Return "Klientenkontakte sind die Personen, mit denen sie in einem Unternehmen Kontakt haben Erzeugen, Ändern und Löschen von Klientenkontakten."
            Case 161103
                Return "Korrigieren von POS-Daten, die nicht erfolgreich in das System importiert wurden."
            Case 161104
                Return "Dies bezieht sich auf die Art der Ausgabentransaktion der Lieferungen. Dies wurde unter Umständen bereits an Kunden verkauft, zum Beispiel als Geschenk oder Aufmerksamkeit für die Mitarbeiter verkauft."
            Case 161105
                Return "Die Verkaufsaufzeichnungen zeigen eine Liste aller Verkäufe und der jeweiligen Verkaufsartikel"
            Case 161106
                Return "Markierte Einträge"
            Case 161107
                Return "Berechnete Menge"
            Case 161132
                Return "Meine Rezepte anzeigen"
            Case 161147
                Return "Rezept- und Menüverwaltung"
            Case 161162
                Return "TCPOS"
            Case 161180
                Return "Konfiguration zum automatischen Hochladen definieren"
            Case 161181
                Return "Hostname"
            Case 161275
                Return "Richtwert für die Tageszufuhr"
            Case 161276
                Return "GDA"
            Case 161279
                Return "Ohne"
            Case 161281
                Return "Hauptkoch"
            Case 161282
                Return "Region-Admin"
            Case 161283
                Return "System-Admin"
            Case 161284
                Return "Firmen-Chef"
            Case 161285
                Return "Region-Chef"
            Case 161286
                Return "Koch"
            Case 161287
                Return "Gast"
            Case 161288
                Return "Standort-Chef"
            Case 161289
                Return "Standort-Admin"
            Case 161290
                Return "Sehen und Drucken"
            Case 161291
                Return "Nicht definiert"
            Case 161292
                Return "Definiert"
            Case 161294
                Return "Unerwünschte Einträge"
            Case 161300
                Return "Einkaufspreissatz"
            Case 161333
                Return "Beschriftungen"
            Case 161334
                Return "Rezepte %x-%y von %z"
            Case 161468
                Return "Alle prüfen"
            Case 161484
                Return "Temperatur"
            Case 161485
                Return "Produktion" & vbCrLf & "Datum"
            Case 161486
                Return "Verbrauch" & vbCrLf & "Datum"
            Case 161487
                Return "Tagesprodukt"
            Case 161488
                Return "zu verbrauchen bis"
            Case 161489
                Return "Frisch zubereitet - frisch geniessen"
            Case 161490
                Return "Allergiker-Info; enthält:"
            Case 161491
                Return "Markierte Elemente bearbeiten"
            Case 161494
                Return "bei max. 5° C lagern"
            Case 161538
                Return "Vielen Dank für Ihr Interesse an EGS-Produkten."
            Case 161554
                Return "Sie finden außerdem weitergehende Information über unsere Produkte als Dokumente im PDF-Format unter <a href=""%url"">Produkt-Ressourcen</a>."
            Case 161576
                Return "Einheits- preis"
            Case 161577
                Return "Uhr"
            Case 161578
                Return "Total Zutataufwand"
            Case 161579
                Return "berechnen"
            Case 161580
                Return "Zutataufwand"
            Case 161581
                Return "Taxe"
            Case 161582
                Return "Bruttomarge in Fr."
            Case 161583
                Return "Bruttomarge in %"
            Case 161584
                Return "EH"
            Case 161585
                Return "Preis/" & vbCrLf & "EH"
            Case 161710
                Return "Vorlage"
            Case 161766
                Return "Kleine Portion"
            Case 161767
                Return "Grosse Portion"
            Case 161777
                Return "Schlüsselwort entfernen"
            Case 161778
                Return "Schlüsselworte zuordnen/entfernen"
            Case 161779
                Return "Breadcrumbs"
            Case 161780
                Return "Breadcrumbs überwachen"
            Case 161781
                Return "Unerwünschtes Schlüsselwort"
            Case 161782
                Return "Aufkleber drucken"
            Case 161783
                Return "Anweisungsvorlage"
            Case 161784
                Return "Student"
            Case 161785
                Return "Nährstoffwerte Zutat per %s"
            Case 161786
                Return "Nährstoffwerte Zutat per 100g/ml"
            Case 161787
                Return "Vorlage anwenden"
            Case 161788
                Return "Zugeordnete/abgeleitete Schlüsselworte"
            Case 161823
                Return "Zeile(n) hinzufügen"
            Case 161824
                Return "Aus Zwischenablage einfügen"
            Case 161825
                Return "Keine zu verknüpfende Ware vorhanden."
            Case 161826
                Return "Andere Ware auswählen"
            Case 161827
                Return "Standardpreis pro Einheit:"
            Case 161828
                Return "Einheit auswählen"
            Case 161829
                Return "Als neue Einheit hinzufügen"
            Case 161830
                Return "Überprüfter Artikel"
            Case 161831
                Return "Ich möchte die Ware vor dem Hinzufügen bearbeiten"
            Case 161832
                Return "%s als Ergänzung anfügen"
            Case 161834
                Return "Überprüfen Sie bitte die Preise"
            Case 161835
                Return "Ausschneiden"
            Case 161837
                Return "Zum Rezept hinzufügen"
            Case 161838
                Return "Bestehende Zutaten austauschen"
            Case 161839
                Return "Keine Zutaten gefunden"
            Case 161840
                Return ""
            Case 161841
                Return "Verknüpfung zu Ware oder Unterrezept"
            Case 161842
                Return "Alle Artikel sind nun mit Ware/Unterrezept verknüpft"
            Case 161843
                Return "Artikel ist nun mit Ware/Unterrezept verknüpft"
            Case 161844
                Return "Lagerzeit"
            Case 161845
                Return "Lagertemperatur"
            Case 161851
                Return "Bestellbar"
            Case 161852
                Return "Das Gericht kann Allergene enthalten"
            Case 161853
                Return "Einfügen"
            Case 161855
                Return "Entwürfe"
            Case 161873
                Return "Ausloggen"
            Case 161899
                Return "Eingereicht von"
            Case 161902
                Return "Kommentar hinzufügen"
            Case 161955
                Return "Der Name Ihres Freundes"
            Case 161956
                Return "Die E-Mail Ihres Freundes"
            Case 161970
                Return "Kein Kommentar zu diesem Rezept. Schreiben Sie den ersten Kommentar zu diesem Rezept."
            Case 161986
                Return "Zubereitungsschritt hinzufügen"
            Case 161987
                Return "Artikel %n von %p"
            Case 161988
                Return "Verknüpfte Produkte"
            Case 161989
                Return "Nicht verknüpfte Produkte"
            Case 162032
                Return ""
            Case 162039
                Return "%p Benutzer haben dieses Rezept in ihren Favoriten"
            Case 162054
                Return "Bewertung"

            Case 162057
                Return "%c darf nicht unausgefüllt bleiben"
            Case 162061
                Return ""
            Case 162062
                Return ""
            Case 162102
                Return ""
            Case 162198
                Return "Das ""yield"" ist geändert worden. Klicken Sie die „Calculate"" Taste an, um Zutatenmenge neu zu berechnen."
            Case 162199
                Return "Das ""yield"" ist geändert worden. Möchten Sie fortsetzen ohne die Zutatenmange zu berechnen?"
            Case 162203
                Return "Botschaft"
            Case 162205
                Return "Anzahl Angebote"
            Case 162208
                Return "Wöchentliche Betriebstage"
            Case 162211
                Return "Sprache wählen"
            Case 162212
                Return "Betriebsname"
            Case 162213
                Return "Betriebsnummer"
            Case 162214
                Return "Verfügbare preise"
            Case 162215
                Return "Logo zum Server laden"
            Case 162216
                Return "Einstellungen"
            Case 162219
                Return "Back Office"
            Case 162221
                Return "Allgemeine Konfiguration"
            Case 162222
                Return "Hier einfügen"
            Case 162230
                Return "Formatinformation eingeben"
            Case 162231
                Return "Formatname"
            Case 162232
                Return "Optionen Kopfzeilenformat"
            Case 162235
                Return "Meinen Sie"
            Case 162257
                Return "Letztes Änderungsdatum"
            Case 162276
                Return "Rezept importieren"
            Case 162282
                Return "Notizen"
            Case 162314
                Return "Hersteller"
            Case 162318
                Return "Alkoholgehalt"
            Case 162319
                Return "Jahrgang"
            Case 162338
                Return "Weinsorte"
            Case 162340
                Return "Strasse"
            Case 162341
                Return "Ort"
            Case 162357
                Return "Beispiel"
            Case 162358
                Return "Vorsilbenlänge beibehalten"
            Case 162361
                Return "Registerkarte"
            Case 162362
                Return "senkrechter Strich"
            Case 162363
                Return "Semikolon"
            Case 162364
                Return "Leerzeichen"
            Case 162382
                Return "Zustimmen"
            Case 162383
                Return "Bewerten"
            Case 162386
                Return "Starten"
            Case 162387
                Return "Lieber Rezeptebewerter,    Sie haben ein neues Rezept zur Bewertung erhalten. [...] hat dieses Rezept übermittelt: [...]    Bitte melden Sie sich auf der CALCMENU-Webseite an, um das Rezept zu bewerten.    Beste Grüße von Ihrem EGS-Team"
            Case 162388
                Return "Hallo,    Ihr neu erstelltes Rezept wurde zur Beurteilung abgesendet. Das Rezept wird vor der Online-Nutzung erst noch bewertet und begutachtet. Sie haben folgendes Rezept übermittelt: [...]    Wenn das Rezept erst einmal begutachtet wurde, dann ist es on"
            Case 162389
                Return "Lieber Rezeptebewerter,    Sie haben dieses Rezept begutachtet: [...]    Das Rezept ist nun online verfügbar.    Beste Grüße von Ihrem EGS-Team"
            Case 162390
                Return "Hallo,    Das Rezept [...] ist begutachtet worden. Sie können dieses Rezept nun online verwenden.    Beste Grüße von Ihrem EGS-Team"
            Case 162455
                Return "Login"
            Case 162485
                Return "einem Freund schicken"
            Case 162530
                Return "Bei Login Aufzeichnungen über kürzlich besuchte Seiten entfernen"
            Case 162596
                Return "eine Bewertung abgeben"
            Case 162631
                Return "Passwort vergessen?"
            Case 162632
                Return "Geben Sie ihren Benutzernamen an, um das Passwort zu erhalten."
            Case 162635
                Return "Beantworten Sie folgende Frage, um das Passwort zu erhalten."
            Case 162636
                Return "Frage"
            Case 162637
                Return "Antwort"
            Case 162638
                Return "Ihr Passwort wurde Ihnen zugeschickt."
            Case 162742
                Return "Gut"
            Case 162747
                Return "letzte Änderung:"
            Case 162888
                Return "Bitte wählen Sie eine Datei zum Upload aus."
            Case 162955
                Return "Netto Marge in %"
            Case 163032
                Return "Preisliste kopieren"
            Case 163046
                Return "Leider wurde das Schlüsselwort %k%n%u nicht gefunden. Bitte wählen Sie „Schlüsselwort durchsuchen"", um verfügbare Schlüsselwörter auszuwählen."
            Case 163057
                Return "Kosten für Total %s"
            Case 163058
                Return "Kosten für 1 %s"
            Case 163060
                Return "Kosten in %s"
            Case 163061
                Return "Effektive Kosten in %s"
            Case 167272
                Return "Produktdetails"
            Case 167346
                Return "Alle anzeigen"
            Case 167385
                Return "Untertitel"
            Case 167469
                Return "Anmerkung"
            Case 167719
                Return "Budget"
            Case 168373
                Return "Online benutzt"
            Case 168374
                Return ""
            Case 168375
                Return ""
            Case 169310
                Return "Degustation/Entwicklung"
            Case 169318
                Return "Feedback"
            Case 170155
                Return "Kategorien, Schlüsselworte und Quellen (ein Buch, eine Website oder ein Küchenchef) sind bei der Suche nach Zutat, Rezepten und Menüs sehr nützlich. Der globale Administrator kann diese Listen verwalten und zu jeder Niederlassung zuordnen, Machen Sie Kategorien, Schlüsselworte und Herkunft (könnte ein Buch, eine Website oder ein Chef sein) zu Ihrer Ware, und Rezepten um die Suche für alle Zutaten zu vereinfachen"
            Case 170253
                Return "PDF anzeigen"
            Case 170283
                Return "Bitte nehmen Sie für weitere Details Kontakt mit uns auf."
            Case 170668
                Return "Freundliche Grüsse,"
            Case 170674
                Return "Zugang ohne Login"
            Case 170675
                Return "Kurse"
            Case 170770
                Return "Ertrag drucken"
            Case 170779
                Return "Zutatenliste"
            Case 170780
                Return "Zutateinzelheiten"
            Case 170781
                Return "Nährwertliste der Zutaten"
            Case 170782
                Return "Zutatenkategorien"
            Case 170783
                Return "Zutatenstichwort"
            Case 170784
                Return "Im Web veröffentlichte Zutaten"
            Case 170785
                Return "Nicht im Web veröffentlichte Zutaten"
            Case 170786
                Return "Kosten der Zutaten"
            Case 170801
                Return ""
            Case 170849
                Return "Verkürzte Herstellungsmethode"
            Case 170850
                Return "Nur Kochen"
            Case 170851
                Return "Nur nicht zu kochendes"
            Case 170852
                Return "Prahlen"
            Case 170853
                Return "Schnell & Einfach"
            Case 170854
                Return "Empfehlung vom Küchenchef"
            Case 170855
                Return "Mittelschwer"
            Case 170856
                Return "Anspruchsvoll"
            Case 170857
                Return "Gold"
            Case 170858
                Return "nicht kategorisiert"
            Case 170859
                Return "Bronze"
            Case 170860
                Return "Als neuen Standard festlegen"
            Case 171014
                Return "entspricht"
            Case 171219
                Return "Führung"
            Case 171220
                Return "Anzahl der Portionen"
            Case 171221
                Return "Gesamtertrag"
            Case 171231
                Return "Download von Barcode-Schriftarten"
            Case 171232
                Return "Medien"
            Case 171233
                Return "Drucktyp"
            Case 171234
                Return "Geschützt"
            Case 171235
                Return "Automatisch berechnen"
            Case 171236
                Return "öffentlich"
            Case 171237
                Return "Originalgröße anzeigen"
            Case 171238
                Return "Nicht online verwendet"
            Case 171240
                Return "Ungespeicherte Artikel"
            Case 171241
                Return "Anzeigen, wie viel Prozent bereits übersetzt wurden"
            Case 171242
                Return "Mit geschützten Kopien arbeiten"
            Case 171243
                Return "Beim Drucken und Exportieren mit einbeziehen"
            Case 171244
                Return "Fußzeile: Logo und Adresse für den Bericht"
            Case 171245
                Return "Fußzeile: Adresse"
            Case 171246
                Return "Erzwungenes Löschen von Kategorien"
            Case 171249
                Return "%s existiert schon"
            Case 171301
                Return "Vorbereitungsmethode"
            Case 171302
                Return "Tipps"
            Case 171345
                Return "Alle Kurse"
            Case 171346
                Return "Das ganze Jahr"
            Case 171347
                Return "Angebotene Kurse"
            Case 171348
                Return "Kurs"
            Case 171352
                Return "Der Benutzername oder die Emailadresse ist ungültig"
            Case 171353
                Return "Bitte geben Sie Ihren Benutzernamen oder Ihre Emailadresse ein"
            Case 171354
                Return "Geben Sie Ihren Benutzernamen oder Ihre Emailadresse ein"
            Case 171371
                Return "Mehr Details anzeigen"
            Case 171372
                Return "Weniger anzeigen"
            Case 171373
                Return "Bitte speichern Sie zuerst das Rezept ."
            Case 171399
                Return "Kiosk für %CM"
            Case 171401
                Return "Die Rezepte in diesem Kiosk wurden von %CM erstellt."
            Case 171402
                Return "Dieses Rezept mit %p teilen"
            Case 171425
                Return "Powered by"
            Case 171428
                Return "Ungültige Parameter. Kontaktieren Sie den Absender des Rezeptes oder das CALCMENU Cloud Support-Team."
            Case 171429
                Return "Der Link zu diesem Rezept/dieser Rezeptgruppe ist abgelaufen. Kontaktieren Sie den Absender des Rezeptes oder das CALCMENU Cloud Support-Team."
            Case 171447
                Return "Ihre Emailadresse/SMTP wurde noch nicht konfiguriert. Legen Sie Ihre Emailadresse im Konfigurationsmenü an um dieses Feature nutzen zu können."
            Case 171453
                Return "Die Email konnte nicht verschickt werden"
            Case 171501
                Return "Sollten Sie nicht wissen was Sie eingeben müssen, so mailen Sie uns bitte Ihre CALCMENU Seriennummer und den Namen des Headers."
            Case 171502
                Return "Bitte nutzen Sie für die EGS Website die Login-Angaben, die zu Ihren Produktschlüsseln und der Seriennummer gehören."
            Case 171505
                Return "Dieses Rezept ist für CALCMENU kodiert. Klicken Sie den Link um mehr zu erfahren."
            Case 171506
                Return "Nutzen Sie die Login-Angaben der EGS Website um sich einzuloggen."
            Case 171507
                Return "Sie haben Ihren Benutzernamen und/oder Ihr Passwort vergessen?"
            Case 171555
                Return ""
            Case 171557
                Return ""
            Case 171558
                Return ""
            Case 171559
                Return "RECIPICENTER ist eine umfangreiche Sammlung von Rezepten aus der ganzen Welt - von Amateuren bis zu Profi-Köchen und Website-Mitgliedern,  die Rezepte online bewerten können."
            Case 171560
                Return ""
            Case 171561
                Return ""
            Case 171586
                Return ""
            Case 171588
                Return "Forum Culinaire Kurse"
            Case 171589
                Return "%c Ingredienzen Verwaltung Lieferanten"
            Case 171591
                Return ""
            Case 171592
                Return ""
            Case 171593
                Return ""
            Case 171594
                Return ""
            Case 171595
                Return "Rezepte auf dieser Website werden von %Cmcloud verwaltet, die fortschrittliche Rezepturverwaltung & Editoren-Tool für Lebensmittel-Profis und Rezepte-Editoren."

            Case 171596
                Return ""
            Case 171597
                Return ""
            Case 171598
                Return "Ja, ich möchte Informationen über CALCMENU Cloud erhalten, sobald diese zur Verfügung stehen."
            Case 171599
                Return "In permanenter Entwicklung anspruchsvoller Rezepte-Software stellen wir Ihnen den neuen und verbesserten Recipecenter vor. Die Webseite bietet eine einfache und bequeme Schnittstelle zur Überprüfung von Rezepten online und um sie mit Ihren Freunden über Facebook und Twitter zu teilen, entweder mit Ihrem Handy, iPhone oder iPad, Blackberry und anderen Geräten."
            Case 171600
                Return "Ansehen, ""favoritieren"", bewerten und kommentieren der Recepte. Wenn Sie bereits in recipecenter.com registriert sind, können Sie sich mit den gleichen Kontodaten ""einloggen"", und Ihre verschlüsselten Rezepte und andere Informationen werden nicht verloren sein. Benutzer werden auch bald eine erweiterte Rezeptur-Management-Lösung haben, zum  Kodieren, Rezepte teilen, und Zugriff auf eine große Rezept-Sammlung von neuen Beiträgern zu haben - und das dank der Integration mit dem Rezept-Management-Software - CALCMENU Cloud."

            Case 171601
                Return "Laden Sie doch Ihre Freunde ein, unserer Gemeinschaft beizutreten. Wir hoffen, dass Sie Ihren Besuch auf unserer Webseite genießen und bald wieder kommen."
            Case 171602
                Return "Rezepte auf dieser Website sind verschlüsselt und verwaltet mit Rezept-Management-Software: CALCMENU Cloud."
            Case 171605
                Return ""
            Case 171611
                Return "Favoritieren"
            Case 171612
                Return "Nich gut"
            Case 171614
                Return "Rezept an einen Freund versenden:"
            Case 171615
                Return "Verbinden Sie mit uns"
            Case 171616
                Return ""
            Case 171617
                Return ""
            Case 171618
                Return ""
            Case 171619
                Return ""
            Case 171620
                Return ""
            Case 171621
                Return "Sie erhalten das Kennwort, wenn Sie unten Ihre E-Mail Adresse für Ihr Konto eingeben."
            Case 171622
                Return "Artikel Nummer %c"
            Case 171628
                Return "Rezepte von unseren Teilnehmer"
            Case 171631
                Return ""
            Case 171649
                Return ""
            Case 171650
                Return ""
            Case 171651
                Return ""
            Case 171652
                Return ""
            Case 171653
                Return ""
            Case 171654
                Return ""
            Case 171655
                Return ""
            Case 171656
                Return ""
            Case 171657
                Return ""
            Case 171658
                Return ""
            Case 171662
                Return ""
            Case 171663
                Return ""
            Case 171664
                Return ""
            Case 171665
                Return ""
            Case 171666
                Return ""
            Case 171667
                Return ""
            Case 171668
                Return ""
            Case 171669
                Return ""
            Case 171670
                Return ""
            Case 171671
                Return ""
            Case 171672
                Return ""
            Case 171673
                Return ""
            Case 171674
                Return ""
            Case 171675
                Return ""
            Case 171676
                Return ""
            Case 171677
                Return ""
            Case 171678
                Return ""
            Case 171679
                Return ""
            Case 171680
                Return ""
            Case 171681
                Return ""
            Case 171682
                Return ""
            Case 171683
                Return ""
            Case 171684
                Return ""
            Case 171685
                Return ""
            Case 171686
                Return ""
            Case 171687
                Return ""
            Case 171688
                Return ""
            Case 171689
                Return ""
            Case 171690
                Return ""
            Case 171691
                Return ""
            Case 171692
                Return ""
            Case 171693
                Return ""
            Case 171694
                Return ""
            Case 171696
                Return ""
            Case 171697
                Return ""
            Case 171698
                Return ""
            Case 171699
                Return ""
            Case 171700
                Return ""
            Case 171701
                Return ""
            Case 171702
                Return ""
            Case 171703
                Return ""
            Case 171704
                Return ""
            Case 171705
                Return ""
            Case 171706
                Return ""
            Case 171707
                Return ""
            Case 171708
                Return ""
            Case 171709
                Return ""
            Case 171710
                Return ""
            Case 171711
                Return ""
            Case 171712
                Return ""
            Case 171713
                Return ""
            Case 171714
                Return ""
            Case 171715
                Return ""
            Case 171716
                Return ""
            Case 171717
                Return ""
            Case 171718
                Return ""
            Case 171719
                Return ""
            Case 171720
                Return ""
            Case 171721
                Return ""
            Case 171722
                Return ""
            Case 171723
                Return ""
            Case 171724
                Return ""
            Case 171725
                Return ""
            Case 171726
                Return ""
            Case 171727
                Return ""
            Case 171728
                Return ""
            Case 171729
                Return ""
            Case 171730
                Return ""
            Case 171731
                Return ""
            Case 171732
                Return ""
            Case 171733
                Return ""
            Case 171734
                Return ""
            Case 171735
                Return ""
            Case 171736
                Return ""
            Case 171737
                Return ""
            Case 171738
                Return ""
            Case 171739
                Return ""
            Case 171740
                Return ""
            Case 171741
                Return ""
            Case 171742
                Return ""
            Case 171743
                Return ""
            Case 171744
                Return ""
            Case 171745
                Return ""
            Case 171746
                Return ""
            Case 171747
                Return ""
            Case 171748
                Return ""
            Case 171749
                Return ""
            Case 171750
                Return ""
            Case 171751
                Return ""
            Case 171752
                Return ""
            Case 171753
                Return ""
            Case 171754
                Return ""
            Case 171755
                Return ""
            Case 171756
                Return ""
            Case 171758
                Return ""
            Case 171759
                Return ""
            Case 171760
                Return ""
            Case 171761
                Return ""
            Case 171762
                Return ""
            Case 171763
                Return ""
            Case 171764
                Return ""
            Case 171765
                Return ""
            Case 171767
                Return ""
            Case 171768
                Return ""
            Case 171769
                Return ""
            Case 171770
                Return ""
            Case 171771
                Return ""
            Case 171772
                Return ""
            Case 171773
                Return ""
            Case 171774
                Return ""
            Case 171775
                Return ""
            Case 171776
                Return ""
            Case 171777
                Return ""
            Case 171778
                Return ""
            Case 171779
                Return ""
            Case 171780
                Return ""
            Case 171781
                Return ""
            Case 171782
                Return ""
            Case 171783
                Return ""
            Case 171785
                Return ""
            Case 171786
                Return ""
        End Select
    End Function

 
'french

    Public Function FTBLow3USA(ByVal Code As Integer) As String
        Select Case Code
            Case 1080
                Return "Coût march."
            Case 1081
                Return "Coût des ingrédients"
            Case 1090
                Return "Prix vente"
            Case 1145
                Return "Compteur"
            Case 1146
                Return "En cours"
            Case 1260
                Return "Ingrédient(s)"
            Case 1280
                Return "Remarque"
            Case 1290
                Return "Prix"
            Case 1300
                Return "Déchet"
            Case 1310
                Return "Quantité"
            Case 1400
                Return "Menu"
            Case 1450
                Return "Catégorie"
            Case 1480
                Return "Prix imposé"
            Case 1485
                Return "Prix calculé"
            Case 1500
                Return "Date"
            Case 1530
                Return "Il manque l'unité"
            Case 1600
                Return "Modification d'un menu"
            Case 2430
                Return "&A choisir dans la liste"
            Case 2700
                Return "Impression liste des menus"
            Case 2780
                Return "Bon d'économat"
            Case 3057
                Return "Base de données"
            Case 3140
                Return "Pour"
            Case 3150
                Return "Pourcentage"
            Case 3161
                Return "Coef."
            Case 3195
                Return "Recette no."
            Case 3200
                Return "Chef"
            Case 3204
                Return "Prénom"
            Case 3205
                Return "Nom"
            Case 3206
                Return "Traduction"
            Case 3215
                Return "Prix unitaire"
            Case 3230
                Return "Image"
            Case 3234
                Return "Liste"
            Case 3300
                Return "Carte du menu"
            Case 3305
                Return "Nom référence"
            Case 3306
                Return "Représentant"
            Case 3320
                Return "Faut-il adapter les quantités au nouveau nombre de personnes ?"
            Case 3460
                Return "&Mot de passe"
            Case 3680
                Return "Sauvegarde"
            Case 3685
                Return "Sauvegarde terminée"
            Case 3721
                Return "Source"
            Case 3760
                Return "Importation"
            Case 3800
                Return "Exportation"
            Case 4130
                Return "Espace disque libre"
            Case 4185
                Return "Numéro de produit"
            Case 4755
                Return "Lancer l'importation"
            Case 4825
                Return "Recettes"
            Case 4832
                Return "Recette"
            Case 4834
                Return "Marchandise de la recette"
            Case 4854
                Return "Minimum"
            Case 4855
                Return "Maximum"
            Case 4856
                Return "Depuis le"
            Case 4860
                Return "Nom du fichier"
            Case 4862
                Return "Version"
            Case 4865
                Return "Utilisateurs"
            Case 4867
                Return "Modifier"
            Case 4870
                Return "Modifier un utilisateur"
            Case 4877
                Return "Moyenne"
            Case 4890
                Return "Type de fichier"
            Case 4891
                Return "Aperçu"
            Case 5100
                Return "Unité"
            Case 5105
                Return "Format"
            Case 5270
                Return "Liste des ingrédients"
            Case 5350
                Return "Total"
            Case 5390
                Return "person"
            Case 5500
                Return "Numéro"
            Case 5530
                Return "Prix de vente imposé"
            Case 5590
                Return "Ingrédients"
            Case 5600
                Return "Préparation"
            Case 5610
                Return "Page"
            Case 5720
                Return "Montant"
            Case 5741
                Return "brute"
            Case 5795
                Return "par personne"
            Case 5801
                Return "Rend."
            Case 5900
                Return "Catégorie de ingrédients"
            Case 6000
                Return "Modification de catégorie"
            Case 6002
                Return "Nom de la catégorie"
            Case 6055
                Return "Ajout d'un texte"
            Case 6390
                Return "Monnaie"
            Case 6416
                Return "Facteur"
            Case 6470
                Return "Patientez un instant svp"
            Case 7010
                Return "Non"
            Case 7030
                Return "Imprimante"
            Case 7073
                Return "Parcourir"
            Case 7181
                Return "Tout"
            Case 7183
                Return "Marqués"
            Case 7250
                Return "Français"
            Case 7260
                Return "Allemand"
            Case 7270
                Return "Anglais"
            Case 7280
                Return "Italien"
            Case 7292
                Return "Japonais"
            Case 7296
                Return "Europe"
            Case 7335
                Return "Toutes les marques ont été supprimées avec succès"
            Case 7570
                Return "Dimanche"
            Case 7571
                Return "Lundi"
            Case 7572
                Return "Mardi"
            Case 7573
                Return "Mercredi"
            Case 7574
                Return "Jeudi"
            Case 7575
                Return "Vendredi"
            Case 7576
                Return "Samedi"
            Case 7720
                Return "Emballage"
            Case 7725
                Return "Transport"
            Case 7755
                Return "Système"
            Case 8210
                Return "Calcul"
            Case 8220
                Return "Fiche technique"
            Case 8395
                Return "Ajouter"
            Case 8397
                Return "Supprimer"
            Case 8514
                Return "Nouveau prix"
            Case 8913
                Return "Aucune"
            Case 8914
                Return "Décimal"
            Case 8990
                Return "ou"
            Case 8994
                Return "Outils"
            Case 9030
                Return "Mise à jour"
            Case 9070
                Return "Pas autorisé dans la version de démonstration"
            Case 9140
                Return "Suisse"
            Case 9920
                Return "Description"
            Case 10103
                Return "Copie"
            Case 10104
                Return "Texte"
            Case 10109
                Return "Options"
            Case 10116
                Return "Note"
            Case 10121
                Return "Chercher"
            Case 10125
                Return "Note"
            Case 10129
                Return "Sélection"
            Case 10130
                Return "En stock"
            Case 10131
                Return "Entrée"
            Case 10132
                Return "Sortie"
            Case 10135
                Return "Style"
            Case 10140
                Return "Stock"
            Case 10363
                Return "Taxe"
            Case 10369
                Return "Numéro de fournisseur"
            Case 10370
                Return "En commande"
            Case 10399
                Return "Effacée"
            Case 10417
                Return "Erreur:"
            Case 10430
                Return "Lieu"
            Case 10431
                Return "Inventaire"
            Case 10447
                Return "Commande"
            Case 10468
                Return "État"
            Case 10513
                Return "Rabais"
            Case 10523
                Return "Tél."
            Case 10524
                Return "Fax"
            Case 10554
                Return "Description CCP"
            Case 10555
                Return "Temps de refroidissement"
            Case 10556
                Return "Temps de cuisson"
            Case 10557
                Return "Température de cuisson"
            Case 10558
                Return "Moyen de cuisson"
            Case 10572
                Return "Nutriment"
            Case 10573
                Return "Infos1"
            Case 10970
                Return "Imprimer"
            Case 10990
                Return "Fournisseur"
            Case 11040
                Return "Restitution achevée"
            Case 11060
                Return "Répertoire"
            Case 11280
                Return "Enregistrement"
            Case 12515
                Return "Code-barre"
            Case 12525
                Return "Date invalide"
            Case 13060
                Return "Nutriments"
            Case 13065
                Return "Affichage nutriments"
            Case 13255
                Return "Historique"
            Case 14070
                Return "Ecriture"
            Case 14090
                Return "Titre"
            Case 14110
                Return "Bas de page"
            Case 14816
                Return "Remplace avec"
            Case 14819
                Return "Remplace"
            Case 14884
                Return "Ingrédients mises à jour"
            Case 15360
                Return "Menus marqués"
            Case 15504
                Return "Administrateur"
            Case 15510
                Return "Mot de passe"
            Case 15615
                Return "Entrez votre mot de passe"
            Case 15620
                Return "Confirmation"
            Case 16010
                Return "Calcul"
            Case 18460
                Return "Enregistrement en cours"
            Case 19330
                Return "Taille"
            Case 20122
                Return "Entreprise"
            Case 20200
                Return "Sous-recette"
            Case 20469
                Return "Indiquez le genre d'envoi."
            Case 20530
                Return "Energie"
            Case 20703
                Return "Principal"
            Case 20709
                Return "Unités"
            Case 21550
                Return "Aucun mets trouvé"
            Case 21570
                Return "Impression d'un formulaire FAX"
            Case 21600
                Return "de"
            Case 24002
                Return "Dernière commande"
            Case 24011
                Return "de"
            Case 24016
                Return "Fournisseur"
            Case 24027
                Return "Calcul"
            Case 24028
                Return "Annuler"
            Case 24044
                Return "Les deux"
            Case 24050
                Return "Nouvelles"
            Case 24068
                Return "Marge"
            Case 24075
                Return "Numéro d'article"
            Case 24085
                Return "Assigner nouveau"
            Case 24087
                Return "Aucune marchandise trouvée"
            Case 24105
                Return "Afficher"
            Case 24121
                Return "Abrév."
            Case 24129
                Return "Transfert"
            Case 24150
                Return "Editer"
            Case 24152
                Return "Profession"
            Case 24153
                Return "Ville"
            Case 24163
                Return "Lieu de stockage par défaut"
            Case 24260
                Return "Ce fournisseur ne peut pas être effacé"
            Case 24268
                Return "Déselectionner tous"
            Case 24269
                Return "Sélectionner tous"
            Case 24270
                Return "Retour"
            Case 24271
                Return "Suivant"
            Case 24291
                Return "Sous-total"
            Case 26000
                Return "Poursuivre"
            Case 26100
                Return "Description du produit"
            Case 26101
                Return "Conseil"
            Case 26102
                Return "Variante"
            Case 26103
                Return "Stockage"
            Case 26104
                Return "Rendement"
            Case 27000
                Return "Nom de réf."
            Case 27020
                Return "Adresse"
            Case 27050
                Return "Numéro de téléphone"
            Case 27055
                Return "Entête"
            Case 27056
                Return "et"
            Case 27130
                Return "Paiement"
            Case 27135
                Return "Date d'expiration"
            Case 27220
                Return "Heure"
            Case 27530
                Return "Taux"
            Case 28000
                Return "Erreur d'exécution"
            Case 28008
                Return "Répertoire invalide"
            Case 28420
                Return "Pas d'image disponible"
            Case 28483
                Return "L'enregistrement n'existe pas"
            Case 28655
                Return "Aucune unité n'a été définie"
            Case 29170
                Return "Pas disponible"
            Case 29771
                Return "Modification des ingrédients"
            Case 30210
                Return "L'opération a échoué"
            Case 30240
                Return "Code"
            Case 30270
                Return "introuvable"
            Case 31085
                Return "Mise à jour réalisée avec succès"
            Case 31098
                Return "Enregistrer"
            Case 31370
                Return "Coût ingrédients (%)"
            Case 31375
                Return "CM"
            Case 31380
                Return "Principal"
            Case 31462
                Return "Erreur"
            Case 31492
                Return "Ce service d'assistance par fax vous assure une réponse entre 1h et 24h selon le problème (week-end non garanties)"
            Case 31700
                Return "Jours"
            Case 31732
                Return "Plan de menus"
            Case 31755
                Return "Résultats"
            Case 31758
                Return "Au"
            Case 31769
                Return "vendu"
            Case 31800
                Return "Jour"
            Case 31860
                Return "Période"
            Case 51056
                Return "Produit"
            Case 51086
                Return "Langue"
            Case 51092
                Return "Unité"
            Case 51097
                Return "EGS Enggist && Grandjean Software SA"
            Case 51098
                Return "Pierre-à-Bot 92"
            Case 51099
                Return "2000 Neuchâtel, Suisse"
            Case 51123
                Return "Détails"
            Case 51128
                Return "Nom de la recette"
            Case 51129
                Return "Ingrédients désirés"
            Case 51130
                Return "Ingrédients indésirable"
            Case 51131
                Return "Nom de la catégorie"
            Case 51139
                Return "Désiré"
            Case 51157
                Return "Message"
            Case 51174
                Return "Importation terminée"
            Case 51178
                Return "Veuillez réessayer"
            Case 51198
                Return "Connexion au serveur SMTP en cours"
            Case 51204
                Return "Oui"
            Case 51243
                Return "Marge"
            Case 51244
                Return "Haut"
            Case 51245
                Return "Bas"
            Case 51246
                Return "Gauche"
            Case 51247
                Return "Droite"
            Case 51252
                Return "Téléchargement"
            Case 51257
                Return "E-mail"
            Case 51259
                Return "Serveur SMTP"
            Case 51261
                Return "Nom de l'utilisateur"
            Case 51281
                Return "Ingrédients pour"
            Case 51294
                Return "Pour"
            Case 51311
                Return "Unité invalide"
            Case 51323
                Return "Valeur non-autorisée pour la quantité à servir"
            Case 51336
                Return "Indésiré"
            Case 51337
                Return "Principal"
            Case 51353
                Return "Contrat de Licence"
            Case 51364
                Return "Acceptez-vous la contrat de licence ci-dessus et souhaitez poursuivre avec la soumission de cette recette?"
            Case 51373
                Return "Veuillez SVP introduire toutes les informations concernant SMTP, POP, Nom de l'utilisateur et mot de passe"
            Case 51377
                Return "Envoyer e-mail"
            Case 51392
                Return "Portions"
            Case 51402
                Return "Etes-vous sur de vouloir effacer"
            Case 51500
                Return "Détails de la liste d'achat"
            Case 51502
                Return "Liste d'achat"
            Case 51532
                Return "Imprime la liste des achats"
            Case 51907
                Return "&Afficher le détail"
            Case 52012
                Return "Choisir"
            Case 52110
                Return "Le fichier que vous avez sélectionné sera importé"
            Case 52130
                Return "Nouvelle recette"
            Case 52150
                Return "Fin"
            Case 52307
                Return "Fermer"
            Case 52960
                Return "Simple"
            Case 52970
                Return "Complet"
            Case 53250
                Return "Exporter la séléction"
            Case 54210
                Return "Ne rien changer"
            Case 54220
                Return "Tout en majuscule"
            Case 54230
                Return "Tout en minuscule"
            Case 54240
                Return "Mise en majuscule de la 1ère lettre de chaque mot"
            Case 54245
                Return "Première lettre en majuscule"
            Case 54295
                Return "par"
            Case 54710
                Return "Selected Keywords"
            Case 54730
                Return "Mots clés"
            Case 55011
                Return "Portion"
            Case 55211
                Return "Lien"
            Case 55220
                Return "Qté"
            Case 56100
                Return "Votre nom"
            Case 56130
                Return "Pays"
            Case 56500
                Return "Dictionnaire"
            Case 101600
                Return "Modification d'un menu"
            Case 103150
                Return "Pourcentage"
            Case 103215
                Return "Prix unitaire"
            Case 103305
                Return "Nom réf"
            Case 103306
                Return "Représentant"
            Case 104829
                Return "Liste des fournisseurs"
            Case 104835
                Return "Créer un nouveau produit"
            Case 104836
                Return "Modification d'un produit"
            Case 104854
                Return "Minimum"
            Case 104855
                Return "Maximum"
            Case 104862
                Return "Version"
            Case 104869
                Return "Nouvel utilisateur"
            Case 104870
                Return "Modifier un utilisateur"
            Case 105100
                Return "Unité"
            Case 105110
                Return "Date"
            Case 105200
                Return "pour"
            Case 105360
                Return "Prix de vente par personne"
            Case 106002
                Return "Nom de la catégorie"
            Case 107183
                Return "Marqués"
            Case 109730
                Return "par"
            Case 110101
                Return "Modifie"
            Case 110102
                Return "Efface"
            Case 110112
                Return "Imprime"
            Case 110114
                Return "Aide"
            Case 110129
                Return "Sélection"
            Case 110417
                Return "Erreur:"
            Case 110447
                Return "Commande"
            Case 110524
                Return "Fax"
            Case 113275
                Return "Taxe"
            Case 115510
                Return "Mot de passe"
            Case 115610
                Return "Nouveau mot de passe accepté"
            Case 119130
                Return "Recherche"
            Case 121600
                Return "de"
            Case 124016
                Return "Fournisseur"
            Case 124024
                Return "Approuvée par"
            Case 124042
                Return "Type"
            Case 124164
                Return "Ajustements d'inventaire"
            Case 124257
                Return "Point de vente"
            Case 127010
                Return "Société"
            Case 127040
                Return "Pays"
            Case 127050
                Return "Téléphone"
            Case 127055
                Return "Entête"
            Case 128000
                Return "Erreur d'exécution"
            Case 131462
                Return "Erreur"
            Case 131700
                Return "Jours"
            Case 131757
                Return "Du"
            Case 132541
                Return "Recette"
            Case 132552
                Return "Total tax"
            Case 132553
                Return "Prix de vente imposé + tax"
            Case 132554
                Return "Modifier recette"
            Case 132555
                Return "Ajouter recette"
            Case 132557
                Return "Créer un nouveau menu"
            Case 132559
                Return "Créer une nouvelle marchandise"
            Case 132561
                Return "Veuillez introduire le numéro de série. Vous trouverez cette information dans les documents accompagnant ce logiciel."
            Case 132565
                Return "Complément"
            Case 132567
                Return "Catégorie de marchandise"
            Case 132568
                Return "Catégorie de recette"
            Case 132569
                Return "Catégorie de menu"
            Case 132570
                Return "L'effacement a échoué"
            Case 132571
                Return "La catégorie est utilisée"
            Case 132586
                Return "Information sur votre compte"
            Case 132589
                Return "Nombre de recettes maximum"
            Case 132590
                Return "Nombre de recettes courantes"
            Case 132592
                Return "Nombre de ingrédients maximum"
            Case 132593
                Return "Nombre de ingrédients courantes"
            Case 132597
                Return "Créer une nouvelle recette"
            Case 132598
                Return "Nombre de menus maximum"
            Case 132599
                Return "Nombre de menus courants"
            Case 132600
                Return "Assigner mots clés"
            Case 132601
                Return "Déplacer les éléments marqués dans une nouvelle catégorie"
            Case 132602
                Return "Effacer les éléments marqués"
            Case 132605
                Return "Bon d'économat"
            Case 132607
                Return "Actions sur les marques"
            Case 132614
                Return "Qté nette"
            Case 132615
                Return "Droits"
            Case 132616
                Return "Propriétaire"
            Case 132617
                Return "TOUTES LES CATEGORIES"
            Case 132621
                Return "Modifier source"
            Case 132630
                Return "Conversion automatique"
            Case 132638
                Return "Information sur les utilisateurs"
            Case 132640
                Return "Ce nom d'utilisateur existe déjà"
            Case 132654
                Return "Gestion de la base de donnée"
            Case 132657
                Return "&Restauration"
            Case 132667
                Return "Fusionner"
            Case 132668
                Return "Purger"
            Case 132669
                Return "Monter"
            Case 132670
                Return "Descendre"
            Case 132671
                Return "Standardiser"
            Case 132672
                Return "Etes-vous sur de vouloir effacer %n?"
            Case 132678
                Return "HACCP"
            Case 132683
                Return "Précédent"
            Case 132706
                Return "Les nutriments sont pour 100g ou 100ml"
            Case 132708
                Return "Pas de fournisseur"
            Case 132714
                Return "Veuillez sélectionner depuis la liste"
            Case 132719
                Return "Le prix pour cette unité a déjà été défini."
            Case 132723
                Return "Le pourcentage de déchets total, ne peut pas être plus grand ou égal à 100%"
            Case 132736
                Return "Qté brute"
            Case 132737
                Return "Ajouter une nouveau fournisseur"
            Case 132738
                Return "Modifier un fournisseur"
            Case 132739
                Return "Détails du fournisseur"
            Case 132740
                Return "Etat"
            Case 132741
                Return "URL"
            Case 132779
                Return "Le mot clé est utilisé"
            Case 132783
                Return "Mot clé"
            Case 132788
                Return "Lien aux nutriments"
            Case 132789
                Return "&Login"
            Case 132793
                Return "Nom d'utilisateur et/ou mot de passe invalide"
            Case 132813
                Return "&Configuration"
            Case 132828
                Return "Recalculer les n&utriments"
            Case 132841
                Return "Ajouter marchandise"
            Case 132846
                Return "Enregistrer les marques"
            Case 132847
                Return "Lire les marques"
            Case 132848
                Return "Filtre"
            Case 132855
                Return "Ajouter menu"
            Case 132860
                Return "Ajouter ingrédient"
            Case 132861
                Return "Modifier ingrédient"
            Case 132864
                Return "Remplacer ingrédient"
            Case 132865
                Return "Ajouter un séparateur"
            Case 132877
                Return "Ajouter un élément"
            Case 132896
                Return "Standardiser les catégories"
            Case 132900
                Return "Ajouter prix"
            Case 132912
                Return "Standardiser les textes"
            Case 132915
                Return "Standardiser les unités"
            Case 132924
                Return "Standardiser les unités de recettes"
            Case 132930
                Return "Miniature"
            Case 132933
                Return "Liste des recettes"
            Case 132934
                Return "Dernière recette"
            Case 132937
                Return "Dernier menu"
            Case 132939
                Return "Liste des menus"
            Case 132954
                Return "Ensemble de marques"
            Case 132955
                Return "Choisissez un nom de marque dans la liste  ou introduisez un nouveau nom de marque pour l'enregistrement"
            Case 132957
                Return "Enregistrer les marques sous le nom"
            Case 132967
                Return "Nutriment"
            Case 132971
                Return "Resumé des nutriments"
            Case 132972
                Return "Les nutriments sont pour 1 portion à 100%"
            Case 132974
                Return "Déchet"
            Case 132987
                Return "Resumé"
            Case 132989
                Return "Affichage"
            Case 132997
                Return "le ou avant"
            Case 132998
                Return "le ou après"
            Case 132999
                Return "entre"
            Case 133000
                Return "plus grand que"
            Case 133001
                Return "plus petit que"
            Case 133005
                Return "Imposé"
            Case 133023
                Return "Options d'affichage"
            Case 133043
                Return "Transformations des photos en local"
            Case 133045
                Return "Taille maximale des fichiers de photos"
            Case 133046
                Return "Taille maximale des photos"
            Case 133047
                Return "Optimisation"
            Case 133049
                Return "Activer la conversion automatique des photos pour l'utilisation sur un site Internet"
            Case 133057
                Return "Télécharger le logo sur le site Internet"
            Case 133060
                Return "Couleurs pour site Internet"
            Case 133075
                Return "Nouveau mot de passe"
            Case 133076
                Return "Confirmer nouveau mot de passe"
            Case 133078
                Return "Les mots de passe ne correspondent pas"
            Case 133080
                Return "Dernier"
            Case 133081
                Return "Premier"
            Case 133085
                Return "Type de document"
            Case 133096
                Return "Préparation de la recette"
            Case 133097
                Return "Prix de revient de la recette"
            Case 133099
                Return "Variation"
            Case 133100
                Return "Détails de la recette"
            Case 133101
                Return "Détails du menu"
            Case 133108
                Return "Qu'imprimer?"
            Case 133109
                Return "Sélection des ingrédients à imprimer"
            Case 133111
                Return "Certaines catégories"
            Case 133112
                Return "Ingrédients marquées"
            Case 133115
                Return "Toutes les recettes"
            Case 133116
                Return "Recettes marquées"
            Case 133121
                Return "Menus marquées"
            Case 133123
                Return "Prix de revient du menu"
            Case 133124
                Return "Description du menu"
            Case 133126
                Return "EGS Standard"
            Case 133127
                Return "EGS Moderne"
            Case 133128
                Return "EGS Deux colonnes"
            Case 133133
                Return "Nom de fichier invalide. Veuillez introduire un nom de fichier valide."
            Case 133144
                Return "Recette no."
            Case 133147
                Return "litres"
            Case 133161
                Return "Taille du papier"
            Case 133162
                Return "Unité des marges"
            Case 133163
                Return "Marge gauche"
            Case 133164
                Return "Marge droite"
            Case 133165
                Return "Marge du haut"
            Case 133166
                Return "Marge du bas"
            Case 133168
                Return "Taille d'écriture"
            Case 133172
                Return "Petite photo / Quantité - Nom"
            Case 133173
                Return "Petite photo / Nom - Quantité"
            Case 133174
                Return "Photo moyenne / Quantité - Nom"
            Case 133175
                Return "Photo moyenne / Nom - Quantité"
            Case 133176
                Return "Grande photo / Quantité - Nom"
            Case 133177
                Return "Grande photo / Nom - Quantité"
            Case 133196
                Return "Options des listes"
            Case 133201
                Return "La/les marchandise(s)  est/sont utilisée(s) et n'a/ont donc pas été effacé."
            Case 133207
                Return "La recette peut être utilisée comme sous-recette"
            Case 133208
                Return "Poids"
            Case 133222
                Return "Détails des options"
            Case 133230
                Return "Les recettes suivantes sont utilisées et n'ont donc pas été effacées."
            Case 133241
                Return "Recalcul des prix en cours. Veuillez patienter..."
            Case 133242
                Return "Recalcul des nutriments en cours. Veuillez patienter..."
            Case 133248
                Return "Ingrédient"
            Case 133251
                Return "Séparateur"
            Case 133254
                Return "Trier par"
            Case 133260
                Return "Sources utilisées"
            Case 133266
                Return "Standardiser les mots clefs"
            Case 133286
                Return "Définition"
            Case 133289
                Return "Unités utilisées"
            Case 133290
                Return "Vous ne pouvez pas fusionner deux ou plusieurs unités de base."
            Case 133295
                Return "Cette unité ne peut pas être effacée." & vbCrLf & "Seul les unités définies par l'utilisateur peuvent être effacées."
            Case 133314
                Return "Seul les unités définies par l'utilisateur peuvent être effacées."
            Case 133315
                Return "Vous ne pouvez pas fusionner deux ou plusieurs unités de recettes de base."
            Case 133319
                Return "Unités de recettes utilisées"
            Case 133325
                Return "Etes-vous sur de vouloir purger toutes les catégories non-utilisées?"
            Case 133326
                Return "Pas de source"
            Case 133328
                Return "Nom de la recette"
            Case 133330
                Return "Fichier manquant."
            Case 133334
                Return "Importation de %r en cours"
            Case 133349
                Return "Menu no."
            Case 133350
                Return "Eléments pour %y (quantité nette)"
            Case 133351
                Return "Ingrédients pour %y" ' à %p% (Quantité nette)"
            Case 133352
                Return "Prix de vente imposé par portion + taxe"
            Case 133353
                Return "Prix de vente imposé par portion"
            Case 133359
                Return "Trié par numéros"
            Case 133360
                Return "Trié par dates"
            Case 133361
                Return "Trié par catégories"
            Case 133365
                Return "Prix de vente + taxe"
            Case 133367
                Return "Trié par fournisseurs"
            Case 133405
                Return "Téléchargement de la photo sur le serveur"
            Case 133475
                Return "Image"
            Case 133519
                Return "Choisissez une couleur :"
            Case 133590
                Return "&Coller"
            Case 133692
                Return "Prix suggéré"
            Case 134021
                Return "Inventaire débuté le"
            Case 134032
                Return "Nos coordonnées"
            Case 134054
                Return "Information personnelle"
            Case 134055
                Return "Achats"
            Case 134056
                Return "Ventes"
            Case 134061
                Return "Version, Modules & Licenses"
            Case 134083
                Return "Test"
            Case 134111
                Return "Impossible d'effacer les éléments marqués"
            Case 134174
                Return "Date de création"
            Case 134176
                Return "Liste de ingrédients-nutriments"
            Case 134177
                Return "Liste de recettes-nutriments"
            Case 134178
                Return "Liste de menus-nutriments"
            Case 134182
                Return "Groupe"
            Case 134194
                Return "Quantité pas valide"
            Case 134195
                Return "Prix pas valide"
            Case 134320
                Return "Adresse de facturation"
            Case 134332
                Return "Information"
            Case 134333
                Return "Important"
            Case 134525
                Return "Etes-vous sur de vouloir abandonner les modifictions effectuées?"
            Case 134571
                Return "Valeur non-autorisée"
            Case 134826
                Return "Fermé"
            Case 135024
                Return "Lieu"
            Case 135056
                Return "Règles des nutriments"
            Case 135058
                Return "Ajouter une règle de nutriments"
            Case 135059
                Return "Modifier une règle de nutriments"
            Case 135070
                Return "Net"
            Case 135100
                Return "Numéro de réf."
            Case 135110
                Return "Quantité" & vbCrLf & "inventaire"
            Case 135235
                Return "Valeur du stock"
            Case 135256
                Return "Quantité vendue"
            Case 135257
                Return "Marge Brute"
            Case 135283
                Return "Dernier prix"
            Case 135608
                Return "Port"
            Case 135948
                Return "Inclure les sous-recettes"
            Case 135951
                Return "le login a échoué."
            Case 135955
                Return "Valeur numérique invalide"
            Case 135963
                Return "Base de données"
            Case 135967
                Return "Remplacement dans les recettes"
            Case 135968
                Return "Remplacement dans les menus"
            Case 135969
                Return "Etes-vous sur de vouloir remplacer %o?"
            Case 135971
                Return "&Connection"
            Case 135978
                Return "Nouveau"
            Case 135979
                Return "Renommer"
            Case 135985
                Return "Existant"
            Case 135986
                Return "Manquant"
            Case 135989
                Return "Eléments"
            Case 135990
                Return "Actualiser"
            Case 136018
                Return "Propriété"
            Case 136025
                Return "Conversion de la base de données"
            Case 136030
                Return "Contenu"
            Case 136100
                Return "Inventaires ouverts"
            Case 136110
                Return "Ouvert le"
            Case 136115
                Return "Nbre d'éléments"
            Case 136171
                Return "Changement d'unité"
            Case 136212
                Return "Montrer la liste des ajustements nécessaires"
            Case 136213
                Return "Ajouter un produit à l'inventaire en cours"
            Case 136214
                Return "Retirer un produit de l'inventaire"
            Case 136215
                Return "Ajouter un nouveau lieu de stockage pour le produit"
            Case 136216
                Return "Retirer de l'inventaire le lieu de stockage sélectionné pour le produit"
            Case 136217
                Return "Retirer la quantité pour le produit-lieu sélectionné"
            Case 136230
                Return "Créer un nouvelle inventaire"
            Case 136231
                Return "Modifier des informations sur l'inventaire"
            Case 136265
                Return "Sous-recettes"
            Case 136432
                Return "Code invalide"
            Case 136601
                Return "Mise à zéro"
            Case 136905
                Return "Symbole monnétaire"
            Case 137019
                Return "Change"
            Case 137030
                Return "Défaut"
            Case 137070
                Return "Paramètres de base"
            Case 138030
                Return "Choisir les produits qui feront parti de l'inventaire."
            Case 138031
                Return "Tous les produits pour les inventaires"
            Case 138032
                Return "Produits des catégories marquées"
            Case 138033
                Return "Produits des lieux de stockage marqués"
            Case 138034
                Return "Produits des fournisseurs marqués"
            Case 138035
                Return "Produits de un ou plusieurs inventaire(s) précédent"
            Case 138137
                Return "Effacé"
            Case 138244
                Return "Article de ventes"
            Case 138402
                Return "Tous les transferts sont terminés avec succès"
            Case 138412
                Return "<pas défini>"
            Case 140056
                Return "Fichier"
            Case 140100
                Return "Sauvegarde de sécurité en cours"
            Case 140101
                Return "Restauration en cours"
            Case 140129
                Return "Erreur lors de la restauration d'une sauvegarde de sécurité"
            Case 140130
                Return "Erreur lors de l'enregistrement d'une sauvegarde de sécurité"
            Case 140180
                Return "Chemin pour enregistrer le fichier de sauvegarde de sécurité"
            Case 143001
                Return "Partager"
            Case 143002
                Return "Retirer le partage"
            Case 143003
                Return "Quantité" & vbCrLf & "nette"
            Case 143008
                Return "Déchet"
            Case 143013
                Return "Modification"
            Case 143014
                Return "Utilisateur"
            Case 143508
                Return "Recette utilisée comme sous-recette"
            Case 143509
                Return "Espace entre les lignes"
            Case 143981
                Return "Numéro de code comptable invalide"
            Case 143987
                Return "Type d'élément"
            Case 143995
                Return "Action"
            Case 144582
                Return "Pas de groupe"
            Case 144591
                Return "Temps"
            Case 144682
                Return "Les nutriments sont pour 100g ou 100ml à 100%"
            Case 144684
                Return "Les nutriments sont pour 1 portion à 100%"
            Case 144685
                Return "par unité de recettes à 100%"
            Case 144686
                Return "par %Y à 100%"
            Case 144687
                Return "par 100g ou 100ml à 100%"
            Case 144688
                Return "P/D"
            Case 144689
                Return "Les nutriments sont pour 1 unité de recette / 100g ou 100ml à 100%"
            Case 144716
                Return "Historique"
            Case 144734
                Return "Liste des articles de vente"
            Case 144738
                Return "Poids par %Y"
            Case 145006
                Return "Transfert"
            Case 146043
                Return "Janvier"
            Case 146044
                Return "Février"
            Case 146045
                Return "Mars"
            Case 146046
                Return "Avril"
            Case 146047
                Return "Mai"
            Case 146048
                Return "Juin"
            Case 146049
                Return "Juillet"
            Case 146050
                Return "Août"
            Case 146051
                Return "Septembre"
            Case 146052
                Return "Octobre"
            Case 146053
                Return "Novembre"
            Case 146054
                Return "Décembre"
            Case 146056
                Return "Marge de  contribution"
            Case 146067
                Return "Balance"
            Case 146080
                Return "Client(s)"
            Case 146114
                Return "Afficher sur une nouvelle page si le fournisseur change"
            Case 146211
                Return "Type de vente"
            Case 147070
                Return "OK"
            Case 147075
                Return "Date invalide"
            Case 147126
                Return "Enlever d'abord les marques existantes"
            Case 147174
                Return "Ouvert"
            Case 147381
                Return "Prix d'inventaire utilisés précédemment pour les produits"
            Case 147441
                Return "Cette article de vente a déjà été lié."
            Case 147462
                Return "Rapport"
            Case 147520
                Return "Principal"
            Case 147647
                Return "Le serveur SQL n'existe pas, ou l'accès est interdit"
            Case 147652
                Return "Enlever"
            Case 147692
                Return "Info. repas"
            Case 147699
                Return "Ecraser"
            Case 147700
                Return "Prix total"
            Case 147703
                Return "Nombre de portions préparées"
            Case 147704
                Return "Qté restante"
            Case 147706
                Return "Qté retournée"
            Case 147707
                Return "Qté perdue"
            Case 147708
                Return "Qté vendue"
            Case 147710
                Return "Qté vendue (spécial)"
            Case 147713
                Return "Style EGS"
            Case 147727
                Return "Coût"
            Case 147729
                Return "Evaluation"
            Case 147733
                Return "Choisissez une langue"
            Case 147737
                Return "Introduisez la quantité et choisissez une unité"
            Case 147743
                Return "Télécharger vers le serveur"
            Case 147748
                Return "Anonyme"
            Case 147750
                Return "Commentaire"
            Case 147753
                Return "Coût du travail"
            Case 147771
                Return "Tarif/Hr"
            Case 147772
                Return "Tarif/Min"
            Case 147773
                Return "Personne"
            Case 147774
                Return "Temps (Heure:Minute)"
            Case 149501
                Return "Utiliser entrée-sortie direct"
            Case 149513
                Return "Approbation"
            Case 149531
                Return "Produits finis"
            Case 149645
                Return "Lié à"
            Case 149706
                Return "Enlever le lien"
            Case 149761
                Return "Afficher"
            Case 149766
                Return "Préfixe"
            Case 149774
                Return "Effacer"
            Case 150009
                Return "Exportation terminée. La recette a été Exportée avec succès."
            Case 150333
                Return "effacé avec succès!"
            Case 150341
                Return "Conversion pour monnaie"
            Case 150353
                Return "Trier"
            Case 150634
                Return "E-mail envoyé avec succès."
            Case 150644
                Return "Le serveur SMTP est nécessaire pour envoyer des e-mail."
            Case 150688
                Return "La licence pour cette application est déjà dépassée"
            Case 150707
                Return "Compte"
            Case 151011
                Return "Bureau principale - Suisse"
            Case 151019
                Return "Mots-clés des ingrédients"
            Case 151020
                Return "Mots-clés des recettes"
            Case 151023
                Return "Enregistrement"
            Case 151250
                Return "Rien n'a été changé"
            Case 151286
                Return "Standard"
            Case 151299
                Return "Veuillez introduire les informations demandées"
            Case 151322
                Return "Inclus dans l'inventaire"
            Case 151336
                Return "Charger un ensemble de marques"
            Case 151344
                Return "Enregistrer les marques des ingrédients"
            Case 151345
                Return "Enregistrer les marques des recettes"
            Case 151346
                Return "Enregistrer les marques des menus"
            Case 151364
                Return "Séléctionner deux ou plus de textes"
            Case 151389
                Return "Purger les textes"
            Case 151400
                Return "Coût des ingrédients"
            Case 151404
                Return "TVA"
            Case 151424
                Return "Conversion à la meilleure unité"
            Case 151427
                Return "Trié par nom"
            Case 151435
                Return "Sujet"
            Case 151436
                Return "Attachement"
            Case 151437
                Return "CALCMENU"
            Case 151438
                Return "CALCMENU"
            Case 151459
                Return "Votre e-mail"
            Case 151499
                Return "Remplacer une proposition"
            Case 151500
                Return "Proposition"
            Case 151854
                Return "Excel"
            Case 151886
                Return "Si vous avez des questions au sujet de l'enregistrement, veuillez nous écrire à: %email"
            Case 151890
                Return "Bonjour %name"
            Case 151906
                Return "Adresse e-mail introuvable."
            Case 151907
                Return "Veuillez introduire votre nom d'utilisateur et mot de passe."
            Case 151910
                Return "Connexion"
            Case 151911
                Return "Déconnexion"
            Case 151912
                Return "Oublié votre mot de passe?"
            Case 151915
                Return "Veuillez fournir les informations demandées ci-dessous."
            Case 151916
                Return "Les champs marqués d'un * sont indispensable."
            Case 151917
                Return "Un message de confirmation vous parviendra par E-mail."
            Case 151918
                Return "Veuillez fournir une adresse E-mail valide."
            Case 151920
                Return "Oui, je souhaite recevoir périodiquement des messages de EGS au sujet de nouveauté ou promotions (pas plus que une fois par mois)."
            Case 151976
                Return "Lieu de production par défault"
            Case 152004
                Return "Arborescence"
            Case 152141
                Return "Gestion des ingrédients"
            Case 152146
                Return "NP"
            Case 155024
                Return "Gestion des photos"
            Case 155046
                Return "Traduction"
            Case 155050
                Return "TOUS LES MOTS CLES"
            Case 155052
                Return "Soumettre"
            Case 155118
                Return "Envoyer le bon d'économat au Pocket PC"
            Case 155163
                Return "Nom de famille"
            Case 155170
                Return "Bienvenue %name!"
            Case 155205
                Return "Accueil"
            Case 155225
                Return "PDF"
            Case 155236
                Return "Langue principale"
            Case 155245
                Return "A notre sujet"
            Case 155260
                Return "Facteur imposé"
            Case 155263
                Return "pixel"
            Case 155264
                Return "Traduction"
            Case 155374
                Return "ID de comptabilité"
            Case 155507
                Return "Active"
            Case 155575
                Return "lieu stock. par déf. sorties caisse"
            Case 155601
                Return "Rien n'a été sélectionné."
            Case 155642
                Return "La bourse aux recettes (Recipe Exchange)"
            Case 155654
                Return "Ingrédients pour %s a %p% (quantité nette)"
            Case 155713
                Return "%r existe(nt)"
            Case 155731
                Return "CALCMENU Pro"
            Case 155761
                Return "Importer les ingrédients"
            Case 155763
                Return "Comparer par numéros"
            Case 155764
                Return "Comparer par noms"
            Case 155811
                Return "Quantité" & vbCrLf & "brute"
            Case 155841
                Return "Fichier à restaurer"
            Case 155842
                Return "Personnes"
            Case 155861
                Return "Quantités à zéro pour les éléments sélectionnés"
            Case 155862
                Return "pour"
            Case 155926
                Return "Exporter vers Excel"
            Case 155927
                Return "TOUTES LES SOURCES"
            Case 155942
                Return "Lire la liste des bons d'économat enregistrés"
            Case 155947
                Return "Filtré par"
            Case 155967
                Return "Séparateur pour fichiers Excel"
            Case 155994
                Return "Pas activé"
            Case 155995
                Return "Contrôle..."
            Case 155996
                Return "Adresse E-mail"
            Case 156000
                Return "Déplacer vers un nouveau fournisseur"
            Case 156012
                Return "Support"
            Case 156015
                Return "Contacts"
            Case 156016
                Return "Bureau principal"
            Case 156060
                Return "Coût des march. imposé"
            Case 156061
                Return "Profit imposé"
            Case 156141
                Return "Sauvegarde/Récupère une base de données"
            Case 156337
                Return "Lien aux nutriments"
            Case 156344
                Return "Sélection invalide"
            Case 156355
                Return "Archives"
            Case 156356
                Return "Inclure"
            Case 156405
                Return "Veuillez faire de la place sur votre disque dur puis cliquer sur Réessayer"
            Case 156413
                Return "Définition de la sous-recette"
            Case 156485
                Return "Effacer le fichier après importation"
            Case 156542
                Return "Prix moyen pondéré"
            Case 156552
                Return "Sauvegarder maintenant"
            Case 156590
                Return "Importation de marchandise depuis un fichier CSV (Excel)"
            Case 156669
                Return "Site Web"
            Case 156672
                Return "Utilisé 'en ligne' (comme contenu du site)"
            Case 156683
                Return "Original"
            Case 156720
                Return "Nombre trop long"
            Case 156721
                Return "Nom trop long"
            Case 156722
                Return "Fournisseur trop long"
            Case 156723
                Return "Catégorie trop longue"
            Case 156725
                Return "Description trop longue"
            Case 156734
                Return "Deux unités sont identiques"
            Case 156742
                Return "Expire après"
            Case 156751
                Return "Direct line: +41 32 544 0017<br><br>24/7 English Customer Support: +1 800 964 9357<br><br>Sales: +41 848 000 357<br>Fax: +41 32 753 0275"
            Case 156752
                Return "24/7 Toll Free: +1-800-964-9357"
            Case 156753
                Return "Office line +632 687 3179"
            Case 156754
                Return "Nom du fichier"
            Case 156784
                Return "Total des erreurs: %n"
            Case 156825
                Return "Milliers"
            Case 156870
                Return "Etes-vous sur?"
            Case 156892
                Return "Telecharger dep"
            Case 156925
                Return "OK Telecharge!"
            Case 156938
                Return "Active"
            Case 156941
                Return "Pocket Kitchen"
            Case 156955
                Return "Privé"
            Case 156957
                Return "Hôtels"
            Case 156959
                Return "Partagé"
            Case 156960
                Return "Soumis"
            Case 156961
                Return "Groupe de prix"
            Case 156962
                Return "Pas soumis"
            Case 156963
                Return "Prix"
            Case 156964
                Return "Trouver dans"
            Case 156965
                Return "Unité de recettes"
            Case 156966
                Return "enregistrements affectés"
            Case 156967
                Return "Veuillez introduire une date valide"
            Case 156968
                Return "Le format de l'image est invalide"
            Case 156969
                Return "Veuillez introduire le nom du ficher d'image ou laisser ce champ vide."
            Case 156970
                Return "Introduire la catégorie"
            Case 156971
                Return "Introduire le prix"
            Case 156972
                Return "Introduire le mot clé"
            Case 156973
                Return "Introduire l'unité"
            Case 156974
                Return "Introduire l'unité de recette"
            Case 156975
                Return "Créer de nouvelles recettes et les soumettre au bureau principale pour utilisation par d'autres hotels."
            Case 156976
                Return "Les ingrédients sont les éléments de base de CALCMENU Web. Ils composent les recettes du systemes."
            Case 156977
                Return "Si vous avez des questions au sujet de ce logiciel."
            Case 156978
                Return "Le mot clé parent"
            Case 156979
                Return "Nom du mot clé"
            Case 156980
                Return "Configuration"
            Case 156981
                Return "Taux de taxe"
            Case 156982
                Return "Résultat de la recherche"
            Case 156983
                Return "Désolé, rien n'a été trouvé."
            Case 156984
                Return "Nom d'utilisateur ou mot de passe invalide"
            Case 156986
                Return "existe déjà."
            Case 156987
                Return "a été enregistré avec succès."
            Case 156996
                Return "Copyright © 2004 de EGS Enggist & Grandjean Software SA, Suisse."
            Case 157002
                Return "Le prix pour l'unité n'a pas été défini. Veuillez choisir une unité."
            Case 157020
                Return "Taxe utilisées"
            Case 157026
                Return "Moyen"
            Case 157033
                Return "Ceci va mettre à jour les prix de toutes les ingrédients. Veuillez patienter..."
            Case 157034
                Return "Autentification"
            Case 157038
                Return "Quantité"
            Case 157039
                Return "Quantité"
            Case 157040
                Return "Il n'y a pas de mot clé disponible."
            Case 157041
                Return "l'accès est interdit"
            Case 157049
                Return "Etes-vous sur de vouloir enregistrer?"
            Case 157055
                Return "VERSION ETUDIANT"
            Case 157056
                Return "Souhaitez-vous vraiement annuler?"
            Case 157057
                Return "Les éléments marqués sont maintenant partagés."
            Case 157060
                Return "Numéro de référence"
            Case 157065
                Return "Exporter vers CALCMENU"
            Case 157066
                Return "Exporter vers CALCMENU"
            Case 157076
                Return "Sommaire de l'aide"
            Case 157079
                Return "Les éléments marqués suivants n'ont pas été soumis et ne peuvent pas être tranférrés."
            Case 157084
                Return "Les éléments marqués suivants sont utilisés et n'ont pas été effacés."
            Case 157125
                Return "Affichage"
            Case 157130
                Return "Nous avons reçu vos informations avec succès. Votre inscription sera traitée dans les 3 jours. Merci!"
            Case 157132
                Return "Personnel (partagé)"
            Case 157133
                Return "Personnel (pas partagé)"
            Case 157134
                Return "Visiteur"
            Case 157136
                Return "Crédits"
            Case 157139
                Return "Pire!"
            Case 157140
                Return "Bon!"
            Case 157141
                Return "Fantastique!"
            Case 157142
                Return "Effacer les unités des ingrédients qui ne sont pas utile avant d'importer"
            Case 157151
                Return "Autres liens"
            Case 157152
                Return "Revu des utilisateurs"
            Case 157153
                Return "Le destinataire devra accepter ces produits."
            Case 157154
                Return "Les éléments suivants ne peuvent pas être donnés car ils sont la propriété d'autres utilisateurs"
            Case 157155
                Return "Quelqu'un souhaite vous donner les recettes suivantes:"
            Case 157156
                Return "Promo"
            Case 157157
                Return "Opinions des utilisateurs"
            Case 157158
                Return "Originalité"
            Case 157159
                Return "R"
            Case 157160
                Return "Difficulté"
            Case 157161
                Return "Recette du jour"
            Case 157164
                Return "Nom du propriétaire de la carte"
            Case 157165
                Return "Numéro de la carte de crédit"
            Case 157166
                Return "Limite des enregistrements"
            Case 157168
                Return "Banque"
            Case 157169
                Return "PayPal"
            Case 157170
                Return "Les commandes par Internet ne sont pas possible depuis votre pays."
            Case 157171
                Return "Devenir un membre"
            Case 157172
                Return "Coût de mise à niveau"
            Case 157173
                Return "Coût de la souscription"
            Case 157174
                Return "Offre de mise à niveau"
            Case 157176
                Return "Nombre total d'enregistrements utilisés"
            Case 157177
                Return "Nous offrons une variété de solutions adaptées à vos besoins"
            Case 157178
                Return "Utilisateur de test"
            Case 157179
                Return "Parlez en à un ami"
            Case 157180
                Return "L'adresse d'un(e) ami(e)"
            Case 157182
                Return "FAQ"
            Case 157183
                Return "Conditions d'utilisation du service"
            Case 157214
                Return "Créer un bon d'économat pour les recettes marquées uniquement"
            Case 157217
                Return "Créer un bon d'économat pour les menus marqués uniquement"
            Case 157226
                Return "Les recettes marqués ont été envoyées pour approbation."
            Case 157233
                Return "Le pourcentage de déchets total, ne peut pas être plus grand ou égal à 100%"
            Case 157268
                Return "Monnaie utilisée"
            Case 157269
                Return "Un ensemble de prix est utilisé."
            Case 157273
                Return "Impossible de partager les articles suivants; ils n'ont pas été soumis et ne sont pas à vous."
            Case 157274
                Return "Taux de change"
            Case 157275
                Return "Tous les articles sélectionnés seront fusionnés en un seul. Veuillez sélectionner un article à utiliser par les utilisateurs. Les autre articles seront effacés de la base de données."
            Case 157276
                Return "Fusionné avec succès."
            Case 157277
                Return "Coût total"
            Case 157281
                Return "Prix du fournisseur par défaut"
            Case 157297
                Return "Veuillez sélectionner au moins un article."
            Case 157299
                Return "Modifier le profil et personnaliser votre affichage."
            Case 157300
                Return "Veuillez taper votre nouveau mot de passe. Celui-ci ne doit pas avoir plus de 20 caractères. Cliquez ensuite sur 'Soumettre'."
            Case 157301
                Return "Veuillez taper le nom du fichier image (jpeg/jpg, bmp, etc.) à télécharger sur le serveur. Please enter the image file (jpeg/jpg, bmp, etc.) that you want to upload. Laissez ce champ vide si vous n'avez pas d'image. (NB : Le système n'accepte pas les fich"
            Case 157302
                Return "Rechercher un ingrédient par son nom ou une partie de son nom (utilisez l'astérisque [*]). Pour l'ajouter rapidement, tapez [quantité nette]_[unité]_[ingrédient]. Ex.: 200 g huile Oleic"
            Case 157303
                Return "Pour ajouter ou modifier le prix de la marchandise, tapez le nouveau prix et indiquez l'unité de mesure. Assignez le ratio de cette unité par rapport à l'unité originale. Ex. : À l'origine, le prix et l'unité étaient 11 $US par kilo. Pour ajouter l'unité"
            Case 157304
                Return "Rechercher des mots-clés par leur nom ou une partie de leur nom. Séparez les mots-clés d'une virgule [,]. Ex. : ""boeuf, sauce, mariage""."
            Case 157305
                Return "Veuillez choisir un élément"
            Case 157306
                Return "Type de fichier invalide."
            Case 157310
                Return "Détails de la marchandise"
            Case 157314
                Return "Utiliser les unités principales lors de l'ajout de ingrédients"
            Case 157320
                Return "Partage"
            Case 157322
                Return "Accord d'utilisation"
            Case 157323
                Return "Donner"
            Case 157329
                Return "Terminal"
            Case 157334
                Return "Attention: Vous pouvez perdre tous les changements si un autre utilisateur a modifié ces données. Souhaitez-vous rafraichir cette page?"
            Case 157336
                Return "Pas applicable"
            Case 157339
                Return "Messages par page"
            Case 157340
                Return "Consultation rapide"
            Case 157341
                Return "sur chaque page"
            Case 157342
                Return "L'enregistrement a été modifié par un autre utilisateur. Cliquez OK pour continuer."
            Case 157343
                Return "Cette enregistrement a été effacé par un autre utilisateur."
            Case 157345
                Return "Soumettre au bureau principal"
            Case 157346
                Return "Pas partagé"
            Case 157378
                Return "Membre"
            Case 157379
                Return "Inscrivez-vous maintenant"
            Case 157380
                Return "Votre abonnement expirera le %n. "
            Case 157381
                Return "Votre abonnement a expiré."
            Case 157382
                Return "Prolonger mon adhésion en utilisant les points qui me restent (crédits)"
            Case 157383
                Return "Veuillez faire de la place sur votre disque dur puis cliquer sur Réessayer"
            Case 157384
                Return "Transaction invalide"
            Case 157385
                Return "Merci!"
            Case 157387
                Return "Vous allez être réorienté vers Paypal pour créer votre compte. Veuillez prendre un moment pour choisir la monaie à utiliser pour verser le montant correspondant. Choisissez svp dans la liste ci-dessous."
            Case 157388
                Return "Joindre une invitation "
            Case 157404
                Return "Transaction en attente."
            Case 157405
                Return "Pour toute question, veuillez nous envoyer un email à  "
            Case 157408
                Return "Seuls les membres et les utilisateurs de versions démos peuvent accéder à cette page. Voulez vous gérer votre propre recette sur www.recipegallery.com ? Alors devenez membre en vous inscrivant dans le menu abonnement. "
            Case 157435
                Return "Transfère automatique vers un lieu de vente avant la sortie"
            Case 157437
                Return "Ingrédients brutes"
            Case 157446
                Return "Mois"
            Case 157515
                Return "Hollandais"
            Case 157594
                Return "Accepter"
            Case 157595
                Return "Refuser"
            Case 157596
                Return "Pas de revu d'utilisateur"
            Case 157604
                Return "Support par E-mail"
            Case 157607
                Return "Support par téléphone"
            Case 157608
                Return "Support par Internet"
            Case 157616
                Return "Etats-Unis d'Amériques"
            Case 157617
                Return "Asie et reste du monde"
            Case 157629
                Return "Approbation"
            Case 157633
                Return "Désapprouve"
            Case 157659
                Return "Bloquer"
            Case 157660
                Return "Débloquer"
            Case 157695
                Return "Compte de référence"
            Case 157714
                Return "Commentaires"
            Case 157772
                Return "Optionel"
            Case 157793
                Return "A propos de"
            Case 157802
                Return "Confirmer le mot de passe"
            Case 157901
                Return "Cachez existant"
            Case 157926
                Return "Inscrivez-vous"
            Case 157985
                Return "Il vous est toujours possible de modifier votre mot de passe de la façon suivante :"
            Case 157986
                Return "Accédez au site web d'EGS au <a href='http://www.eg-software.com/f/'>http://www.eg-software.com/f/</a>."
            Case 157992
                Return "Vous avez récemment fait la demande d'un nom d'utilisateur et d'un mot de passe afin d'accéder à votre compte utilisateur EGS."
            Case 157993
                Return "Vous trouverez les détails ci-dessous."
            Case 158005
                Return "Licences"
            Case 158019
                Return "Vérifier l'état de la demande"
            Case 158157
                Return "Ingrédients pour %y"
            Case 158169
                Return "Veuillez choisir vos modalités de paiement." & vbCrLf & "" & vbCrLf & "Paiement à l'avance par :"
            Case 158170
                Return "Veuillez nous envoyer vos données de carte de crédit par e-mail à <a href='mailto:info@calcmenu.com'>info@calcmenu.com</a>. Type de carte (Visa, Mastercard, American Express), nom apparaissant sur la carte, numéro et date d'expiration de la carte."
            Case 158171
                Return "Transfert bancaire/électronique"
            Case 158174
                Return "Remarque : Une fois le transfert effectué, veuillez nous en informer. Nous recevrons confirmation du transfert par notre banque une à deux semaines plus tard."
            Case 158186
                Return "Modifier le mot de passe"
            Case 158216
                Return "Gestion centralisée de recettes, n'importe quand et n'importe où"
            Case 158220
                Return "Créer des nouvelles ingrédients avec un nom jusqu'à 250 caractères, un numéro de produits alphanumérique, un taux de taxe, quatre pourcentages de déchets, catégorie, fournisseur et d'autres informations utiles tels que description du produit, préparation, conseils de cuisson, méthode de raffinement et de stockage."
            Case 158229
                Return "Photos"
            Case 158230
                Return "Les ingrédients, recettes et menus peuvent être recherchés par leur nom ou numéro de référence. Vous pouvez également chercher par la catégories ou des mots clés. Pour les ingrédients, vous pouvez aussi utiliser le fournisseur, la date d'introduction ou de modification, une fourchette de prix, ou par les valeurs nutritives. Pour les recettes et menus, vous pouvez en plus de ces critères, également cherches par les ingrédients contenus dans celle-ci."
            Case 158232
                Return "Les commandes ""Action sur les marques"" permettent d'effectuer certaines actions simultanément sur plusieurs ingrédients, recettes, menus, etc. qui ont été marqués. Vous pouvez utiliser cette commande pour attribuer une catégories ou mots clés à un"
            Case 158234
                Return "Liens de nutriments et calcul"
            Case 158238
                Return "Gestion de fournisseurs"
            Case 158240
                Return "Gestion des catégories, mots clés, sources"
            Case 158243
                Return "Gestion des taux de TVA"
            Case 158246
                Return "Gestion de taux de taxes"
            Case 158249
                Return "Impression, PDF et exportation Excel"
            Case 158306
                Return "Sélectionnez"
            Case 158346
                Return "plus"
            Case 158349
                Return "Mot clé assigné"
            Case 158350
                Return "Mot clé dérivé"
            Case 158376
                Return "Prix de vente imposé théorique"
            Case 158410
                Return "Si certains produits n'ont pas de prix définis (prix=0), utiliser en remplacement le prix du fournisseur par défaut."
            Case 158511
                Return "Si vous croyez que ce n'est pas le cas, veuillez écrire à <a href='mailto:%email'>%email</a>."
            Case 158577
                Return "Langue du site"
            Case 158585
                Return "Siége de la société"
            Case 158588
                Return "Vous ne pouvez pas soumettre les éléments suivants car vous n'en êtes pas le propriétaire."
            Case 158653
                Return "Natel"
            Case 158677
                Return "Numéro" & vbCrLf & "article de vente"
            Case 158694
                Return "Modifier les infos"
            Case 158696
                Return "Pour les clients des Philippines seulement"
            Case 158730
                Return "Exclure"
            Case 158734
                Return "La base de données n'est pas compatible avec cette version du logiciel."
            Case 158783
                Return "Include recette(s)/sous-recette(s)"
            Case 158810
                Return "Calculer le prix"
            Case 158835
                Return "Trié par taxe"
            Case 158837
                Return "Trié par prix"
            Case 158839
                Return "Trié par coût des ingrédients"
            Case 158840
                Return "Trié par constante"
            Case 158845
                Return "Trié par prix de vente"
            Case 158846
                Return "Trié par prix imposé"
            Case 158849
                Return "Elevée"
            Case 158850
                Return "Faible"
            Case 158851
                Return "Crée par"
            Case 158860
                Return "Modifier la configuration des caisses"
            Case 158868
                Return "Chinois"
            Case 158902
                Return "Heure d'ouverture"
            Case 158912
                Return "Demandes"
            Case 158935
                Return "Revenu total"
            Case 158946
                Return "Définir la quantité en stock comme quantité d'inventaire"
            Case 158947
                Return "Vous allez être redirigé vers Paypal pour passer votre commande."
            Case 158952
                Return "Approuvé"
            Case 158953
                Return "Pas approuvé"
            Case 158960
                Return "Cette fonction a été désactivée. Veuillez contact votre bureau principal si vous souhaitez ajouter de nouvelles recettes"
            Case 158998
                Return "Fonctions de recherche"
            Case 158999
                Return "Les listes de ingrédients, recettes et menus peuvent être imprimées ensemble avec leur détails, prix et valeures nutritives. Le bon d'économat avec les quantités cummulées de chaque ingrédient utilisés dans plusieurs recettes ou menus peuvent également être imprimés. Ces rapports sont également disponible en format PDF ou Excel."
            Case 159000
                Return "Groupe de prix et gestion de multiple monnaies"
            Case 159009
                Return "Bordure"
            Case 159035
                Return "Incomplet"
            Case 159064
                Return "Le nom ne peut pas être vide"
            Case 159082
                Return "Mise à jour des produits en fonction de la date de dernière modification"
            Case 159088
                Return "Envoyer une demande d'approbation"
            Case 159089
                Return "Annuler la demande d'approbation"
            Case 159112
                Return "Pour approbation"
            Case 159113
                Return "Dont on peut hériter"
            Case 159133
                Return "Information sur l'envoi par poste"
            Case 159139
                Return "Composition"
            Case 159140
                Return "Unité trop longue"
            Case 159141
                Return "L'unité %n n'existe pas."
            Case 159142
                Return "%n ne peut pas être vide."
            Case 159144
                Return "Importation du fichier. Veuillez patienter..."
            Case 159145
                Return "Enregistrement en cours. Veuillez patienter..."
            Case 159162
                Return "&Cacher les détails"
            Case 159168
                Return "Trié par quantités nettes"
            Case 159169
                Return "Trié par quantités brutes"
            Case 159171
                Return "Horaire"
            Case 159181
                Return "Trié par montant"
            Case 159264
                Return "Importation de ingrédients CSV/Réseau de fournisseurs"
            Case 159273
                Return "Marge de contribution totale"
            Case 159274
                Return "%number only"
            Case 159275
                Return "Limité par le nombre de licences"
            Case 159298
                Return "Mot clé de menu"
            Case 159349
                Return "Annuler filtre"
            Case 159350
                Return "Votre abonnement de support et mise à jour a expiré."
            Case 159360
                Return "Chef propriétaire"
            Case 159361
                Return "Chef principal"
            Case 159362
                Return "Article sélectionné en cours d'utilisation."
            Case 159363
                Return "Entrer les renseignements sur la marque"
            Case 159364
                Return "Marque"
            Case 159365
                Return "Rôle"
            Case 159366
                Return "Utilisation du SMTP sur le serveur"
            Case 159367
                Return "Utilisation du SMTP sur le réseau"
            Case 159368
                Return "Logo"
            Case 159369
                Return "Comparer par"
            Case 159370
                Return "importé(s) avec succès"
            Case 159372
                Return "Mondial"
            Case 159379
                Return "croissant"
            Case 159380
                Return "décroissant"
            Case 159381
                Return "Montrer à tous les utilisateurs"
            Case 159382
                Return "Convertir en recette système"
            Case 159383
                Return "Ne pas montrer"
            Case 159384
                Return "Propriété"
            Case 159385
                Return "Soumettre l'entrée"
            Case 159386
                Return "Les prix et nutriments n'ont pas été recalculés."
            Case 159387
                Return "Les prix et nutriments ont été recalculés."
            Case 159388
                Return "Créer une nouvelle carte"
            Case 159389
                Return "Modifiez la carte du menu"
            Case 159390
                Return "E-mail envoyé."
            Case 159391
                Return "Prix approuvé"
            Case 159424
                Return "Cette fonction a été désactivée. Veuillez contact votre bureau principal si vous souhaitez ajouter de nouvelles ingrédients "
            Case 159426
                Return "Rechercher un ingrédient par son nom ou une partie de son nom. Pour l'ajouter rapidement, tapez [quantité nette]_[unité]_[ingrédient]"
            Case 159430
                Return "Les informations sur l'enregistrement ont été sauvegardées avec succès."
            Case 159433
                Return "Soumettre au système"
            Case 159434
                Return "Soumis au système"
            Case 159435
                Return "Déplacer les éléments marqués dans une nouvelle catégorie"
            Case 159436
                Return "Écrire à l'expéditeur dans le cas d'alertes système"
            Case 159437
                Return "Le téléchargement du fichier sur le serveur a réussi."
            Case 159444
                Return "Imposer une taille d'image"
            Case 159445
                Return "Fuseau horaire"
            Case 159446
                Return "Traitement d'image"
            Case 159457
                Return "La recherche en texte intégral du Serveur SQL a la possibilité d'exécuter des requêtes complexes. Ces recherches peuvent inclure des mots ou des phrases, des recherches de proximité, des correspondances d'inflections (ex.: conduire = un conduit) et taux de pertinence ( mots les plus proches)"
            Case 159458
                Return "Ensemble des éléments"
            Case 159459
                Return "Recherche en Texte Intégral"
            Case 159460
                Return "minute"
            Case 159461
                Return "Chaque"
            Case 159462
                Return "Démarrer"
            Case 159463
                Return "Eléments par Incréments"
            Case 159464
                Return "Séparateur de mots"
            Case 159468
                Return "Utilisé comme ingrédient"
            Case 159469
                Return "Pas utilisé comme ingrédient"
            Case 159471
                Return "Adresse IP"
            Case 159472
                Return "Liste des IP bloquées."
            Case 159473
                Return "Bloquer l'IP lorsque le nombre d'essais atteint"
            Case 159474
                Return "Veuillez entrer au moins " & vbCrLf & " caractères"
            Case 159485
                Return "Soumettre à la bourse aux recettes (Recipe Exchange)"
            Case 159486
                Return "Soumis à la bourse aux recettes (Recipe Exchange)"
            Case 159487
                Return "Vous avez validé cette recette. Elle peut maintenant être vue par tous les utilisateurs."
            Case 159488
                Return "Langue inconnue"
            Case 159594
                Return "&Ajouter à la recette"
            Case 159607
                Return "Logiciel de gestion de recettes mono-poste"
            Case 159608
                Return "Logiciel de gestion de recettes pour multi-utilisateurs sur un réseau"
            Case 159609
                Return "Logiciel de gestion de recettes pour le web"
            Case 159610
                Return "Logiciel de gestion de stock et inventaire"
            Case 159611
                Return "Visualisateur de recettes pour Pocket PC"
            Case 159612
                Return "Logiciel de gestion de repas à la carte en restauration collective"
            Case 159613
                Return "Logiciel E-Cookbook (Livre de recettes)"
            Case 159681
                Return "La recette (%s) a trop d'ingrédients. (Max. est %n)"
            Case 159689
                Return "Soumise avec photo"
            Case 159690
                Return "Soumise sans photo"
            Case 159699
                Return "Mettre à jour les éléments existants"
            Case 159700
                Return "&Importer des recettes/menus"
            Case 159707
                Return "France"
            Case 159708
                Return "Allemagne"
            Case 159733
                Return "Article no."
            Case 159751
                Return "Site"
            Case 159778
                Return "Avancé"
            Case 159779
                Return "Normal"
            Case 159782
                Return "Lier les articles de vente aux produits"
            Case 159783
                Return "Lier les articles de vente aux recettes/menus"
            Case 159795
                Return "Importation des caisses - Configuration"
            Case 159918
                Return "Vous n'avez pas le droit d'accéder à cette fonction. Veuillez contacter votre administrateur pour changer vos droits."
            Case 159924
                Return "Gérer"
            Case 159925
                Return "Conversion non valide"
            Case 159929
                Return "Options de page"
            Case 159934
                Return "Inclure l'information sur les nutriments"
            Case 159940
                Return "Exporter les mises à jour"
            Case 159941
                Return "Exporter tout"
            Case 159942
                Return "Répertoire de sortie"
            Case 159943
                Return "Qualité"
            Case 159944
                Return "Parent"
            Case 159946
                Return "CALCMENU Web 2008"
            Case 159947
                Return "Sélectionner ou télécharger un fichier sur le serveur"
            Case 159949
                Return "Le format ne peut pas compter plus de 10 caractères."
            Case 159950
                Return "Le nom du nutriment ne peut pas compter plus de 25 caractères."
            Case 159951
                Return "Rôles"
            Case 159962
                Return "Entrer les données sur la taxe"
            Case 159963
                Return "Entrer la traduction"
            Case 159966
                Return "Assigner les articles marqués à une autre marque"
            Case 159967
                Return "Entrer le nom du site par défaut :"
            Case 159968
                Return "Entrer le thème du site par défaut"
            Case 159969
                Return "Permettre le regroupement de sites par propriété à gérer par l'administrateur de la propriété :"
            Case 159970
                Return "Exiger des utilisateurs qu'ils soumettent les données à l'approbateur avant que celles-ci puissent être utilisées ou publiées :"
            Case 159971
                Return "Entrer la traduction dans chaque langue correspondante, sans quoi le texte par défaut sera utilisé :"
            Case 159973
                Return "Sélectionner les sites qui doivent appartenir à cette propriété."
            Case 159974
                Return "Sélectionner les langues offertes pour traduire ingrédients, recettes, menus et autres données."
            Case 159975
                Return "Sélectionner un ou plusieurs groupes de prix à utiliser pour assigner des prix à vos ingrédients, recettes et menus."
            Case 159976
                Return "Cocher les articles à inclure"
            Case 159977
                Return "Liste des propriétaires"
            Case 159978
                Return "Choisir parmi les formats ci-dessous"
            Case 159979
                Return "Choisir la liste de base à purger"
            Case 159981
                Return "Voici les sites partagés pour cet article"
            Case 159982
                Return "Assigner la nouvelle source aux marques"
            Case 159987
                Return "Type de demande"
            Case 159988
                Return "Demandé par"
            Case 159990
                Return "Modifier la marque"
            Case 159994
                Return "Remplacer une marchandise/recette du menu"
            Case 159997
                Return "Partage mondial"
            Case 160004
                Return "Premier niveau"
            Case 160005
                Return "L'ingrédient sélectionné devrait porter les unités suivantes :"
            Case 160008
                Return "Étape"
            Case 160009
                Return "Autres actions"
            Case 160012
                Return "Cette recette ou ce menu est publié sur le web."
            Case 160013
                Return "Cette recette ou ce menu n'est pas publié sur le web."
            Case 160014
                Return "Se souvenir de moi"
            Case 160016
                Return "Afficher les propriétaires"
            Case 160018
                Return "Cette marchandise est publiée sur le web."
            Case 160019
                Return "Cette marchandise n'est pas publiée sur le web."
            Case 160020
                Return "Cette marchandise est exposée."
            Case 160021
                Return "Cette marchandise n'est pas exposée."
            Case 160023
                Return "Pour impression"
            Case 160028
                Return "Ne pas publier"
            Case 160030
                Return "Ajouter à la liste d'emplettes"
            Case 160033
                Return "Ajouter des mots-clés"
            Case 160035
                Return "Vous avez tenté d'ouvrir une session %n fois."
            Case 160036
                Return "Ce compte est maintenant désactivé."
            Case 160037
                Return "Pour le réactiver, communiquez avec votre administrateur."
            Case 160038
                Return "Mon profil"
            Case 160039
                Return "Dernière session"
            Case 160040
                Return "Pas de session ouverte."
            Case 160041
                Return "Langue de la page"
            Case 160042
                Return "Traduction principale"
            Case 160043
                Return "Catalogue de prix principal"
            Case 160045
                Return "Lignes par page"
            Case 160046
                Return "Affichage par défaut"
            Case 160047
                Return "Quantités d'ingrédients"
            Case 160048
                Return "Dernier accès"
            Case 160049
                Return "Reçu '%f'"
            Case 160050
                Return "Longueur"
            Case 160051
                Return "Pas reçu '%f'"
            Case 160055
                Return "La quantité doit être supérieure à 0."
            Case 160056
                Return "Créer une nouvelle sous-recette"
            Case 160057
                Return "Votre session est échue."
            Case 160058
                Return "Votre session est échue en raison d'une période d'inactivité de %n minutes."
            Case 160065
                Return "Aucun nom"
            Case 160066
                Return "Souhaitez-vous vraiment clore la session?"
            Case 160067
                Return "Votre entrée doit être approuvée."
            Case 160068
                Return "Pour demander une approbation, cliquez sur le bouton '%s'."
            Case 160070
                Return "Articles cochés à traiter"
            Case 160071
                Return "Cette entrée a été soumise pour approbation."
            Case 160072
                Return "Une demande est déjà associée à cette entrée."
            Case 160074
                Return "Sélectionner l'unité"
            Case 160082
                Return "De nouvelles demandes attendent votre approbation."
            Case 160085
                Return "Votre demande a été étudiée."
            Case 160086
                Return "Imprimer Liste des nutriments"
            Case 160087
                Return "Imprimer Liste"
            Case 160088
                Return "Imprimer Détails"
            Case 160089
                Return "Activer"
            Case 160090
                Return "Créer"
            Case 160091
                Return "Enlever de la liste les articles sélectionnés"
            Case 160093
                Return "Soumettre au Système en vue d'un partage mondial"
            Case 160094
                Return "Offrir le contenu sur le navigateur du kiosque"
            Case 160095
                Return "Créer une copie du Système"
            Case 160096
                Return "Remplacer un ingrédient utilisé dans les recettes et menus"
            Case 160098
                Return "Ne pas publier sur le web"
            Case 160100
                Return "Créer une liste d'ingrédients à acheter"
            Case 160101
                Return "Il est possible d'utiliser du texte comme un ingrédient sans quantités ou prix définis."
            Case 160102
                Return "Créez votre propre base de recettes, partagez-la avec les autres utilisateurs, imprimez-la et utilisez-la pour générer votre liste d'emplettes."
            Case 160103
                Return "Menu est la liste des recettes et ingrédients offerts dans un repas."
            Case 160105
                Return "Organiser les renseignements de base, par ex. ceux en lien avec les utilisateurs, fournisseurs, etc."
            Case 160106
                Return "Bienvenue"
            Case 160107
                Return "Bienvenue sur %s"
            Case 160108
                Return "Personnaliser l'affichage et les autres paramètres"
            Case 160109
                Return "Profil du site web"
            Case 160110
                Return "Personnaliser le nom du site web, ses thèmes, etc."
            Case 160111
                Return "Acheminement de l'approbation"
            Case 160112
                Return "Approbation des ingrédients, recettes et autres données"
            Case 160113
                Return "Réglages des avis en lien avec les alertes et le SMTP"
            Case 160114
                Return "Configurer la connexion à votre serveur de courrier; activer ou désactiver les alertes."
            Case 160115
                Return "Régler le nombre maximal d'essais pour ouvrir une session et surveiller les adresses IP bloquées."
            Case 160116
                Return "Profil d'impression"
            Case 160117
                Return "Définir plusieurs formats d'impression en tant que profils"
            Case 160118
                Return "Définir la liste des langues utilisées dans la traduction des ingrédients, recettes, menus et autres données."
            Case 160119
                Return "Devises offertes pour la conversion monétaire et les définitions de prix."
            Case 160120
                Return "Utiliser plusieurs jeux de prix avec les ingrédients, recettes et menus."
            Case 160121
                Return "Les propriétés sont des groupes de sites."
            Case 160122
                Return "Les sites permettent d'organiser les utilisateurs qui travaillent ensemble sur un ensemble spécifique de recettes."
            Case 160123
                Return "Gérer les utilisateurs qui travaillent sur %s"
            Case 160124
                Return "Préférences de traitement des images"
            Case 160125
                Return "Définir la taille standard des images pour les ingrédients, recettes et menus."
            Case 160130
                Return "Marques de commerce ou noms distinctifs servant à identifier les ingrédients."
            Case 160132
                Return "Sert à regrouper ingrédients, recettes et menus par attributs partagés."
            Case 160135
                Return "Les mots-clés décrivent plus en détail ingrédients, recettes et menus. Les utilisateurs peuvent assigner plusieurs mots-clés à chaque marchandise, recette, ou menu."
            Case 160139
                Return "On peut définir jusqu'à 34 valeurs nutritionnelles pour des nutriments tels que : Énergie, Glucides, Protéines et Lipides."
            Case 160141
                Return "Créer des règles qui serviront de filtres de recherche supplémentaires."
            Case 160151
                Return "%s est accompagné d'une liste d'unités prédéfinies (unités système) qui servent à définir le prix des ingrédients et à encoder recettes et menus."
            Case 160152
                Return "Les utilisateurs peuvent apporter des ajouts à cette liste."
            Case 160153
                Return "Utilisé dans le calcul des prix."
            Case 160154
                Return "Source désigne la provenance d'une recette donnée. Il peut s'agir d'un chef, d'un livre, d'un magazine, d'un service alimentaire, d'un organisme ou d'un site web."
            Case 160155
                Return "Importer les ingrédients, recettes et menus de CALCMENU Pro, CALCMENU Enterprise, ou un autre produit EGS."
            Case 160156
                Return "Mise à jour du taux de change de diverses monnaies."
            Case 160157
                Return "Supprimer les textes inutilisés."
            Case 160158
                Return "Formater tous les textes."
            Case 160159
                Return "Imprimer la liste des ingrédients"
            Case 160160
                Return "Imprimer le détails de la marchandise"
            Case 160161
                Return "Imprimer le détails des recettes"
            Case 160162
                Return "Impression de la liste des recettes"
            Case 160163
                Return "Imprimer le détails du menu"
            Case 160164
                Return "L'ingénierie des menus permet d'évaluer la conception et le coût actuels et futurs des recettes."
            Case 160169
                Return "Charger la liste des cartes"
            Case 160170
                Return "Modifier ou afficher en aperçu les cartes enregistrées."
            Case 160175
                Return "Modifier, imprimer ou afficher en aperçu les listes d'emplettes enregistrées."
            Case 160177
                Return "Sécurité"
            Case 160180
                Return "Normaliser le format des articles"
            Case 160181
                Return "Éliminer les articles inutiles"
            Case 160182
                Return "Droits du rôle"
            Case 160184
                Return "Exporter TCPOS"
            Case 160185
                Return "Exporter les articles de vente"
            Case 160187
                Return "Créer de nouvelles ingrédients locales qui serviront d'ingrédients dans vos recettes."
            Case 160188
                Return "Afficher la liste des marques enregistrées"
            Case 160189
                Return "Afficher la liste des articles à acheter"
            Case 160190
                Return "Créer vos propres menus à partir des recettes qui figurent dans la base de données."
            Case 160191
                Return "Créer un texte qui servira dans les recettes et les menus."
            Case 160200
                Return "Trié par noms de menus"
            Case 160202
                Return "A choisir dans la liste"
            Case 160209
                Return "Veuillez introduire le numéro de série. Vous trouverez cette information dans les documents accompagnant ce logiciel."
            Case 160210
                Return "Articles désirés"
            Case 160211
                Return "Articles indésirables"
            Case 160212
                Return "Brouillons"
            Case 160217
                Return "Emplacement des archives"
            Case 160218
                Return "Erreurs à l'importation des données CSV"
            Case 160219
                Return "Liste des ingrédients en attente et devant être corrigées"
            Case 160220
                Return "Définir les options pour l'importation des ingrédients"
            Case 160232
                Return "Exporter vers"
            Case 160237
                Return "Gras"
            Case 160254
                Return "Veuillez remettre en marche le service Window %n  pour prendre en compte les changements."
            Case 160258
                Return "La monnaie ne correspond pas au groupe de prix choisi ."
            Case 160259
                Return "Le nom ou le nombre existe déjà."
            Case 160260
                Return "Date d'importation"
            Case 160262
                Return "Les nutriments sont pour 1 portion à 100%"
            Case 160292
                Return "Allergènes"
            Case 160293
                Return "Liste d'allergies alimentaires ou sensibilités associés à des ingrédients."
            Case 160295
                Return "Le compte est actuellement utilisé. Svp essayez de nouveau un peu plus tard."
            Case 160353
                Return "Catalogue de Prix d'Achat"
            Case 160354
                Return "Catalogue de Prix de Vente"
            Case 160414
                Return "Qté inventaire" & vbCrLf & "précédent"
            Case 160423
                Return "Logiciel de gestion de recettes/menus mono-poste"
            Case 160433
                Return "Consommation dans les"
            Case 160500
                Return "Gestion des libellés ou Textes"
            Case 160687
                Return "Couleur d'article alternative"
            Case 160688
                Return "Couleur d'article normale"
            Case 160690
                Return "Veuillez noter que si vous faites un Reset de votre système, tous les autres utilisateuras seront automatiquement arrêtés."
            Case 160691
                Return "Sauvegarder / restaurer les Images"
            Case 160716
                Return "Placez les articles à global par défaut"
            Case 160774
                Return "Désactiver"
            Case 160775
                Return "Retirer les zéros complémentaires"
            Case 160776
                Return "Retourner à %s"
            Case 160777
                Return ""
            Case 160788
                Return "Elément(s) sélectionné(s) ont été activé(s)"
            Case 160789
                Return "Elément(s) sélectionné(s) ont été désactivé(s)"
            Case 160790
                Return "Confirmer la suppression des éléments sélectionnés ?"
            Case 160791
                Return "Elément(s) sélectionné(s) ont été supprimé(s)"
            Case 160801
                Return "Vous ne pouvez fusionner que des recettes identiques"
            Case 160802
                Return "Confirmer la fusion des éléments sélectionnés ?"
            Case 160803
                Return "Confirmer la suppression des éléments ?"
            Case 160804
                Return "Remplir les champs obligatoires SVP"
            Case 160805
                Return "Sélectionnez les éléments à fusionner"
            Case 160806
                Return "Confirmer la désactivation des éléments sélectionnés ?"
            Case 160863
                Return "Liste de prix des ingrédients"
            Case 160880
                Return "Recalculer"
            Case 160894
                Return ""
            Case 160940
                Return "Date d'entrée en effet"
            Case 160941
                Return "Article de vente lié"
            Case 160953
                Return "Facteur comparatif Groupe de prix de vente/Groupe prix d'achat"
            Case 160958
                Return "Travailler avec l'article avec de multiples groupes de prix de vente."
            Case 160985
                Return "Articles de vente non-liés"
            Case 160987
                Return "Création des articles de vente et lien avec recettes existantes."
            Case 160988
                Return "Article de vente utilisé à la vente et lié généralement avec une recette"
            Case 161028
                Return "Etes-vous certain(e) de vouloir modifier la base de données d'éléments nutritifs ? Cette action modifiera les définitions des éléments nutritifs que vous avez déjà fixées pour votre marchandise."
            Case 161029
                Return "La case à cocher Rendements ou Ingrédients doit être sélectionnée (cochée)."
            Case 161049
                Return "Forcer l'effacement du mot-clé et des mots-clés liés"
            Case 161050
                Return "Les mots clés supprimés seront aussi désassignés pour les articles des ingrédients/recettes/menus."
            Case 161051
                Return "Les mots clés sélectionnés et tous leurs sous-mots clés ont été supprimés avec succès. Les mots clés supprimés ont maintenant aussi été désassignés pour les articles des ingrédients, des recettes et des menus."
            Case 161078
                Return "Correspondance exacte"
            Case 161079
                Return "Commence par"
            Case 161080
                Return "Contient"
            Case 161082
                Return "Deuxième"
            Case 161083
                Return "Troisième"
            Case 161084
                Return "Quatrième"
            Case 161085
                Return "Une seule fois"
            Case 161086
                Return "Quotidien"
            Case 161087
                Return "Hebdomadaire"
            Case 161088
                Return "Mensuel"
            Case 161089
                Return "Quand le fichier change"
            Case 161090
                Return "Quand l'ordianteur démarre"
            Case 161091
                Return "Entrer %s information"
            Case 161092
                Return "Groupe de fournisseur"
            Case 161093
                Return "Information de facturation"
            Case 161094
                Return "Date de début"
            Case 161095
                Return "du mois"
            Case 161096
                Return "Données en erreur sur l'importation des points de vente"
            Case 161097
                Return "Organisez et gérez les informations liées à vos fournisseurs incluant les contacts des sociétés, les adresses, les termes de paiement, etc. pour faciliter le processus de commande."
            Case 161098
                Return "Le terme ""terminal"" fait référence aux stations de travail de votre système POS qui sont reliées à votre application CALCMENU Web. Vous pouvez ajouter, modifier ou supprimer des terminaux dans ce programme."
            Case 161099
                Return "Configurer les paramètres d'importation du système POS. Fixer le calendrier, les emplacements des fichiers d'importation, etc."
            Case 161100
                Return "Les produits et les articles en stock sont conservés et déplacés dans différents lieux à des moments différents. Gardez le contrôle en établissant dans quels lieux possibles vos produits peuvent être trouvés à tout moment donné."
            Case 161101
                Return "Les clients sont des sociétés et entreprises qui achètent vos produits ou produits finis. Ce programme vous permet de gérer votre liste de clients."
            Case 161102
                Return "Les contacts ""client"" sont des personnes avec qui vous traitez dans une société. Vous pouvez créer, modifier et supprimer des contacts ""client""."
            Case 161103
                Return "Fixe les données POS qui ne sont pas importées avec succès dans le système."
            Case 161104
                Return "Ceci fait référence au type de transaction d'émission pour les produits fournis. Ils peuvent avoir été vendus à des clients ou ne pas avoir été vendus à des clients comme dans le cas d'avantages offerts à des employés ou de distributions gratuites."
            Case 161105
                Return "L'Historique des ventes affiche rapidement une liste des transactions de vente et des articles impliqués."
            Case 161106
                Return "Eléments marqués"
            Case 161107
                Return "Portion calculé"
            Case 161132
                Return "Voir mes recettes"
            Case 161147
                Return "Gestion de recettes et menus"
            Case 161162
                Return "TCPOS"
            Case 161180
                Return "Définir la configuration automatique du téléchargement"
            Case 161181
                Return "Nom du serveur"
            Case 161275
                Return "Repères Nutritionnels Journaliers"
            Case 161276
                Return "RNJ"
            Case 161279
                Return "Sans"
            Case 161281
                Return "Chef principal"
            Case 161282
                Return "Administrateur niveau établissement"
            Case 161283
                Return "Administrateur système"
            Case 161284
                Return "Chef niveau Entreprise"
            Case 161285
                Return "Chef d'un établissement"
            Case 161286
                Return "Chef"
            Case 161287
                Return "Invité"
            Case 161288
                Return "Chef niveau site"
            Case 161289
                Return "Administrateur niveau site"
            Case 161290
                Return "Voir et imprimer"
            Case 161291
                Return "Pas défini"
            Case 161292
                Return "Défini"
            Case 161294
                Return "Articles indésirables"
            Case 161300
                Return "Catalogue de Prix d'Achat"
            Case 161333
                Return "Libellés"
            Case 161334
                Return "Recettes %x-%y sur %z"
            Case 161468
                Return "Valider tout"
            Case 161484
                Return "Température"
            Case 161485
                Return "Production" & vbCrLf & "Date"
            Case 161486
                Return "Consommation" & vbCrLf & "Date"
            Case 161487
                Return "Produits journaliers"
            Case 161488
                Return "a consommer jusqu'au"
            Case 161489
                Return "Frisch zubereitet - frisch geniessen"
            Case 161490
                Return "Info Allergies; contient:"
            Case 161491
                Return "Action sur les éléments marqués"
            Case 161494
                Return "conserver à max. 5°C"
            Case 161538
                Return "Veuillez fournir les informations demandées ci-dessous."
            Case 161554
                Return "Veuillez fournir les informations demandées ci-dessous."
            Case 161576
                Return "Prix unitaire"
            Case 161577
                Return "Heure"
            Case 161578
                Return "Coût total march."
            Case 161579
                Return "calcul"
            Case 161580
                Return "Coût march."
            Case 161581
                Return "Taxe"
            Case 161582
                Return "Marge brute en Fr."
            Case 161583
                Return "Marge brute en %"
            Case 161584
                Return "Unité"
            Case 161585
                Return "Prix/" & vbCrLf & "Unité"
            Case 161710
                Return "Modèle"
            Case 161766
                Return "Petite portion"
            Case 161767
                Return "Grande portion"
            Case 161777
                Return "Mot-clé non assigné"
            Case 161778
                Return "Lié/délié mot-clés"
            Case 161779
                Return "Historique"
            Case 161780
                Return "Visualiser l'historique"
            Case 161781
                Return "Mots-clés non voulus"
            Case 161782
                Return "Imprimer les Etiquettes"
            Case 161783
                Return "Modèle de procédure"
            Case 161784
                Return "Etudiant"
            Case 161785
                Return "Valeurs nutritives des ingrédients pour %s"
            Case 161786
                Return "Valeurs nutritives des ingrédients pour 100g/ml"
            Case 161787
                Return "Appliquer le modèle"
            Case 161788
                Return "Mots-clés Assignés/Dérivés"
            Case 161823
                Return "Ajouter une ligne"
            Case 161824
                Return "Coller depuis le Presse-papier"
            Case 161825
                Return "Il n'y a pas de marchandise qui doit être lié."
            Case 161826
                Return "Choisir un autre"
            Case 161827
                Return "Prix/unité par défaut:"
            Case 161828
                Return "Choisir parmis les unités existantes"
            Case 161829
                Return "Ajouter ceci comme une nouvelle unité"
            Case 161830
                Return "Article validé"
            Case 161831
                Return "Laissez moi éditer la marchandise avant de l'ajouter"
            Case 161832
                Return "mettre %s comme complément"
            Case 161834
                Return "Veuillez controller les prix"
            Case 161835
                Return "Couper"
            Case 161837
                Return "Ajouter à la recette"
            Case 161838
                Return "Remplacer des ingrédients existants"
            Case 161839
                Return "Pas d'ingrédient trouvé"
            Case 161840
                Return ""
            Case 161841
                Return "Lien à des ingrédients/sous-recettes"
            Case 161842
                Return "Tous les éléments sont maintenant liés à des ingrédients/sous-recettes"
            Case 161843
                Return "L'élément est maintenant lié à une marchandise/sous-recette"
            Case 161844
                Return "Temps de stockage"
            Case 161845
                Return "Temp. de stockage"
            Case 161851
                Return "Peut être commandé"
            Case 161852
                Return "La recette peut contenir des allergènes"
            Case 161853
                Return "Coller"
            Case 161855
                Return "Brouillons"
            Case 161873
                Return "Deconnexion"
            Case 161899
                Return "Soumis par"
            Case 161902
                Return "Ajouter un commentaire"
            Case 161955
                Return "Le nom de votre ami(e)"
            Case 161956
                Return "L'émail de votre ami(e)"
            Case 161970
                Return "Pas de commentaire pour cette recette. Soyez le/la premier(e) à donner un commentaire."
            Case 161986
                Return "Ajouter une étape"
            Case 161987
                Return "Elément %n de %p"
            Case 161988
                Return "Produits liés"
            Case 161989
                Return "Produits pas liés"
            Case 162032
                Return "Votre émail a été envoyé à votre ami(e)"
            Case 162039
                Return "%p utilisateurs ont ajoutés cette recette à leurs favorites"
            Case 162054
                Return "Evaluation"

            Case 162057
                Return "Le champ %c  ne peut pas être vide"
            Case 162061
                Return "sur"
            Case 162062
                Return "Recette sur"
            Case 162102
                Return "Evaluation de %p (%r évaluations)"
            Case 162198
                Return "Le rendement a été changé. Cliquez sur le bouton Calculer pour changer les quantités d'ingrédients."
            Case 162199
                Return "Le rendement a été changé. Voulez-vous continuer et enregistrer le fichier sans calculer les quantités d'ingrédients?"
            Case 162203
                Return "Information"
            Case 162205
                Return "Nombre de propositions"
            Case 162208
                Return "Jours d'ouverture hebdomadaire"
            Case 162211
                Return "Langue choisie"
            Case 162212
                Return "Nom de l'établissement"
            Case 162213
                Return "Numéro de l'établissement"
            Case 162214
                Return "Prix disponibles"
            Case 162215
                Return "Mettre le logo sur le serveur"
            Case 162216
                Return "Configuration"
            Case 162219
                Return "Arrière-boutique"
            Case 162221
                Return "Configuration générale"
            Case 162222
                Return "Insérer ici"
            Case 162230
                Return "Entrez les informations liées au style"
            Case 162231
                Return "Nom du style"
            Case 162232
                Return "Options style en-tête"
            Case 162235
                Return "Vouliez-vous dire"
            Case 162257
                Return "Date de la dernière modification"
            Case 162276
                Return "Importer des recettes/menus"
            Case 162282
                Return "Notes"
            Case 162314
                Return "Producteur"
            Case 162318
                Return "Alcool"
            Case 162319
                Return "Millésime"
            Case 162338
                Return "Type de vin"
            Case 162340
                Return "Street"
            Case 162341
                Return "Lieu"
            Case 162357
                Return "Exemple"
            Case 162358
                Return "Conserver la longueur du préfixe"
            Case 162361
                Return "Onglet"
            Case 162362
                Return "Tuyau"
            Case 162363
                Return "Point-virgule"
            Case 162364
                Return "Espace"
            Case 162382
                Return "Approbation"
            Case 162383
                Return "Approbation"
            Case 162386
                Return "Démarrer"
            Case 162387
                Return "Bonjour Approver,Vous avez reçu une recette qui doit être approuvée. […] vous soumets cette recette :Veuillez vous connectez à CALCMENU Web pour réviser et approuver la recette.Cordialement,L'équipe EGS"
            Case 162388
                Return "Bonjour,Votre nouvelle recette a été envoyée pour approbation. Vous avez soumis cette recette: [...]Une fois approuvé, la recette sera disponible sur le site.Cordialement,L'équipe EGS"
            Case 162389
                Return "Bonjour,Vous avez approuvé cette recette: [...]La recette sera disponible sur le site.Cordialement,L'équipe EGS"
            Case 162390
                Return "Bonjour, La recette […] a été approuvée. Vous pouvez maintenant utiliser cette recette sur le site.Cordialement,L'équipe EGS"
            Case 162455
                Return "Login"
            Case 162485
                Return ""
            Case 162530
                Return "Supprimer les ""breadcrumbs"" lors de la connexion (login)"
            Case 162596
                Return ""
            Case 162631
                Return ""
            Case 162632
                Return ""
            Case 162635
                Return ""
            Case 162636
                Return ""
            Case 162637
                Return ""
            Case 162638
                Return ""
            Case 162742
                Return "Bien"
            Case 162747
                Return ""
            Case 162888
                Return ""
            Case 162955
                Return "Marge nette en %"
            Case 163032
                Return ""
            Case 163046
                Return ""
            Case 163057
                Return "Coût total %s"
            Case 163058
                Return "Coût pour 1 %s"
            Case 163060
                Return "Coût en %s"
            Case 163061
                Return "Coût effectif en %s"
            Case 167272
                Return "Détails du produit"
            Case 167346
                Return "Afficher tout"
            Case 167385
                Return "Sous-titre"
            Case 167469
                Return "Nota bene"
            Case 167719
                Return "Budget"
            Case 168373
                Return "Utilisé 'en ligne'"
            Case 168374
                Return ""
            Case 168375
                Return ""
            Case 169310
                Return ""
            Case 169318
                Return "Feedback"
            Case 170155
                Return "Assignez des ingrédients, des recettes et des menus à des catégories, des mots-clés et des sources (cela peut être un livre de cuisine, un site internet, un chef, etc.). Cela vous permet de grouper et d'organiser vos articles dans EGS CALCMENU Web.  Rechercher une marchandise, une recette ou un menu peut se faire plus rapidement et plus facilement car les catégories, les mots-clés et les sources sont très utiles dans l'affinage de vos résultats de recherche."
            Case 170253
                Return "Afficher le PDF"
            Case 170283
                Return ""
            Case 170668
                Return "Nous vous prions d'agréer, Monsieur/Madame, nos respectueuses salutations."
            Case 170674
                Return ""
            Case 170675
                Return ""
            Case 170770
                Return "Nombre de portions à imprimer"
            Case 170779
                Return "Liste des ingrédients"
            Case 170780
                Return "Les détails de l'ingrédient"
            Case 170781
                Return "Liste des nutriments d'ingrédient"
            Case 170782
                Return "Catégorie d'ingrédients"
            Case 170783
                Return "Mot clé d'ingrédient"
            Case 170784
                Return "Ingrédient publié sur le Web"
            Case 170785
                Return "Ingrédient non publié sur le Web"
            Case 170786
                Return "Coût de l'ingrédient"
            Case 170801
                Return ""
            Case 170849
                Return ""
            Case 170850
                Return ""
            Case 170851
                Return ""
            Case 170852
                Return ""
            Case 170853
                Return ""
            Case 170854
                Return ""
            Case 170855
                Return ""
            Case 170856
                Return ""
            Case 170857
                Return ""
            Case 170858
                Return ""
            Case 170859
                Return ""
            Case 170860
                Return "Changer les éléments marqués vers un nouveau standard"
            Case 171014
                Return ""
            Case 171219
                Return ""
            Case 171220
                Return "Nombre de portions"
            Case 171221
                Return "Rendement total"
            Case 171231
                Return ""
            Case 171232
                Return ""
            Case 171233
                Return ""
            Case 171234
                Return ""
            Case 171235
                Return ""
            Case 171236
                Return ""
            Case 171237
                Return ""
            Case 171238
                Return ""
            Case 171240
                Return ""
            Case 171241
                Return ""
            Case 171242
                Return ""
            Case 171243
                Return ""
            Case 171244
                Return ""
            Case 171245
                Return ""
            Case 171246
                Return ""
            Case 171249
                Return "%s existe déjà."
            Case 171301
                Return "Méthode de préparation"
            Case 171302
                Return "Astuces"
            Case 171345
                Return "Tous les plats"
            Case 171346
                Return "Toute l'année"
            Case 171347
                Return "Plats proposés"
            Case 171348
                Return "Plat"
            Case 171352
                Return "Nom d'utilisateur/adresse email non valide"
            Case 171353
                Return "Pour récupérer votre mot de passe, saisissez votre nom d'utilisateur ou votre adresse e-mail"
            Case 171354
                Return "Saisissez votre nom d'utilisateur ou votre adresse e-mail"
            Case 171371
                Return "Afficher plus de détails"
            Case 171372
                Return "Afficher moins"
            Case 171373
                Return "Veuillez d'abord enregistrer le Recette."
            Case 171399
                Return "Kiosque pour %CM"
            Case 171401
                Return "Les recettes visibles sur ce Kiosque ont été créées par %CM."
            Case 171402
                Return "Partager cette recette sur %p"
            Case 171425
                Return "Développé et géré par"
            Case 171428
                Return "Paramètre non valide. Contactez l'expéditeur de la recette ou l'Equipe support du Cloud CALCMENU."
            Case 171429
                Return "Le lien vers cette recette / ce groupe de recette a expiré. Contactez l'expéditeur de la recette ou l'Equipe support du Cloud CALCMENU."
            Case 171447
                Return "Votre e-mail/smtp n'a pas été configuré. Configurez votre e-mail dans le menu Configurations pour utiliser cette fonctionnalité."
            Case 171453
                Return "impossible d'envoyer l'e-mail."
            Case 171501
                Return "Si vous ne le connaissez pas, veuillez nous envoyer par e-mail votre numéro de série CALCMENU et  votre entête."
            Case 171502
                Return ""
            Case 171505
                Return "Cette recette est encodée dans CALCMENU. Visitez calcmenu.com pour en savoir plus."
            Case 171506
                Return ""
            Case 171507
                Return ""
            Case 171555
                Return ""
            Case 171557
                Return ""
            Case 171558
                Return ""
            Case 171559
                Return "RECIPECENTER est une vaste collection de recettes du monde entier - de l'amateur aux  chefsprofessionnels et les membres du site qui peuvent évaluer les recettes en ligne ."
            Case 171560
                Return ""
            Case 171561
                Return ""
            Case 171586
                Return ""
            Case 171588
                Return "Forum Culinaire Cours"
            Case 171589
                Return "Gestion des ingrédients des fournisseurs %c"
            Case 171591
                Return ""
            Case 171592
                Return "Nom du fournisseur"
            Case 171593
                Return "Code fournisseur"
            Case 171594
                Return ""
            Case 171595
                Return "Les recettes sur ce site sont gérées par %Cmcloud, un outil de gestion et d'édition de recettes avancée pour les professionnels de l'alimentation et des éditeurs de recettes."

            Case 171596
                Return ""
            Case 171597
                Return ""
            Case 171598
                Return "Oui, je souhaite recevoir des information au sujet du CALCMENU Cloud, dès qu'il sera disponible."
            Case 171599
                Return "Ayant le souci permanent de développer des logiciels de gestion de recettes de pointe, nous vous présentons le nouveau Recipecenter amélioré. Le site offre une interface simple et facile pour vérifier des recettes en ligne et les partager avec  vos amis sur Facebook et Twitter en utilisant votre téléphone mobile, iPhone ou iPad, Blackberry et autres appareils."
            Case 171600
                Return "Voir,  ""favoriser"", évaluer et poster des commentaires sur les recettes. Si vous êtes déjà inscrit dans recipecenter.com, vous pouvez vous connecter en utilisant les mêmes détails de compte, et vos recettes codées et d'autres informations ne seront pas perdues. D'autre part, les utilisateurs auront bientôt une solution de gestion de recettes plus avancée pour coder, partager et accéder à une vaste collection de recettes de nouveaux participants  - grâce à l'intégration avec le logiciel de gestion de recettes - CALCMENU Cloud ."

            Case 171601
                Return "Veuillez inviter vos amis à rejoindre notre communauté. Nous espérons que vous apprécierez votre visite sur notre site et y reviendrez bientôt."
            Case 171602
                Return "Les recettes de ce site web sont encodées et gérées par le logiciel de gestion de recettes: Cloud CALCMENU."
            Case 171605
                Return ""
            Case 171611
                Return "Favoriser"
            Case 171612
                Return "Pas bien"
            Case 171614
                Return "Envoyer une recette à un ami:"
            Case 171615
                Return "Se connecter avec nous"
            Case 171616
                Return ""
            Case 171617
                Return ""
            Case 171618
                Return ""
            Case 171619
                Return ""
            Case 171620
                Return ""
            Case 171621
                Return "Pour récupérer votre mot de passe, veuillez introduire l'adresse é-mail de votre compte ci-dessous."
            Case 171622
                Return "NR. Article %c"
            Case 171628
                Return "Recettes par nos contributeurs"
            Case 171631
                Return ""
            Case 171649
                Return "No de lot"
            Case 171650
                Return ""
            Case 171651
                Return ""
            Case 171652
                Return ""
            Case 171653
                Return ""
            Case 171654
                Return ""
            Case 171655
                Return ""
            Case 171656
                Return ""
            Case 171657
                Return ""
            Case 171658
                Return ""
            Case 171662
                Return ""
            Case 171663
                Return ""
            Case 171664
                Return ""
            Case 171665
                Return ""
            Case 171666
                Return ""
            Case 171667
                Return ""
            Case 171668
                Return ""
            Case 171669
                Return ""
            Case 171670
                Return "Marque Réputée"
            Case 171671
                Return ""
            Case 171672
                Return ""
            Case 171673
                Return ""
            Case 171674
                Return ""
            Case 171675
                Return ""
            Case 171676
                Return ""
            Case 171677
                Return ""
            Case 171678
                Return ""
            Case 171679
                Return ""
            Case 171680
                Return ""
            Case 171681
                Return ""
            Case 171682
                Return ""
            Case 171683
                Return ""
            Case 171684
                Return ""
            Case 171685
                Return ""
            Case 171686
                Return ""
            Case 171687
                Return ""
            Case 171688
                Return ""
            Case 171689
                Return ""
            Case 171690
                Return ""
            Case 171691
                Return ""
            Case 171692
                Return ""
            Case 171693
                Return ""
            Case 171694
                Return ""
            Case 171696
                Return ""
            Case 171697
                Return ""
            Case 171698
                Return ""
            Case 171699
                Return ""
            Case 171700
                Return ""
            Case 171701
                Return ""
            Case 171702
                Return ""
            Case 171703
                Return ""
            Case 171704
                Return ""
            Case 171705
                Return ""
            Case 171706
                Return ""
            Case 171707
                Return ""
            Case 171708
                Return ""
            Case 171709
                Return ""
            Case 171710
                Return ""
            Case 171711
                Return ""
            Case 171712
                Return ""
            Case 171713
                Return ""
            Case 171714
                Return ""
            Case 171715
                Return ""
            Case 171716
                Return ""
            Case 171717
                Return ""
            Case 171718
                Return ""
            Case 171719
                Return ""
            Case 171720
                Return ""
            Case 171721
                Return ""
            Case 171722
                Return ""
            Case 171723
                Return ""
            Case 171724
                Return ""
            Case 171725
                Return ""
            Case 171726
                Return ""
            Case 171727
                Return ""
            Case 171728
                Return ""
            Case 171729
                Return ""
            Case 171730
                Return ""
            Case 171731
                Return ""
            Case 171732
                Return ""
            Case 171733
                Return ""
            Case 171734
                Return ""
            Case 171735
                Return ""
            Case 171736
                Return ""
            Case 171737
                Return ""
            Case 171738
                Return ""
            Case 171739
                Return ""
            Case 171740
                Return ""
            Case 171741
                Return ""
            Case 171742
                Return ""
            Case 171743
                Return ""
            Case 171744
                Return ""
            Case 171745
                Return ""
            Case 171746
                Return ""
            Case 171747
                Return ""
            Case 171748
                Return ""
            Case 171749
                Return ""
            Case 171750
                Return ""
            Case 171751
                Return ""
            Case 171752
                Return ""
            Case 171753
                Return ""
            Case 171754
                Return ""
            Case 171755
                Return ""
            Case 171756
                Return ""
            Case 171758
                Return ""
            Case 171759
                Return ""
            Case 171760
                Return ""
            Case 171761
                Return ""
            Case 171762
                Return ""
            Case 171763
                Return ""
            Case 171764
                Return ""
            Case 171765
                Return ""
            Case 171767
                Return ""
            Case 171768
                Return ""
            Case 171769
                Return ""
            Case 171770
                Return ""
            Case 171771
                Return ""
            Case 171772
                Return ""
            Case 171773
                Return ""
            Case 171774
                Return ""
            Case 171775
                Return ""
            Case 171776
                Return ""
            Case 171777
                Return ""
            Case 171778
                Return ""
            Case 171779
                Return ""
            Case 171780
                Return ""
            Case 171781
                Return ""
            Case 171782
                Return ""
            Case 171783
                Return ""
            Case 171785
                Return ""
            Case 171786
                Return ""
            Case 176055
                Return "Veuillez sélectionner au moins une loi sur les aliments"
        End Select
    End Function

 
'italian

    Public Function FTBLow4USA(ByVal Code As Integer) As String
        Select Case Code
            Case 1080
                Return "Costo delle merci"
            Case 1081
                Return "Costo delle merci"
            Case 1090
                Return "Prezzo di vendita"
            Case 1145
                Return "Contatore"
            Case 1146
                Return "In corso"
            Case 1260
                Return "Ingredienti(i)"
            Case 1280
                Return "Commento"
            Case 1290
                Return "Prezzo"
            Case 1300
                Return "Scarti"
            Case 1310
                Return "Quantità"
            Case 1400
                Return "Menú"
            Case 1450
                Return "Categoria"
            Case 1480
                Return "Prezzo stabilito"
            Case 1485
                Return "Prezzo calcolato"
            Case 1500
                Return "Data"
            Case 1530
                Return "Manca l' unità"
            Case 1600
                Return "Modifica il Menú"
            Case 2430
                Return "Scegli dall' elenco"
            Case 2700
                Return "Stampa l' elenco dei menú"
            Case 2780
                Return "Elenco acquisti"
            Case 3057
                Return "Database"
            Case 3140
                Return "Per"
            Case 3150
                Return "Percentuale"
            Case 3161
                Return "Fattore"
            Case 3195
                Return "Ricetta nu."
            Case 3200
                Return "Cuoco"
            Case 3204
                Return "Nome"
            Case 3205
                Return "Nome"
            Case 3206
                Return "Traduzione"
            Case 3215
                Return "Prezzo Unità"
            Case 3230
                Return "Foto"
            Case 3234
                Return "Lista"
            Case 3300
                Return "Carta del menú"
            Case 3305
                Return "Nome di riferimento"
            Case 3306
                Return "Rappresentante"
            Case 3320
                Return "Vuoi adattare le quantità al nuovo numero di persone ?"
            Case 3460
                Return "Password"
            Case 3680
                Return "Backup"
            Case 3685
                Return "Backup eseguito"
            Case 3721
                Return "Fonte"
            Case 3760
                Return "Importazione dati"
            Case 3800
                Return "Esportazione dati"
            Case 4130
                Return "Spazio libero nel disco"
            Case 4185
                Return "Identificazione del prodotto"
            Case 4755
                Return "Importa"
            Case 4825
                Return "Ricette"
            Case 4832
                Return "Ricetta"
            Case 4834
                Return "Ingredienti della ricetta"
            Case 4854
                Return "Minimo"
            Case 4855
                Return "Massimo"
            Case 4856
                Return "Da"
            Case 4860
                Return "Nome del file"
            Case 4862
                Return "Versione"
            Case 4865
                Return "Utenti"
            Case 4867
                Return "Modifica"
            Case 4870
                Return "Modifica un' utente"
            Case 4877
                Return "Media"
            Case 4890
                Return "Tipo di file"
            Case 4891
                Return "Anteprima"
            Case 5100
                Return "Unità"
            Case 5105
                Return "Formato"
            Case 5270
                Return "Lista delle merci"
            Case 5350
                Return "Totale"
            Case 5390
                Return "persone"
            Case 5500
                Return "Numero"
            Case 5530
                Return "Prezzo di vendita stabilito"
            Case 5590
                Return "Ingredienti"
            Case 5600
                Return "Preparazione"
            Case 5610
                Return "Pagina"
            Case 5720
                Return "Importo"
            Case 5741
                Return "Lorda"
            Case 5795
                Return "per persona"
            Case 5801
                Return "Profitto"
            Case 5900
                Return "Categorie delle merci"
            Case 6000
                Return "Modifica la categoria"
            Case 6002
                Return "Nome della categoria"
            Case 6055
                Return "Aggiungi del testo"
            Case 6390
                Return "Valuta"
            Case 6416
                Return "Fattore"
            Case 6470
                Return "Attendere per favore"
            Case 7010
                Return "No"
            Case 7030
                Return "Stampante"
            Case 7073
                Return "Scegli"
            Case 7181
                Return "Tutti"
            Case 7183
                Return "Selezionato"
            Case 7250
                Return "Francese"
            Case 7260
                Return "Tedesco"
            Case 7270
                Return "Inglese"
            Case 7280
                Return "Italiano"
            Case 7292
                Return "Giapponese"
            Case 7296
                Return "Europa"
            Case 7335
                Return "Tutte le selezioni sono state rimosse con successo"
            Case 7570
                Return "Domenica"
            Case 7571
                Return "Lunedì"
            Case 7572
                Return "Martedi"
            Case 7573
                Return "Mercoledi"
            Case 7574
                Return "Giovedi"
            Case 7575
                Return "Venerdi"
            Case 7576
                Return "Sabato"
            Case 7720
                Return "Imballaggio"
            Case 7725
                Return "Trasporto"
            Case 7755
                Return "Sistema"
            Case 8210
                Return "Calcolo"
            Case 8220
                Return "Procedura"
            Case 8395
                Return "Aggiungi"
            Case 8397
                Return "Cancella"
            Case 8514
                Return "Nuovo prezzo"
            Case 8913
                Return "Nessuno"
            Case 8914
                Return "Decimale"
            Case 8990
                Return "o"
            Case 8994
                Return "Utensili"
            Case 9030
                Return "Aggiornamento"
            Case 9070
                Return "Non autorizzato nella versione di dimostrazione"
            Case 9140
                Return "Svizzera"
            Case 9920
                Return "Descrizione"
            Case 10103
                Return "Copia"
            Case 10104
                Return "Testo"
            Case 10109
                Return "Opzioni"
            Case 10116
                Return "Nota"
            Case 10121
                Return "Cerca"
            Case 10125
                Return "Nota"
            Case 10129
                Return "Selezione"
            Case 10130
                Return "In stock"
            Case 10131
                Return "Entrata"
            Case 10132
                Return "Uscita"
            Case 10135
                Return "Stile"
            Case 10140
                Return "Gestione del magazzino"
            Case 10363
                Return "Iva"
            Case 10369
                Return "Numero fornitore"
            Case 10370
                Return "In ordinazione"
            Case 10399
                Return "Cancellato"
            Case 10417
                Return "Fallito :"
            Case 10430
                Return "Luogo di stoccaggio"
            Case 10431
                Return "Inventario"
            Case 10447
                Return "Ordine"
            Case 10468
                Return "Stato"
            Case 10513
                Return "Sconto"
            Case 10523
                Return "Tel."
            Case 10524
                Return "Fax"
            Case 10554
                Return "Descrizione CCP"
            Case 10555
                Return "Tempo di raffreddamento"
            Case 10556
                Return "Tempo di cottura"
            Case 10557
                Return "Temperatura di cottura"
            Case 10558
                Return "Modo di cottura"
            Case 10572
                Return "Nutrienti"
            Case 10573
                Return "Info1"
            Case 10970
                Return "Stampa"
            Case 10990
                Return "Fornitori"
            Case 11040
                Return "Ripristino eseguito"
            Case 11060
                Return "Percorso"
            Case 11280
                Return "Registrazione"
            Case 12515
                Return "Barcode"
            Case 12525
                Return "Data non valida"
            Case 13060
                Return "Nutrienti"
            Case 13065
                Return "Visualizza i nutrienti"
            Case 13255
                Return "Chi siamo"
            Case 14070
                Return "Carattere"
            Case 14090
                Return "Titolo"
            Case 14110
                Return "Nota"
            Case 14816
                Return "Sostituire con"
            Case 14819
                Return "Sostituisci"
            Case 14884
                Return "Articoli aggiornati"
            Case 15360
                Return "Menú selezionati"
            Case 15504
                Return "L' amminstratore"
            Case 15510
                Return "Password"
            Case 15615
                Return "Inserisci la password"
            Case 15620
                Return "Conferma"
            Case 16010
                Return "Calcolo"
            Case 18460
                Return "Salvataggio in corso"
            Case 19330
                Return "Punti"
            Case 20122
                Return "Ditta"
            Case 20200
                Return "Sottoricette"
            Case 20469
                Return "Specifica la modalità di spedizione"
            Case 20530
                Return "Energia"
            Case 20703
                Return "Principale"
            Case 20709
                Return "Unità"
            Case 21550
                Return "Nessun piatto trovato"
            Case 21570
                Return "Stampa un modulo  fax"
            Case 21600
                Return "di"
            Case 24002
                Return "L' ultimo ordine"
            Case 24011
                Return "di"
            Case 24016
                Return "Fornitore"
            Case 24027
                Return "Calcolo"
            Case 24028
                Return "Esci"
            Case 24044
                Return "Ambedue"
            Case 24050
                Return "Nuovo"
            Case 24068
                Return "Margine"
            Case 24075
                Return "Numero dell' articolo"
            Case 24085
                Return "Assegna un nuovo"
            Case 24087
                Return "Nessuna ingredienti trovata"
            Case 24105
                Return "Visualizza"
            Case 24121
                Return "Abbreviazi."
            Case 24129
                Return "Trasferimenti"
            Case 24150
                Return "Modifica"
            Case 24152
                Return "Posizione"
            Case 24153
                Return "Città"
            Case 24163
                Return "Luogo predefinito"
            Case 24260
                Return "Non si puó cancellare questo fornitore"
            Case 24268
                Return "Deselez. Tutti"
            Case 24269
                Return "Seleziona tutti"
            Case 24270
                Return "Precedente"
            Case 24271
                Return "Avanti"
            Case 24291
                Return "Sub Totale"
            Case 26000
                Return "proseguire"
            Case 26100
                Return "Descrizione del prodotto"
            Case 26101
                Return "Consigli per la cottura"
            Case 26102
                Return "Varianti"
            Case 26103
                Return "Immagazzinamento"
            Case 26104
                Return "Resa"
            Case 27000
                Return "Nome di rif."
            Case 27020
                Return "Indirizzo"
            Case 27050
                Return "Nu. di telefono"
            Case 27055
                Return "Intestazione"
            Case 27056
                Return "e"
            Case 27130
                Return "Pagamento"
            Case 27135
                Return "Data di scadenza"
            Case 27220
                Return "Ora"
            Case 27530
                Return "Tasso"
            Case 28000
                Return "Errore nell' operazione"
            Case 28008
                Return "Directory non valida"
            Case 28420
                Return "Foto non disponibile"
            Case 28483
                Return "Questo record non esiste"
            Case 28655
                Return "Nessuna unità definita"
            Case 29170
                Return "Non disponibile"
            Case 29771
                Return "Modificare le merci"
            Case 30210
                Return "Operazione fallita"
            Case 30240
                Return "Codice"
            Case 30270
                Return "introvabile"
            Case 31085
                Return "Aggiornato con successo"
            Case 31098
                Return "Salvare"
            Case 31370
                Return "Costo delle merci"
            Case 31375
                Return "FC"
            Case 31380
                Return "Principale"
            Case 31462
                Return "Errore"
            Case 31492
                Return "La nostra assistenza fax assicura una risposta entro  24 ore, dipende dal problema riscontrato (eccetto i fine settimana)"
            Case 31700
                Return "Giorni"
            Case 31732
                Return "Piano dei menú"
            Case 31755
                Return "Risultati"
            Case 31758
                Return "A"
            Case 31769
                Return "Menu venduti"
            Case 31800
                Return "Giorno"
            Case 31860
                Return "Periodo"
            Case 51056
                Return "Prodotto"
            Case 51086
                Return "Lingua"
            Case 51092
                Return "Unità"
            Case 51097
                Return "EGS Enggist && Grandjean Software SA"
            Case 51098
                Return "Pierre-à-Bot 92"
            Case 51099
                Return "2000 Neuchâtel, Svizzera"
            Case 51123
                Return "Dettagli"
            Case 51128
                Return "Nome della ricetta"
            Case 51129
                Return "Ingredienti desiderati"
            Case 51130
                Return "Ingredienti indesiderati"
            Case 51131
                Return "Nome della categoria"
            Case 51139
                Return "Desiderato"
            Case 51157
                Return "Messaggio"
            Case 51174
                Return "Importazione terminata"
            Case 51178
                Return "Riprovare per favore"
            Case 51198
                Return "Connessione al server SMTP"
            Case 51204
                Return "Si"
            Case 51243
                Return "Margini"
            Case 51244
                Return "Alto"
            Case 51245
                Return "Inferiore"
            Case 51246
                Return "Sinistro"
            Case 51247
                Return "Destra"
            Case 51252
                Return "Scarica la Brochure"
            Case 51257
                Return "E-mail"
            Case 51259
                Return "SMTP Server"
            Case 51261
                Return "Nome Utente"
            Case 51281
                Return "Ingredienti per"
            Case 51294
                Return "Per"
            Case 51311
                Return "Unità non valida"
            Case 51323
                Return "valore non valido per la resa"
            Case 51336
                Return "Non desiderato"
            Case 51337
                Return "Principale"
            Case 51353
                Return "Accordo Copyright"
            Case 51364
                Return "Accetti l'accordo di copyright sopra e vuoi procedere per l'invio della ricetta?"
            Case 51373
                Return "Inserite tutte le informazioni riguardanti SMTP, POP, nome utente e password"
            Case 51377
                Return "Invia email"
            Case 51392
                Return "Porzioni"
            Case 51402
                Return "Sei sicuro di voler cancellare"
            Case 51500
                Return "Dettagli della lista della spesa"
            Case 51502
                Return "Lista della Spesa"
            Case 51532
                Return "Stampa la lista della spesa"
            Case 51907
                Return "&Mostra dettagli"
            Case 52012
                Return "Scegli foto"
            Case 52110
                Return "I file selezionati saranno importati"
            Case 52130
                Return "Nuova ricetta"
            Case 52150
                Return "Finito"
            Case 52307
                Return "Chiudi"
            Case 52960
                Return "Semplice"
            Case 52970
                Return "Completo"
            Case 53250
                Return "Esporta le selezioni"
            Case 54210
                Return "Non cambiare niente"
            Case 54220
                Return "Tutto in maiuscolo"
            Case 54230
                Return "Tutto in minuscolo"
            Case 54240
                Return "In maiuscolo la prima lettera di ogni parola"
            Case 54245
                Return "La prima lettera in maiuscolo"
            Case 54295
                Return "Con"
            Case 54710
                Return "Parole chiave selezionate"
            Case 54730
                Return "Parola chiave"
            Case 55011
                Return "Porzione"
            Case 55211
                Return "Collegamento"
            Case 55220
                Return "Qtà"
            Case 56100
                Return "Il vostro nome"
            Case 56130
                Return "Nazione"
            Case 56500
                Return "Dizionario"
            Case 101600
                Return "Modifica il Menú"
            Case 103150
                Return "Percentuale"
            Case 103215
                Return "Prezzo unità"
            Case 103305
                Return "Nome di riferimento"
            Case 103306
                Return "Rappresentante"
            Case 104829
                Return "Lista dei fornitori"
            Case 104835
                Return "Crea un nuovo prodotto"
            Case 104836
                Return "Modifica un' ingrediente"
            Case 104854
                Return "Minimo"
            Case 104855
                Return "Massimo"
            Case 104862
                Return "Versione"
            Case 104869
                Return "Nuovo utente"
            Case 104870
                Return "Modifica un' utente"
            Case 105100
                Return "Unità"
            Case 105110
                Return "Data"
            Case 105200
                Return "Per"
            Case 105360
                Return "Prezzo di vendita per persona"
            Case 106002
                Return "Nome della categoria"
            Case 107183
                Return "Selezionato"
            Case 109730
                Return "con"
            Case 110101
                Return "Modifica"
            Case 110102
                Return "Cancella"
            Case 110112
                Return "Stampa"
            Case 110114
                Return "Aiuto"
            Case 110129
                Return "Selezione"
            Case 110417
                Return "Fallito :"
            Case 110447
                Return "Ordine"
            Case 110524
                Return "Fax"
            Case 113275
                Return "Iva"
            Case 115510
                Return "Password"
            Case 115610
                Return "Nuova password accettata"
            Case 119130
                Return "Cerca"
            Case 121600
                Return "di"
            Case 124016
                Return "Fornitore"
            Case 124024
                Return "Approvato da"
            Case 124042
                Return "Tipo"
            Case 124164
                Return "Aggiustamenti nell' inventario"
            Case 124257
                Return "Punto vendita"
            Case 127010
                Return "Ditta"
            Case 127040
                Return "Nazione"
            Case 127050
                Return "Nu. di telefono"
            Case 127055
                Return "Intestazione"
            Case 128000
                Return "Errore nell'operazione"
            Case 131462
                Return "Errore"
            Case 131700
                Return "Giorni"
            Case 131757
                Return "Da"
            Case 132541
                Return "Ricetta"
            Case 132552
                Return "Totale Iva"
            Case 132553
                Return "Prezzo di vendita imposto + Iva"
            Case 132554
                Return "Modifica Ricetta"
            Case 132555
                Return "Aggiungi Ricetta"
            Case 132557
                Return "Crea un nuovo Menu"
            Case 132559
                Return "Crea una nuova Ingredienti"
            Case 132561
                Return "Si prega di inserire il Numero di Serie, L'Intestazione ed il codice ID del prodotto. Troverete queste informazioni con la documentazione fornita con CALCMENU."
            Case 132565
                Return "Complemento"
            Case 132567
                Return "Categoria delle merci"
            Case 132568
                Return "Categoria delle ricette"
            Case 132569
                Return "Categoria dei menu"
            Case 132570
                Return "Impossibile cancellare."
            Case 132571
                Return "La categoria è in uso."
            Case 132586
                Return "Informazioni sull'account"
            Case 132589
                Return "Numero massimo di ricette"
            Case 132590
                Return "Numero di ricette attuali"
            Case 132592
                Return "Numero massimo di merci"
            Case 132593
                Return "Numero di merci attuali"
            Case 132597
                Return "Creare una nuova ricetta"
            Case 132598
                Return "Numero massimo di menu"
            Case 132599
                Return "Numero di menu attuali"
            Case 132600
                Return "Assegna una parola chiave"
            Case 132601
                Return "Spostare gli elementi selezionati in una nuova categoria"
            Case 132602
                Return "Cancellare gli elementi selezionati"
            Case 132605
                Return "Lista della spesa"
            Case 132607
                Return "Azione sugli elementi selezionati"
            Case 132614
                Return "Qtà netta"
            Case 132615
                Return "Destra"
            Case 132616
                Return "Proprietario"
            Case 132617
                Return "TUTTE LE CATEGORIE"
            Case 132621
                Return "Modificare Fonte"
            Case 132630
                Return "Conversione automatica"
            Case 132638
                Return "Informazioni Utente"
            Case 132640
                Return "Username già in uso."
            Case 132654
                Return "Gestione dei Database"
            Case 132657
                Return "&Ripristino"
            Case 132667
                Return "Fondere"
            Case 132668
                Return "Eliminare"
            Case 132669
                Return "Muovi su"
            Case 132670
                Return "Muovi giù"
            Case 132671
                Return "Standardizzare"
            Case 132672
                Return "Sei sicuro di voler cancellare %n?"
            Case 132678
                Return "HACCP"
            Case 132683
                Return "Precedente"
            Case 132706
                Return "I valori nutrizionali sono per 100g o 100ml."
            Case 132708
                Return "Il fornitore non esiste"
            Case 132714
                Return "Seleziona dalla lista."
            Case 132719
                Return "Il prezzo per questa unità è già definito."
            Case 132723
                Return "Lo scarto totale non deve essere superiore o uguale a 100%"
            Case 132736
                Return "Qtà Lorda"
            Case 132737
                Return "Aggiungere un nuovo Fornitore"
            Case 132738
                Return "Modifica Fornitore"
            Case 132739
                Return "Dettagli del fornitore"
            Case 132740
                Return "Stato"
            Case 132741
                Return "URL"
            Case 132779
                Return "La parola chiave è già in uso."
            Case 132783
                Return "Parola chiave"
            Case 132788
                Return "Collegamento ai nutrienti"
            Case 132789
                Return "&Login"
            Case 132793
                Return "Nome dell'utente o password non valida."
            Case 132813
                Return "&Configurazione"
            Case 132828
                Return "&Ricalcolo dei Nutrienti"
            Case 132841
                Return "Aggiungi Merci"
            Case 132846
                Return "Salva le selezioni"
            Case 132847
                Return "Carica le selezioni"
            Case 132848
                Return "Filtro"
            Case 132855
                Return "Aggiungi Menu"
            Case 132860
                Return "Aggiungi Ingrediente"
            Case 132861
                Return "Modifica Ingrediente"
            Case 132864
                Return "Sostituisci Ingrediente"
            Case 132865
                Return "Aggiungi separatore"
            Case 132877
                Return "Aggiungi articolo"
            Case 132896
                Return "Standardizza Categorie"
            Case 132900
                Return "Aggiungi prezzo"
            Case 132912
                Return "Standardizza Testi"
            Case 132915
                Return "Standardizza Unità"
            Case 132924
                Return "Standardizza le unità delle ricette"
            Case 132930
                Return "Miniature"
            Case 132933
                Return "Lista delle Ricette"
            Case 132934
                Return "Ultima ricetta"
            Case 132937
                Return "Ultimo menu"
            Case 132939
                Return "Lista dei menu"
            Case 132954
                Return "Gruppo di selezioni"
            Case 132955
                Return "Scegli un nome di selezione dalla lista o digita un nuovo nome di selezione da memorizzare"
            Case 132957
                Return "Salva le selezioni come"
            Case 132967
                Return "Nutrienti"
            Case 132971
                Return "Sommario dei nutrienti"
            Case 132972
                Return "I valori nutrizionali sono per porzione al 100%"
            Case 132974
                Return "Scarti"
            Case 132987
                Return "Sommario"
            Case 132989
                Return "Visualizza"
            Case 132997
                Return "Attuale o prima"
            Case 132998
                Return "Attuale o dopo"
            Case 132999
                Return "tra"
            Case 133000
                Return "maggiore di"
            Case 133001
                Return "minore di"
            Case 133005
                Return "Imposto"
            Case 133023
                Return "Visualizza le Opzioni"
            Case 133043
                Return "Gestione immagini in locale"
            Case 133045
                Return "Dimensione massima del file dell'immagine"
            Case 133046
                Return "Dimensione massima dell'immagine"
            Case 133047
                Return "Ottimizzazione"
            Case 133049
                Return "Attivare L'autoconversione per le immagini per l'uso nella pagina Web"
            Case 133057
                Return "Carica il logo per il Sito Web"
            Case 133060
                Return "Colori Web"
            Case 133075
                Return "Nuova Password"
            Case 133076
                Return "Conferma la nuova Password"
            Case 133078
                Return "La Password non corrisponde."
            Case 133080
                Return "Ultimo"
            Case 133081
                Return "Prima"
            Case 133085
                Return "Output Documento"
            Case 133096
                Return "Preparazione della Ricetta"
            Case 133097
                Return "Costi delle ricette"
            Case 133099
                Return "Varianti"
            Case 133100
                Return "Dettagli della Ricetta"
            Case 133101
                Return "Dettagli del Menu"
            Case 133108
                Return "Cosa vuoi stampare?"
            Case 133109
                Return "Selezione delle merci da stampare"
            Case 133111
                Return "Alcune Categorie"
            Case 133112
                Return "Merci selezionate"
            Case 133115
                Return "Tutte le ricette"
            Case 133116
                Return "Ricette selezionate"
            Case 133121
                Return "Menu selezionati"
            Case 133123
                Return "Costi del menu"
            Case 133124
                Return "Descrizione del menu"
            Case 133126
                Return "EGS Standard"
            Case 133127
                Return "EGS Moderno"
            Case 133128
                Return "EGS Due colonne"
            Case 133133
                Return "Nome del file non corretto. Inserire un nome valido."
            Case 133144
                Return "Ricetta Nu."
            Case 133147
                Return "litri"
            Case 133161
                Return "Dimensione foglio"
            Case 133162
                Return "Unità dei margini"
            Case 133163
                Return "Margine sinistro"
            Case 133164
                Return "Margine destro"
            Case 133165
                Return "Margine superiore"
            Case 133166
                Return "Margine inferiore"
            Case 133168
                Return "Formato" & vbCrLf & "Caratteri"
            Case 133172
                Return "Foto piccola / Quantità - Nome"
            Case 133173
                Return "Foto piccola / Nome - Quantità"
            Case 133174
                Return "Foto media / Quantità - Nome"
            Case 133175
                Return "Foto media / Nome - Quantità"
            Case 133176
                Return "Foto grande / Quantità - Nome"
            Case 133177
                Return "Foto grande / Nome - Quantità"
            Case 133196
                Return "Opzioni della lista"
            Case 133201
                Return "Le seguenti merci(e) sono in uso e non saranno cancellate."
            Case 133207
                Return "La Ricetta può essere utilizzata come Sotto Ricetta"
            Case 133208
                Return "Peso"
            Case 133222
                Return "Opzioni dei Dettagli"
            Case 133230
                Return "Le seguenti ricette(a) sono in uso e non saranno cancellate."
            Case 133241
                Return "Ricalcolo dei prezzi. Attendere.."
            Case 133242
                Return "Ricalcolo dei valori nutrizionali. Attendere.."
            Case 133248
                Return "Ingrediente"
            Case 133251
                Return "Separatore"
            Case 133254
                Return "Ordinare per"
            Case 133260
                Return "Fonte in uso."
            Case 133266
                Return "Standardizza i nomi delle Parole Chiave"
            Case 133286
                Return "Definizione"
            Case 133289
                Return "L'unità è in uso."
            Case 133290
                Return "Non puoi unire due o più unità di sistema."
            Case 133295
                Return "Questa unità non può essere cancellata. " & vbCrLf & "Solo le unità definite dall'utente possono essere cancellate.."
            Case 133314
                Return "Solo le unità di resa porzione definite dall'utente possono essere cancellate."
            Case 133315
                Return "Non puoi unire due o più unità di resa porzione di sistema."
            Case 133319
                Return "L'unità di resa porzione è in uso."
            Case 133325
                Return "Sei sicuro di voler pulire le categorie inutilizzate?"
            Case 133326
                Return "Nessuna Fonte"
            Case 133328
                Return "Nome Ricetta"
            Case 133330
                Return "File mancante."
            Case 133334
                Return "Importazione %r"
            Case 133349
                Return "Menu"
            Case 133350
                Return "Articoli per %y (Quantità netta)"
            Case 133351
                Return "Ingredienti per %y" '  al %p% (quantità netta)"
            Case 133352
                Return "Prezzo imposto per persona + Iva"
            Case 133353
                Return "Prezzo imposto per persona"
            Case 133359
                Return "Ordinamento per Numero"
            Case 133360
                Return "Ordinamento per Data"
            Case 133361
                Return "Ordinamento per Categoria"
            Case 133365
                Return "Prezzo di vendita + Iva"
            Case 133367
                Return "Ordinamento per Fornitore"
            Case 133405
                Return "Carica le immagini"
            Case 133475
                Return "Immagine"
            Case 133519
                Return "Seleziona un colore :"
            Case 133590
                Return "&Incolla"
            Case 133692
                Return "Prezzo suggerito"
            Case 134021
                Return "Inventario creato il"
            Case 134032
                Return "Contatti"
            Case 134054
                Return "Informazioni personali"
            Case 134055
                Return "Acquisti"
            Case 134056
                Return "Vendite"
            Case 134061
                Return "Versione, Moduli & Licenze"
            Case 134083
                Return "Prova"
            Case 134111
                Return "Impossibile aggiornare le categorie dei prodotti selezionati."
            Case 134174
                Return "Data di creazione"
            Case 134176
                Return "Merci- Lista dei nutrienti"
            Case 134177
                Return "Ricette-Lista dei nutrienti"
            Case 134178
                Return "Menu-Lista dei nutrienti"
            Case 134182
                Return "Gruppo"
            Case 134194
                Return "Quantità non valida"
            Case 134195
                Return "Prezzo non valido"
            Case 134320
                Return "Indirizzo di fatturazione"
            Case 134332
                Return "Informazioni"
            Case 134333
                Return "Importante"
            Case 134525
                Return "Sei sicuro di voler cancellare le modifiche effettuate?"
            Case 134571
                Return "Volore non valido"
            Case 134826
                Return "Chiuso"
            Case 135024
                Return "Luogo"
            Case 135056
                Return "Regole dei Nutrienti"
            Case 135058
                Return "Aggiungi una regola dei nutrienti"
            Case 135059
                Return "Modifica una regola dei nutrienti"
            Case 135070
                Return "Netta"
            Case 135100
                Return "Numero di Rif."
            Case 135110
                Return "Quantità" & vbCrLf & "Inventario"
            Case 135235
                Return "Valore"
            Case 135256
                Return "Quantità venduta"
            Case 135257
                Return "Margine Lordo"
            Case 135283
                Return "Ultimo prezzo"
            Case 135608
                Return "Porta"
            Case 135948
                Return "Includi la sotto ricetta(e)"
            Case 135951
                Return "Login fallito."
            Case 135955
                Return "Valore numerico non valido."
            Case 135963
                Return "Database"
            Case 135967
                Return "Sostituisci nelle ricette."
            Case 135968
                Return "Sostituisci nei menu."
            Case 135969
                Return "Sei sicuro di voler sostituire %o?"
            Case 135971
                Return "&Connessione"
            Case 135978
                Return "Nuovo"
            Case 135979
                Return "Rinomina"
            Case 135985
                Return "Esistente"
            Case 135986
                Return "Mancante"
            Case 135989
                Return "Articoli"
            Case 135990
                Return "Aggiorna"
            Case 136018
                Return "Paternità"
            Case 136025
                Return "Conversione del database"
            Case 136030
                Return "Contenuti"
            Case 136100
                Return "Inventari attualmente aperti"
            Case 136110
                Return "Aperto il"
            Case 136115
                Return "Nu. di Articoli"
            Case 136171
                Return "Cambia Unità"
            Case 136212
                Return "Visualizza la lista degli aggiustamenti richiesti"
            Case 136213
                Return "Aggiungi un prodotto all'inventario corrente"
            Case 136214
                Return "Rimuovi un prodotto dall'inventario"
            Case 136215
                Return "Aggiungi un nuovo luogo per il prodotto"
            Case 136216
                Return "Rimuovi il luogo di stoccaggio selezionato"
            Case 136217
                Return "Annulla le quantità in inventario per le selezioni"
            Case 136230
                Return "Crea un nuovo inventario"
            Case 136231
                Return "Modifica le informazioni dell'inventario"
            Case 136265
                Return "Sotto Ricette"
            Case 136432
                Return "Codice non valido"
            Case 136601
                Return "Resetta a zero"
            Case 136905
                Return "Simbolo di valuta"
            Case 137019
                Return "Cambio"
            Case 137030
                Return "Predefinito"
            Case 137070
                Return "Impostazioni Generali"
            Case 138030
                Return "Seleziona quali prodotti vuoi per questo inventario."
            Case 138031
                Return "Tutti i prodotti per l'inventario"
            Case 138032
                Return "I prodotti da categorie selezionate"
            Case 138033
                Return "I prodotti da luoghi selezionati"
            Case 138034
                Return "I prodotti da fornitori selezionati"
            Case 138035
                Return "I prodotti da uno o più degli inventari precedenti"
            Case 138137
                Return "Cancellato"
            Case 138244
                Return "Articoli di vendita"
            Case 138402
                Return "Tutti i trasferimenti sono stati effettuati con successo."
            Case 138412
                Return "<non definito>"
            Case 140056
                Return "File"
            Case 140100
                Return "Backup in corso"
            Case 140101
                Return "Ripristino in corso"
            Case 140129
                Return "Errore nel ripristinare il Backup"
            Case 140130
                Return "Errore nel creare il Backup"
            Case 140180
                Return "Percorso dove salvare i file di Backup"
            Case 143001
                Return "Condividi"
            Case 143002
                Return "Non condividere"
            Case 143003
                Return "Quantità" & vbCrLf & "Netta"
            Case 143008
                Return "Scarti"
            Case 143013
                Return "Modifiche"
            Case 143014
                Return "Utente"
            Case 143508
                Return "Ricetta utilizzata come sotto ricetta"
            Case 143509
                Return "Linea separatrice"
            Case 143981
                Return "Numero di conto non valido"
            Case 143987
                Return "Tipo articolo"
            Case 143995
                Return "Azione"
            Case 144582
                Return "No gruppi"
            Case 144591
                Return "Tempo"
            Case 144682
                Return "I valori nutrizionali sono per 100g o 100ml al 100%"
            Case 144684
                Return "I valori nutrizionali sono per una porzione al 100%"
            Case 144685
                Return "per porzione al 100%"
            Case 144686
                Return "per %Y al 100%"
            Case 144687
                Return "per 100g o 100ml al 100%"
            Case 144688
                Return "N/D"
            Case 144689
                Return "I valori nutrizionali sono per 100g o 100ml al 100%"
            Case 144716
                Return "Storico"
            Case 144734
                Return "Lista degli articoli delle vendite"
            Case 144738
                Return "Peso per %Y"
            Case 145006
                Return "Trasferimento"
            Case 146043
                Return "Gennaio"
            Case 146044
                Return "Febbraio"
            Case 146045
                Return "Marzo"
            Case 146046
                Return "Aprile"
            Case 146047
                Return "Maggio"
            Case 146048
                Return "15 Giugno"
            Case 146049
                Return "Luglio"
            Case 146050
                Return "Agosto"
            Case 146051
                Return "Settembre"
            Case 146052
                Return "Ottobre"
            Case 146053
                Return "Novembre"
            Case 146054
                Return "Dicembre"
            Case 146056
                Return "Margine di contribuzione"
            Case 146067
                Return "Bilancio"
            Case 146080
                Return "Cliente"
            Case 146114
                Return "Visualizza in una nuova pagina de il fornitore è diverso"
            Case 146211
                Return "Uscite"
            Case 147070
                Return "OK"
            Case 147075
                Return "Data non valida"
            Case 147126
                Return "Rimuove le selezione esistenti prima"
            Case 147174
                Return "Aperta"
            Case 147381
                Return "Prezzo dell'inventario utilizzato per i prodotti precedentemente"
            Case 147441
                Return "Questo articolo della cassa è già stato collegato."
            Case 147462
                Return "Rapporto"
            Case 147520
                Return "Principale"
            Case 147647
                Return "Il server SQL non esiste, o l'accesso non è permesso"
            Case 147652
                Return "Rimuovi"
            Case 147692
                Return "Informazioni del pasto"
            Case 147699
                Return "Sovrascrivi"
            Case 147700
                Return "Costo totale"
            Case 147703
                Return "Numero di porzioni preparate"
            Case 147704
                Return "Quantità Rimasta"
            Case 147706
                Return "Quantità Ritornata"
            Case 147707
                Return "Quantità Persa"
            Case 147708
                Return "Quantità Venduta"
            Case 147710
                Return "Quantità Venduta Speciale"
            Case 147713
                Return "EGS layout"
            Case 147727
                Return "Costo"
            Case 147729
                Return "Valutazione"
            Case 147733
                Return "Seleziona una lingua"
            Case 147737
                Return "Inserisci la quantità e seleziona un unità"
            Case 147743
                Return "Carica"
            Case 147748
                Return "Anonimo"
            Case 147750
                Return "Commento"
            Case 147753
                Return "Costo del lavoro"
            Case 147771
                Return "Tariffa/Hr"
            Case 147772
                Return "Tariffa/Min"
            Case 147773
                Return "Persone"
            Case 147774
                Return "Tempo (Ore:Minuti)"
            Case 149501
                Return "Utilizza Entrata/Uscita"
            Case 149513
                Return "Appro."
            Case 149531
                Return "Prodotti Finiti"
            Case 149645
                Return "Collegato a"
            Case 149706
                Return "Azzera i Link"
            Case 149761
                Return "Mostra"
            Case 149766
                Return "Prefisso"
            Case 149774
                Return "Pulisci"
            Case 150009
                Return "Exportazione terminata. Le ricette sono state Exportate con successo."
            Case 150333
                Return "Cancellata con successo!"
            Case 150341
                Return "Conversione di valuta"
            Case 150353
                Return "Ordinare"
            Case 150634
                Return "Email spedita con successo."
            Case 150644
                Return "Il Server SMTP è necessario per spedire delle email dal tuo computer."
            Case 150688
                Return "La licenza per questo programma è scaduta."
            Case 150707
                Return "Programma"
            Case 151011
                Return "Ufficio Principale - Svizzero"
            Case 151019
                Return "Parola chiave delle merci"
            Case 151020
                Return "Parole chiave della ricetta"
            Case 151023
                Return "Registrati"
            Case 151250
                Return "Niente è cambiato"
            Case 151286
                Return "Standard"
            Case 151299
                Return "Inserisci le informazioni richieste"
            Case 151322
                Return "Includi magazzino"
            Case 151336
                Return "carica un set delle selezioni"
            Case 151344
                Return "Salva le selezioni per le merci"
            Case 151345
                Return "Salva le selezioni per i piatti"
            Case 151346
                Return "Salva le selezioni per i menu"
            Case 151364
                Return "Seleziona due o più testi"
            Case 151389
                Return "Unisci i testi"
            Case 151400
                Return "Costo delle merci"
            Case 151404
                Return "IVA"
            Case 151424
                Return "Converti alle unità migliori"
            Case 151427
                Return "Ordinamento per il nome dell'articolo"
            Case 151435
                Return "Soggetto"
            Case 151436
                Return "Allegato"
            Case 151437
                Return "CALCMENU"
            Case 151438
                Return "CALCMENU"
            Case 151459
                Return "Il vostro Email"
            Case 151499
                Return "Sostituisci la proposta"
            Case 151500
                Return "Proposta"
            Case 151854
                Return "Excel"
            Case 151886
                Return "Se avete domande riguardanti la tua registrazione, ti invitiamo a contattarci all'indirizzo: e-mail%"
            Case 151890
                Return ""
            Case 151906
                Return "Indirizzo email non trovato"
            Case 151907
                Return "Inserite il vostro nome utente e password."
            Case 151910
                Return "Accedi"
            Case 151911
                Return "Esci"
            Case 151912
                Return "Hai dimenticato la password?"
            Case 151915
                Return "Compilate il modulo che trovate sotto."
            Case 151916
                Return "I campi contrassegnati con * sono obbligatori"
            Case 151917
                Return "Un messaggio di conferma vi sarà inviato via email."
            Case 151918
                Return "Si prega di fornire un indirizzo email valido"
            Case 151920
                Return "Desidero ricevere messaggi email periodici da EGS riguardanti i nostri nuovi prodotti o promozioni in corso (non più di uno al mese)"
            Case 151976
                Return "Luogo predefinito per la produzione"
            Case 152004
                Return "Visualizza la struttura"
            Case 152141
                Return "Gestione delle merci"
            Case 152146
                Return "CAP"
            Case 155024
                Return "Gestione Immagini"
            Case 155046
                Return "Traduzione"
            Case 155050
                Return "TUTTE LE PAROLE CHIAVE"
            Case 155052
                Return "Conferma"
            Case 155118
                Return "Spedisci la lista della spesa al Pocket PC"
            Case 155163
                Return "Cognome"
            Case 155170
                Return "Benvenuto %name!"
            Case 155205
                Return "Home"
            Case 155225
                Return "PDF"
            Case 155236
                Return "Lingua principale"
            Case 155245
                Return "A proposito di noi"
            Case 155260
                Return "Fattore Imposto"
            Case 155263
                Return "pixel"
            Case 155264
                Return "Traduci"
            Case 155374
                Return "ID Account"
            Case 155507
                Return "Permetti"
            Case 155575
                Return "Luogo predef. per lo scarico auto"
            Case 155601
                Return "Nessun articolo selezionato."
            Case 155642
                Return "Scambio di ricette"
            Case 155654
                Return "Ingredienti per %s al %p% (quantitá netta)"
            Case 155713
                Return "%r esiste."
            Case 155731
                Return "CALCMENU Pro"
            Case 155761
                Return "Importa merci"
            Case 155763
                Return "Confronta per numero"
            Case 155764
                Return "Confronta per ingredienti"
            Case 155811
                Return "Quantità" & vbCrLf & "Lorda"
            Case 155841
                Return "File da ripristinare"
            Case 155842
                Return "Persone"
            Case 155861
                Return "Imposta a 0  le quantità per gli articoli selezionati"
            Case 155862
                Return "per"
            Case 155926
                Return "Esportazione dati in Excel"
            Case 155927
                Return "TUTTE LE FONTI"
            Case 155942
                Return "Carica la lista delle liste degli acquisti salvate"
            Case 155947
                Return "Filtra per"
            Case 155967
                Return "Separatore per i file Excel"
            Case 155994
                Return "Non Attivo"
            Case 155995
                Return "Controllo...."
            Case 155996
                Return "Indirizzo Email"
            Case 156000
                Return "Assegna un nuovo Fornitore"
            Case 156012
                Return "Supporto"
            Case 156015
                Return "Supporto Tecnico / Contatti"
            Case 156016
                Return "Ufficio principale"
            Case 156060
                Return "CM Imposto"
            Case 156061
                Return "Profitto Imposto"
            Case 156141
                Return "BackUp/Restore del Database"
            Case 156337
                Return "Collega nutriente"
            Case 156344
                Return "Selezione non valida."
            Case 156355
                Return "Archivi"
            Case 156356
                Return "Includi"
            Case 156405
                Return "Liberate dello spazio poi cliccate per riprovare"
            Case 156413
                Return "Definizione delle Sotto Ricette"
            Case 156485
                Return "Cancella i file dopo l'importazione"
            Case 156542
                Return "Prezzo medio ponderato"
            Case 156552
                Return "Backup Ora"
            Case 156590
                Return "Importare delle merci da un file CSV (Excel)"
            Case 156669
                Return "Sito Web"
            Case 156672
                Return "Utilizzate 'On line' (per contenuti del sito)"
            Case 156683
                Return "Originale"
            Case 156720
                Return "Numero troppo lungo"
            Case 156721
                Return "Nome troppo lungo"
            Case 156722
                Return "Fornitore troppo lungo"
            Case 156723
                Return "Categoria troppo lunga"
            Case 156725
                Return "Descrizione  troppo lunga"
            Case 156734
                Return "Due unità sono uguali"
            Case 156742
                Return "Scade tra"
            Case 156751
                Return "Direct line: +41 32 544 0017<br><br>24/7 English Customer Support: +1 800 964 9357<br><br>Sales: +41 848 000 357<br>Fax: +41 32 753 0275"
            Case 156752
                Return "24/7 Toll Free: +1-800-964-9357"
            Case 156753
                Return "Office line +632 687 3179"
            Case 156754
                Return "Nome del file"
            Case 156784
                Return "Totale errori: %n"
            Case 156825
                Return "Migliaia"
            Case 156870
                Return "Sei sicuro?"
            Case 156892
                Return "Scarica:"
            Case 156925
                Return "Scaricamento OK!"
            Case 156938
                Return "Active"
            Case 156941
                Return "Pocket Kitchen"
            Case 156955
                Return "Privato"
            Case 156957
                Return "Hotel"
            Case 156959
                Return "Condiviso"
            Case 156960
                Return "Submitted"
            Case 156961
                Return "Impostazione del prezzo"
            Case 156962
                Return "Non Inserito"
            Case 156963
                Return "Prezzi"
            Case 156964
                Return "Trovi in"
            Case 156965
                Return "Rendimenti"
            Case 156966
                Return "Records colpite"
            Case 156967
                Return "Si prega di inserire la data corretta."
            Case 156968
                Return "Formato di file immagine non valido"
            Case 156969
                Return "Si prega di inserire il file immagine da caricare. In caso contrario, lasciare in bianco."
            Case 156970
                Return "Inserisci Categoria Informazioni"
            Case 156971
                Return "Inserisci il prezzo stabilito Informazioni"
            Case 156972
                Return "Inserisci le parole chiave di informazione"
            Case 156973
                Return "Inserisci Unità Informazione"
            Case 156974
                Return "Inserisci Rendimento Informazioni"
            Case 156975
                Return "Creare nuove ricette e trasmettere alla sede principale per l'utilizzo con altri hotel."
            Case 156976
                Return "Merchandise è l'elemento di base o la voce che comprende le vostre ricette e menu."
            Case 156977
                Return "Se avete domande o questioni tecniche su questo software."
            Case 156978
                Return "Capogruppo di parole chiave"
            Case 156979
                Return "Nome di parole chiave"
            Case 156980
                Return "Configurazione"
            Case 156981
                Return "Aliquote IVA"
            Case 156982
                Return "Risultati della ricerca"
            Case 156983
                Return "Spiacenti, non sono stati trovati risultati."
            Case 156984
                Return "Nome utente o password non validi"
            Case 156986
                Return "esiste già."
            Case 156987
                Return "è stato salvato con successo."
            Case 156996
                Return "Copyright © 2004 of EGS Enggist & Grandjean Software SA, Switzerland."
            Case 157002
                Return "Prezzo per l'unità non è definito. Si prega di selezionare una unità."
            Case 157020
                Return "Iva utilizzata"
            Case 157026
                Return "Medio"
            Case 157033
                Return "Il sistema consente di aggiornare i prezzi di tutte le merci. Ti preghiamo di attendere ..."
            Case 157034
                Return "Autenticazione"
            Case 157038
                Return "Mese"
            Case 157039
                Return "Anno"
            Case 157040
                Return "Non ci sono parole chiave disponibili."
            Case 157041
                Return "l'accesso non è permesso"
            Case 157049
                Return "Sei sicuro di voler salvare?"
            Case 157055
                Return "VERSIONE STUDENTE"
            Case 157056
                Return "Vuoi cancellare?"
            Case 157057
                Return "Contrassegnato oggetti sono ora condivise."
            Case 157060
                Return "Numero di riferimento"
            Case 157065
                Return "Esporta in CALCMENU"
            Case 157066
                Return "Esporta in CALCMENU"
            Case 157076
                Return "Sommario d' aiuto"
            Case 157079
                Return "I seguenti elementi non sono segnate presentate e non possono essere trasferiti:"
            Case 157084
                Return "I seguenti elementi marcati vengono utilizzati e non vengono eliminati:"
            Case 157125
                Return "Visualizza"
            Case 157130
                Return "I vostri dati della carta di credito è stato inviato con successo. L'abbonamento sarà processata entro tre giorni. Grazie!"
            Case 157132
                Return "Personali (condivisione)"
            Case 157133
                Return "Personali (non condivisi)"
            Case 157134
                Return "Visitatore"
            Case 157136
                Return "Crediti"
            Case 157139
                Return "La cosa peggiore!"
            Case 157140
                Return "Buono!"
            Case 157141
                Return "Fantastico!"
            Case 157142
                Return "Cancella l'unità e il prezzo di una ingredienti che non è stata trovata nel file da importare"
            Case 157151
                Return "Altri link"
            Case 157152
                Return "Recensioni degli utenti"
            Case 157153
                Return "Il beneficiario sarà richiesto di accettare tali articoli."
            Case 157154
                Return "I seguenti oggetti non possono essere fornite in quanto sono di proprietà di altri utenti."
            Case 157155
                Return "Qualcuno vorrebbe darvi le seguenti ricette:"
            Case 157156
                Return "Promo"
            Case 157157
                Return "Opinioni degli utenti"
            Case 157158
                Return "Originalità"
            Case 157159
                Return "Risultato"
            Case 157160
                Return "Difficoltà"
            Case 157161
                Return "La ricetta del giorno"
            Case 157164
                Return "Nome del titolare della carta"
            Case 157165
                Return "Numero della carta di credito"
            Case 157166
                Return "Record Limit"
            Case 157168
                Return "Banca"
            Case 157169
                Return "PayPal"
            Case 157170
                Return "L'ordine on line non è disponibile per la vostra nazione. " & vbCrLf & "" & vbCrLf & " Scaricate e compilate il modulo per l'ordine ed inviatelo via fax al seguente numero  +1 732-353-5173"
            Case 157171
                Return "Diventa un membro"
            Case 157172
                Return "Aggiornamento a pagamento"
            Case 157173
                Return "Abbonamento"
            Case 157174
                Return "Pacchetti di aggiornamento"
            Case 157176
                Return "Totale segnalazioni utilizzati"
            Case 157177
                Return "Offriamo una varietà di soluzioni per soddisfare le vostre esigenze"
            Case 157178
                Return "Trial utente"
            Case 157179
                Return "Dillo a un amico"
            Case 157180
                Return "Friend's e-mail"
            Case 157182
                Return "FAQ"
            Case 157183
                Return "Termini e condizioni del servizio"
            Case 157214
                Return "Crea lista della spesa solamente per le merci delle ricette selezionate"
            Case 157217
                Return "Crea lista della spesa per gli ingredienti dei menu selezionati"
            Case 157226
                Return "Contrassegnato ricette sono stati inviati per l'approvazione."
            Case 157233
                Return "Lo scarto totale non deve essere superiore o uguale a 100%"
            Case 157268
                Return "Valuta."
            Case 157269
                Return "Set di prezzo è in uso."
            Case 157273
                Return "Non è possibile condividere i seguenti elementi, perché non sono state né presentato né di proprietà."
            Case 157274
                Return "Exchange Rate"
            Case 157275
                Return "Tutti gli elementi elencati saranno fusi in uno solo. Si prega di selezionare una voce per essere utilizzati dagli utenti. Altri oggetti saranno cancellati dal database."
            Case 157276
                Return "Portato a termine la fusione."
            Case 157277
                Return "Costo totale"
            Case 157281
                Return "Prezzo del Fornitore predefinito"
            Case 157297
                Return "Selezionate almeno un articolo."
            Case 157299
                Return "Modifica profilo e personalizzare il tuo punto di vista."
            Case 157300
                Return "Si prega di inserire la nuova password. La password non può superare i 20 caratteri. Fare clic su 'Invia' quando hai finito."
            Case 157301
                Return "Si prega di inserire il file immagine (JPEG / JPG, BMP, ecc) che si desidera caricare. In caso contrario, lasciare in bianco. (Nota: i file GIF non è supportato. Tutte le immagini vengono copiati e poi convertito in normale e anteprima in formato jpeg)."
            Case 157302
                Return "Cerca per nome ingrediente o di una parte del nome (uso [*] asterisco). Per aggiungere rapidamente, inserisci [netto quanitity] _ [unità] _ [ingredienti] come 200 g Oel Alto oleico"
            Case 157303
                Return "Per aggiungere o modificare il prezzo della ingredienti, immettere il nuovo prezzo e definire le unità di misura. Assegna il rapporto di tale unità per l'originale unità. Ad esempio, il prezzo originario e unitario è di US $ 11 per chilogrammo (kg). Se si desidera aggiungere l'unità di borsa, è necessario definire il prezzo di quel sacco, o definire quanti kg ci sono in 1 borsa (rapporto)."
            Case 157304
                Return "Parole chiave di ricerca per nome o una parte del nome. Usa la virgola [,] per più parole chiave. Ad esempio, la ricerca ""manzo, salsa di nozze""."
            Case 157305
                Return "Scegli"
            Case 157306
                Return "Tipo di file non valido."
            Case 157310
                Return "Dettagli delle merci"
            Case 157314
                Return "Utilizza le unità principali quando si aggiorna il prezzo delle merci"
            Case 157320
                Return "Condividi"
            Case 157322
                Return "Licenza d'uso"
            Case 157323
                Return "Dare"
            Case 157329
                Return "Terminale"
            Case 157334
                Return "Avvertenza: Si può perdere tutte le modifiche, se un altro utente ha modificato il record. Vuoi ricaricare questa pagina?"
            Case 157336
                Return "Non applicabile"
            Case 157339
                Return "Messaggi per pagina"
            Case 157340
                Return "Quick sfogliare"
            Case 157341
                Return "su ogni pagina"
            Case 157342
                Return "Il record è stato modificato da un altro utente. Fare clic su OK per procedere."
            Case 157343
                Return "Questo record è stato eliminato da un altro utente."
            Case 157345
                Return "Presentare al Capo Ufficio"
            Case 157346
                Return "Non condiviso"
            Case 157378
                Return "Stato"
            Case 157379
                Return "Iscriviti ora"
            Case 157380
                Return "L'abbonamento scade il n.%"
            Case 157381
                Return "L'abbonamento è scaduto."
            Case 157382
                Return "Estendere la mia adesione utilizzando il mio restanti punti (crediti)"
            Case 157383
                Return "Liberate dello spazio poi cliccate per riprovare"
            Case 157384
                Return "Operazione non valida"
            Case 157385
                Return "Grazie!"
            Case 157387
                Return "Verrai reindirizzato a PayPal per completare la vostra iscrizione. Si prega di prendere un momento di scegliere quale moneta di utilizzare, al fine di addebitare l'importo corretto. Si prega di scegliere dalla lista qui sotto."
            Case 157388
                Return "Un invito a partecipare"
            Case 157404
                Return "In attesa di operazione."
            Case 157405
                Return "Per informazioni, si prega di inviare un'e-mail a"
            Case 157408
                Return "Solo i membri di prova e gli utenti possono accedere a questa pagina. Vuoi gestire la tua ricetta Ricetta Gallery.com? Vai al menu di sottoscrizione e la sottoscrizione di un membro."
            Case 157435
                Return "Trasferisce automaticamente ad un punto vendita prima dello scarico"
            Case 157437
                Return "Merci non scaricabili"
            Case 157446
                Return "Mese"
            Case 157515
                Return "Olandese"
            Case 157594
                Return "Accetta"
            Case 157595
                Return "Negare"
            Case 157596
                Return "N. User Review"
            Case 157604
                Return "Supporto Email"
            Case 157607
                Return "Supporto telefonico"
            Case 157608
                Return "Supporto Online"
            Case 157616
                Return "Stati Uniti d'America"
            Case 157617
                Return "Asia e il resto del mondo"
            Case 157629
                Return "Approvare"
            Case 157633
                Return "Disapprovato"
            Case 157659
                Return "Blocca"
            Case 157660
                Return "Sblocca"
            Case 157695
                Return "Conto per la contabilità"
            Case 157714
                Return "Commenti"
            Case 157772
                Return "Opzionale"
            Case 157793
                Return "Presentazione"
            Case 157802
                Return "Conferma la password"
            Case 157901
                Return "Nascondi esistenti"
            Case 157926
                Return "Registrati"
            Case 157985
                Return "È sempre possibile modificare la password, procedi nel seguente modo:"
            Case 157986
                Return "Accedi al sito Web all'indirizzo EGS <a href='http://www.eg-software.com'>http://www.eg-software.com.</a>"
            Case 157992
                Return "Ha recentemente richiesto il nome utente e la password per accedere al tuo conto EGS Login."
            Case 157993
                Return "Si prega di trovare i dettagli qui di seguito"
            Case 158005
                Return "Licenze"
            Case 158019
                Return "Check Richiesta Stato"
            Case 158157
                Return "Ingredienti per %y"
            Case 158169
                Return "Scegliete le vostre condizioni di pagamento." & vbCrLf & "" & vbCrLf & "Pagamento anticipato con:"
            Case 158170
                Return "Gentilmente ci spedisca via email i dettagli della sua carta di credito a <a href='mailto:info@calcmenu.com'>info@calcmenu.com</a>. Tipo di carta di credito (Visa, Mastercard, American Express), Nome del titolare della carta, numero della carta di credito e data di scadenza."
            Case 158171
                Return "Trasferimento bancario"
            Case 158174
                Return "Nota: CI avvisi una volta che il trasferimento è avvenuto. Possono essere necessari sa 1 a 2 settimane prima che il trasferimento bancario arrivi."
            Case 158186
                Return "Cambia la password"
            Case 158216
                Return "Gestione delle ricette centralizzata sempre ed ovunque"
            Case 158220
                Return "Crea un nuovo nome di ingredienti con un massimo di 250 caratteri alfanumerici e includere il numero di riferimento, aliquota d'imposta, quattro sprechi percentuali, categoria, fornitore, e altre utili informazioni quali la descrizione del prodotto, la preparazione, la cucina punta, metodi di affinamento e lo stoccaggio."
            Case 158229
                Return "Fotografie"
            Case 158230
                Return "Merci, ricette, menu e possono essere cercati utilizzando il proprio nome o numeri di riferimento. Puoi anche cercare attraverso le categorie e le parole chiave. Per la ingredienti, è possibile utilizzare anche il fornitore, data codificati o modificata per l'ultima volta, gamma di prezzi e valori nutrizionali durante la ricerca. Per le ricette e menu, è possibile effettuare una ricerca utilizzando gli oggetti utilizzati e non utilizzati."
            Case 158232
                Return "Azione marchi sono scorciatoie nello svolgimento di una funzione simile che potrebbe applicarsi a una ingredienti, ricette o menu. È possibile utilizzare l'azione di assegnare marchi ingredienti, ricetta, o ad una categoria di menu e le parole chiave, eliminarli, l'esportazione, inviare via e-mail, stampare, condividere e unshare ad altri utenti senza dover ripetere per ogni voce. Questo vi risparmierà un sacco di tempo e di impegno nello svolgimento di un ricorso al segnate oggetti."
            Case 158234
                Return "Collegamento e calcolo dei valori nutrizionali"
            Case 158238
                Return "Gestione dei fornitori"
            Case 158240
                Return "Gestione delle categorie, parole chiave e fonti"
            Case 158243
                Return "Gestione aliquote IVA"
            Case 158246
                Return "Gestione delle unità"
            Case 158249
                Return "Stampa, Esportazione in formato PDF e Excel"
            Case 158306
                Return "Selezionare"
            Case 158346
                Return "più"
            Case 158349
                Return "Parole chiave assegnate"
            Case 158350
                Return "Parole chiave derivate"
            Case 158376
                Return "Totale Prezzo di vendita imposto"
            Case 158410
                Return "Se alcuni prodotti hanno il prezzo definito (prezzo = 0), utilizza il prezzo del fornitore predefinito per tutti i prodotti con prezzo = 0."
            Case 158511
                Return "Se ritieni che questo non è il caso, vi preghiamo di inviarci una e-mail <a href='mailto:%email'>e-mail%</a>"
            Case 158577
                Return "Lingua del sito"
            Case 158585
                Return "Headoffice"
            Case 158588
                Return "Non possono presentare i seguenti elementi, perché sono di proprietà di un altro utente."
            Case 158653
                Return "Mobile"
            Case 158677
                Return "Numero" & vbCrLf & "Articolo di vendita"
            Case 158694
                Return "Modifica le informazioni"
            Case 158696
                Return "Per i clienti solo filippino"
            Case 158730
                Return "Escludi"
            Case 158734
                Return "La versione del database non è compatibile con questa versione del programma."
            Case 158783
                Return "Includi ricetta(e)/sottoricetta(e)"
            Case 158810
                Return "Calcola Prezzo"
            Case 158835
                Return "Ordinamento per IVA"
            Case 158837
                Return "Ordinamento per prezzo"
            Case 158839
                Return "Ordinamento per costo delle merci"
            Case 158840
                Return "Ordinamento per fattore"
            Case 158845
                Return "Ordinamento per prezzo di vendita teorico"
            Case 158846
                Return "Ordinamento per prezzo di vendita imposto"
            Case 158849
                Return "Alta"
            Case 158850
                Return "Bassa"
            Case 158851
                Return "Creata da"
            Case 158860
                Return "Modifica le impostazioni del POS"
            Case 158868
                Return "Cinese"
            Case 158902
                Return "Orario di apertura"
            Case 158912
                Return "Richieste"
            Case 158935
                Return "Totale ricavi"
            Case 158946
                Return "Quantità in stock come quantità dell'inventario"
            Case 158947
                Return "Verrai reindirizzato a Paypal per completare l'ordine."
            Case 158952
                Return "Approvato"
            Case 158953
                Return "Non Approvate"
            Case 158960
                Return "Questa funzione è stata disabilitata. Contattate il vostro ufficio principale se avete bisogno di nuove ricette."
            Case 158998
                Return "Funzioni di ricerca"
            Case 158999
                Return "Merchandise, ricette, menu e le liste possono essere stampate con i loro dettagli, prezzi e valori nutrizionali. Shopping liste o la lista degli ingredienti insieme con cumulativo quantitativi utilizzati in varie ricette possono inoltre essere stampati. PDF e file di Excel possono essere create anche per le varie relazioni."
            Case 159000
                Return "Gestione di listini prezzi diversi e delle valute"
            Case 159009
                Return "Bordo"
            Case 159035
                Return "Incompleta"
            Case 159064
                Return "In nome non può essere vuoto"
            Case 159082
                Return "Aggiorna i prodotti basandoti sull'ultima data di modifca del prodotto."
            Case 159088
                Return "Invia per approvazione"
            Case 159089
                Return "Cancella approvazione"
            Case 159112
                Return "Per approvazione"
            Case 159113
                Return "Non ereditabile"
            Case 159133
                Return "Informazioni per la spedizione"
            Case 159139
                Return "Composizione"
            Case 159140
                Return "Unità troppo lunga"
            Case 159141
                Return "L'unità &n non esiste"
            Case 159142
                Return "%n non può essere vuota"
            Case 159144
                Return "Importazione file. Attendere..."
            Case 159145
                Return "Salvataggio dati. attendere..."
            Case 159162
                Return "&Nascondi i dettagli"
            Case 159168
                Return "Ordinamento per quantità netta"
            Case 159169
                Return "Ordinamento per quantità lorda"
            Case 159171
                Return "Orario"
            Case 159181
                Return "Ordinamento per totale"
            Case 159264
                Return "Importa le merci CSV / Rete dei fornitori"
            Case 159273
                Return "Margine di contribuzione totale"
            Case 159274
                Return "% numero solo"
            Case 159275
                Return "Limitato dalla licenza"
            Case 159298
                Return "Parole chiave per i menu"
            Case 159349
                Return "Azzera i filtri"
            Case 159350
                Return "Il Piano di Supporto e Aggiornamenti sono scaduti"
            Case 159360
                Return "Proprietà Chef"
            Case 159361
                Return "Executive Chef"
            Case 159362
                Return "Selezionato in uso."
            Case 159363
                Return "Inserisci marca informazioni"
            Case 159364
                Return "Marchio"
            Case 159365
                Return "Ruolo"
            Case 159366
                Return "Utilizzando il server SMTP"
            Case 159367
                Return "Utilizzo SMTP sulla rete"
            Case 159368
                Return "Logo"
            Case 159369
                Return "Confronta per"
            Case 159370
                Return "importati con successo."
            Case 159372
                Return "Global"
            Case 159379
                Return "ascendente"
            Case 159380
                Return "discendente"
            Case 159381
                Return "Esporre a tutti gli utenti"
            Case 159382
                Return "Convertito al sistema Ricetta"
            Case 159383
                Return "Non esporre"
            Case 159384
                Return "Proprietà"
            Case 159385
                Return "Invia entrata"
            Case 159386
                Return "Prezzi e nutrienti non sono stati ricalcolati."
            Case 159387
                Return "I prezzi e le sostanze nutrienti sono stati ricalcolati."
            Case 159388
                Return "Crea un nuovo menu di carte"
            Case 159389
                Return "Modificare Menu Card"
            Case 159390
                Return "Email inviata. Grazie!"
            Case 159391
                Return "Approvazione Prezzo"
            Case 159424
                Return "Questa funzione è stata disabilitata. Contattate il vostro ufficio principale se avete bisogno di nuove merci."
            Case 159426
                Return "Cerca ingrediente dal nome o parte del nome. Per aggiungere rapidamente, inserisci [netto quanitity] _ [unità] _ [ingredienti]."
            Case 159430
                Return "Le informazioni di registrazione sono state salvate con successo."
            Case 159433
                Return "Invia al sistema"
            Case 159434
                Return "Presentata al sistema"
            Case 159435
                Return "Spostare gli elementi selezionati in una nuova categoria"
            Case 159436
                Return "E-mail del mittente per il sistema di notifiche di"
            Case 159437
                Return "File è stato caricato con successo."
            Case 159444
                Return "Imporre Immagine Dimensioni"
            Case 159445
                Return "Fuso orario"
            Case 159446
                Return "Gestore delle immagini"
            Case 159457
                Return "SQL Server Testo ricerca ha la capacità di effettuare query complesse contro carattere dati. Full Text Search permette la ricerca di testi simili. Per esempio, la ricerca ""pomodoro"" anche rendimento ""pomodori"". SQL 2009 prevede la classificazione dei risultati di ricerca basati su le partite nel nome, la nota (o procedura), e l'ingrediente di ricerca."
            Case 159458
                Return "Full popolazione"
            Case 159459
                Return "Ricerca a tutto testo"
            Case 159460
                Return "minuti"
            Case 159461
                Return "Ogni"
            Case 159462
                Return "Eseguite"
            Case 159463
                Return "Incrementali Popolazione"
            Case 159464
                Return "Lingua Parola ruttore"
            Case 159468
                Return "Utilizzato come ingrediente"
            Case 159469
                Return "Non utilizzato come ingrediente"
            Case 159471
                Return "indirizzo IP"
            Case 159472
                Return "Elenco IP bloccati"
            Case 159473
                Return "Blocca PI, quando i tentativi di login raggiungere"
            Case 159474
                Return "Inserire al meno " & vbCrLf & " caratteri per la Testata"
            Case 159485
                Return "Invia a Recipe Exchange"
            Case 159486
                Return "Presentata a Ricetta Exchange"
            Case 159487
                Return "È stato approvato questa ricetta. Si può ora essere visto da tutti gli utenti."
            Case 159488
                Return "Lingua sconosciuta."
            Case 159594
                Return "Aggiungi alla ricetta"
            Case 159607
                Return "Programma per la gestione di ricette standalone"
            Case 159608
                Return "Programma per la gestione di ricette adatto a essere utilizzato in rete da più utenti"
            Case 159609
                Return "Sistema per la gestione di ricette Web Based"
            Case 159610
                Return "Programma per la gestione del magazzino, vendite e procedure di Back Office"
            Case 159611
                Return "Visualizzatore di ricette per Pocket PC"
            Case 159612
                Return "Gestione delle prenotazioni dei pasti e monitoraggio dell'analisi nutrizionale"
            Case 159613
                Return "Programma E-Cookbook"
            Case 159681
                Return "La ricetta (%s) ha troppi ingredienti. (massimo %n)"
            Case 159689
                Return "Inviate con la fotografia"
            Case 159690
                Return "Inviate senza la fotografia"
            Case 159699
                Return "Aggiorna gli articoli esistenti"
            Case 159700
                Return "&Importazione di Ricette/Menu"
            Case 159707
                Return "Francia"
            Case 159708
                Return "Germania"
            Case 159733
                Return "Articolo nu:"
            Case 159751
                Return "Sito"
            Case 159778
                Return "Avanzato"
            Case 159779
                Return "Normale"
            Case 159782
                Return "Collega articoli di vendita ai prodotti"
            Case 159783
                Return "Collega articoli di vendita alla ricette/menu"
            Case 159795
                Return "Importare da POS - Configurazione"
            Case 159918
                Return "Non hai i diritti per accedere a questa funzione. Contatta il tuo amministratore per cambiare i tuoi diritti d'accesso."
            Case 159924
                Return "Gestisci"
            Case 159925
                Return "La conversione non valida"
            Case 159929
                Return "Opzioni della pagina"
            Case 159934
                Return "Includi le informazioni nutrizionali"
            Case 159940
                Return "Esporta Aggiornamenti"
            Case 159941
                Return "Esporta tutte le"
            Case 159942
                Return "Uscita Directory"
            Case 159943
                Return "Qualità"
            Case 159944
                Return "Parente"
            Case 159946
                Return "CALCMENU Web 2008"
            Case 159947
                Return "Selezionare o caricare il file"
            Case 159949
                Return "Formato non deve superare i 10 caratteri."
            Case 159950
                Return "Nutrienti nome non deve superare i 25 caratteri."
            Case 159951
                Return "Ruoli"
            Case 159962
                Return "Inserisci Informazioni fiscali"
            Case 159963
                Return "Inserisci Traduzione"
            Case 159966
                Return "Spostare oggetti segnate a nuovo marchio"
            Case 159967
                Return "Inserisci il nome di default del sito:"
            Case 159968
                Return "Inserisci il sito Web predefinito tema"
            Case 159969
                Return "Attiva dal raggruppamento dei siti di proprietà di essere gestita da admin di proprietà:"
            Case 159970
                Return "Richiedere agli utenti di inviare le informazioni per la prima approvazioni prima che possa essere usato o pubblicati:"
            Case 159971
                Return "Inserisci la traduzione per ogni lingua o il testo di default può essere utilizzato:"
            Case 159973
                Return "Seleziona i siti che dovrebbero appartenere a questa proprietà"
            Case 159974
                Return "Selezionare le lingue disponibili da utilizzare per la traduzione di merci, ricette, menu e altre informazioni"
            Case 159975
                Return "Seleziona uno o più gruppi di prezzo da utilizzare per l'assegnazione dei prezzi alla vostra ingredienti, ricette e menu"
            Case 159976
                Return "Controllare le voci di includere"
            Case 159977
                Return "Elenco dei proprietari"
            Case 159978
                Return "Scegliere un formato di seguito"
            Case 159979
                Return "Scegli elenco di base di spurgo"
            Case 159981
                Return "Di seguito sono riportati i siti condivisi per questa voce"
            Case 159982
                Return "Spostare di nuovo segnato fonte"
            Case 159987
                Return "Tipo richiesta"
            Case 159988
                Return "Commissionato dalla"
            Case 159990
                Return "Cambia marca"
            Case 159994
                Return "Sostituire una ingredienti/ricetta in un menu"
            Case 159997
                Return "Global Condivisione"
            Case 160004
                Return "Primo Livello"
            Case 160005
                Return "La scelta degli ingredienti deve avere i seguenti unità:"
            Case 160008
                Return "Passaggio"
            Case 160009
                Return "Altre azioni"
            Case 160012
                Return "Questa ricetta / menu è pubblicato sul web."
            Case 160013
                Return "Questa ricetta / menu non è pubblicato sul web."
            Case 160014
                Return "Ricordati di me"
            Case 160016
                Return "Vedi Proprietari"
            Case 160018
                Return "Questa ingredienti è pubblicato sul web."
            Case 160019
                Return "Questa ingredienti non è pubblicato sul web."
            Case 160020
                Return "Questa ingredienti è esposta."
            Case 160021
                Return "Questa ingredienti non è esposta."
            Case 160023
                Return "Per la stampa"
            Case 160028
                Return "Non deve essere pubblicato"
            Case 160030
                Return "Aggiungi alla lista della spesa"
            Case 160033
                Return "Aggiungi parole chiave"
            Case 160035
                Return "Si è tentato di login% n volte"
            Case 160036
                Return "Questo account è stato disattivato"
            Case 160037
                Return "Contattare l'amministratore di sistema per riattivare l'account."
            Case 160038
                Return "Il mio profilo"
            Case 160039
                Return "Ultimo login"
            Case 160040
                Return "Lei non è firmato pollici"
            Case 160041
                Return "Page Language"
            Case 160042
                Return "Main traduzione"
            Case 160043
                Return "Main Set di Prezzo"
            Case 160045
                Return "Righe per pagina"
            Case 160046
                Return "Visualizzato predefinito"
            Case 160047
                Return "Ingrediente Quantitativi"
            Case 160048
                Return "Ultimo accesso"
            Case 160049
                Return "Ricevuto '% f'"
            Case 160050
                Return "Lunghezza"
            Case 160051
                Return "Impossibile ricevere '% f'"
            Case 160055
                Return "Quantità deve essere superiore a 0."
            Case 160056
                Return "Creare un nuovo sub-ricetta"
            Case 160057
                Return "Sessione è scaduta."
            Case 160058
                Return "I tuoi dati di accesso è scaduta a causa di inattività per% n minuti."
            Case 160065
                Return "No name"
            Case 160066
                Return "Sei sicuro di voler chiudere?"
            Case 160067
                Return "La tua voce richiede l'approvazione"
            Case 160068
                Return "Fare clic sul '% s' pulsante per richiedere l'approvazione."
            Case 160070
                Return "Contrassegnato oggetti che devono essere trattati"
            Case 160071
                Return "Questa voce è stata presentata per l'approvazione."
            Case 160072
                Return "Esiste già una richiesta per questa voce."
            Case 160074
                Return "Seleziona l'unità"
            Case 160082
                Return "Attendo le vostre nuove richieste di approvazione."
            Case 160085
                Return "La tua richiesta è stata oggetto di una revisione."
            Case 160086
                Return "Stampa Elenco dei nutrienti"
            Case 160087
                Return "Stampa Listino"
            Case 160088
                Return "Stampa Dettagli"
            Case 160089
                Return "Attivazione"
            Case 160090
                Return "Creare"
            Case 160091
                Return "Rimuovere l'elemento selezionato dalla lista."
            Case 160093
                Return "Invia al sistema globale per la condivisione"
            Case 160094
                Return "I contenuti disponibili sul chiosco browser"
            Case 160095
                Return "Creare un sistema di copia"
            Case 160096
                Return "Sostituire ingrediente utilizzato in ricette e menu"
            Case 160098
                Return "Non pubblicare sul web"
            Case 160100
                Return "Crea un elenco di ingredienti per essere acquistati"
            Case 160101
                Return "È possibile utilizzare il testo come ingredienti che non hanno bisogno di quantità e prezzo definizioni."
            Case 160102
                Return "Crea la tua ricetta base di dati, condividere con altri utenti, di stampa, e persino creare una lista della spesa per questo."
            Case 160103
                Return "Il menu è una lista di ingredienti e ricette disponibili in un pasto."
            Case 160105
                Return "Organizzare le informazioni di base come ad esempio quelli relativi agli utenti, fornitori, ecc"
            Case 160106
                Return "Benvenuto"
            Case 160107
                Return "Benvenuti a% s"
            Case 160108
                Return "Personalizza la tua vista e altre impostazioni."
            Case 160109
                Return "Profilo di sito Web"
            Case 160110
                Return "Personalizzare il nome del sito Web, i temi, ecc"
            Case 160111
                Return "Approvazione Routing"
            Case 160112
                Return "Approvazione della ingredienti, ricette e altre informazioni."
            Case 160113
                Return "SMTP e di notifica Impostazioni"
            Case 160114
                Return "Configurare la connessione al server di posta, abilitare o disabilitare gli avvisi."
            Case 160115
                Return "Imposta massimo i tentativi di accesso e controllare gli indirizzi IP bloccati."
            Case 160116
                Return "Profilo Stampa"
            Case 160117
                Return "Definire più formati di stampa come profili."
            Case 160118
                Return "Definisci elenco di lingue per la traduzione di merci, ricette, menu e altre informazioni."
            Case 160119
                Return "Disponibile monete per conversione di valuta e di definizione dei prezzi."
            Case 160120
                Return "Lavora con la ingredienti, ricette e menu con varie serie di prezzi."
            Case 160121
                Return "Proprietà sono gruppi di siti."
            Case 160122
                Return "Siti organizzare gli utenti che lavorano insieme su un particolare insieme di ricette."
            Case 160123
                Return "Gestire gli utenti che lavorano su% s"
            Case 160124
                Return "Elaborazione immagini Preferenze"
            Case 160125
                Return "Definire le dimensioni standard per le merci, ricette e menu."
            Case 160130
                Return "Marchi o nomi di identificazione distintivo della ingredienti."
            Case 160132
                Return "Utilizzato per gruppo di merci, ricette, menu o di comune attributi."
            Case 160135
                Return "Parole chiave descrittive fornire dettagli al fine di merci, ricette, o menu. Gli utenti possono assegnare più parole chiave per le merci, ricetta, o un menu."
            Case 160139
                Return "Definire fino a 34 elementi nutritivi valori per le sostanze nutritive, come l'energia, carboidrati, proteine e lipidi."
            Case 160141
                Return "Crea delle regole che possono essere utilizzate come un ulteriore filtro per la ricerca."
            Case 160151
                Return "Elenco dei predefiniti (o sistema), unità utilizzate per la definizione dei prezzi di merci, nonché la codifica delle ricette e dei menu."
            Case 160152
                Return "Gli utenti possono aggiungere a questo elenco."
            Case 160153
                Return "Usato nel calcolo del prezzo"
            Case 160154
                Return "Fonte riferisce l'origine di una particolare ricetta. Può essere un cuoco, libro, rivista, servizi di ristorazione aziendale, l'organizzazione, o il sito web."
            Case 160155
                Return "Importazione di merci, ricette, menu o da CALCMENU Pro, CALCMENU Enterprise, e di altri prodotti EGS."
            Case 160156
                Return "Manutenzione del tasso di cambio di valute diverse"
            Case 160157
                Return "Elimina inutilizzati testi."
            Case 160158
                Return "Formato tutti i testi."
            Case 160159
                Return "Stampa della Lista delle merci"
            Case 160160
                Return "Stampa dei dettagli delle merci"
            Case 160161
                Return "Stampa di più ricette"
            Case 160162
                Return "Stampa della lista delle ricette"
            Case 160163
                Return "Stampa dei dettagli del menu"
            Case 160164
                Return "Menu di ingegneria vi permette di valutare le attuali e future ricetta dei prezzi e del design. Analizza i menu e le singole voci di menu per il raggiungimento ottimale di profitto. Usa Menu Ingegneria di identificare quali voci di menu a discesa o da mantenere il vostro menu."
            Case 160169
                Return "Caricare Menu Carte Elenco"
            Case 160170
                Return "Modificare o salvati in anteprima il menu carte."
            Case 160175
                Return "Modificare, visualizzare in anteprima o stampare salvato liste della spesa."
            Case 160177
                Return "Sicurezza"
            Case 160180
                Return "Standardizzare il formato degli elementi"
            Case 160181
                Return "Elimina voci"
            Case 160182
                Return "Ruolo diritti"
            Case 160184
                Return "TCPOS Export"
            Case 160185
                Return "Le vendite per l'esportazione voce"
            Case 160187
                Return "Crea un nuovo locale ingredienti che può essere utilizzato come ingrediente per le vostre ricette."
            Case 160188
                Return "Visualizza elenco dei marchi salvato"
            Case 160189
                Return "Visualizza elenco di oggetti da acquistare."
            Case 160190
                Return "Crea il tuo menù basato su ricette disponibili nel database."
            Case 160191
                Return "Creare un testo utilizzato per le ricette e menu."
            Case 160200
                Return "Ordinamento per nome"
            Case 160202
                Return "Scegli dall' elenco"
            Case 160209
                Return "Si prega di inserire il Numero di Serie, L'Intestazione ed il codice ID del prodotto. Troverete queste informazioni con la documentazione fornita con %s."
            Case 160210
                Return "Cerco oggetti"
            Case 160211
                Return "Oggetti indesiderati"
            Case 160212
                Return "Bozze"
            Case 160217
                Return "Percorso archivi"
            Case 160218
                Return "Importazione di dati con errori Merchandise"
            Case 160219
                Return "In attesa della lista delle merci che deve essere fissato"
            Case 160220
                Return "Definire le opzioni per l'importazione della ingredienti"
            Case 160232
                Return "Esportazione inventario  in"
            Case 160237
                Return "Grassetto"
            Case 160254
                Return "Si prega di riavviare il servizio finestre% n per le modifiche abbiano effetto."
            Case 160258
                Return "Valuta non coincide con la scelta di prezzo."
            Case 160259
                Return "Nome o numero già esistente."
            Case 160260
                Return "Data importati"
            Case 160262
                Return "I valori nutrizionali sono per una porzione al 100%"
            Case 160292
                Return "Allergeni"
            Case 160293
                Return "Elenco delle allergie alimentari o sensibilità associata ad una ingredienti."
            Case 160295
                Return "Questo account è attualmente in uso. Si prega di riprovare più tardi."
            Case 160353
                Return "Prezzo d'acquisto Set di"
            Case 160354
                Return "Set di prezzi di vendita"
            Case 160414
                Return "Qtà Inventario" & vbCrLf & "Precedente"
            Case 160423
                Return "Programma per la gestione di ricette/menu standalone"
            Case 160433
                Return "Da consumarsi entro"
            Case 160500
                Return "Testo di gestione"
            Case 160687
                Return "Voce alternata a colori"
            Case 160688
                Return "Voce colore normale"
            Case 160690
                Return "Si prega di notare che quando si esegue il ripristino, essa verrà automaticamente cut-off attualmente gli utenti che utilizzano il sistema."
            Case 160691
                Return "Backup / Restore Immagini"
            Case 160716
                Return "Set di oggetti globali di default"
            Case 160774
                Return "Disattiva"
            Case 160775
                Return "Rimuovere trailing zeri"
            Case 160776
                Return "Torna a% s"
            Case 160777
                Return "Clicca qui per saperne di più su CALCMENU."
            Case 160788
                Return "Elemento selezionato (s) è stato attivato."
            Case 160789
                Return "Elemento selezionato (s) è stato disattivato."
            Case 160790
                Return "Sei sicuro di voler rimuovere l'elemento selezionato (s)?"
            Case 160791
                Return "Elemento selezionato (s) è stato rimosso con successo."
            Case 160801
                Return "È possibile unire due o più ricette simili."
            Case 160802
                Return "Sei sicuro di voler unire le voci selezionate?"
            Case 160803
                Return "Sei sicuro di voler eliminare gli oggetti?"
            Case 160804
                Return "Si prega di compilare tutti i campi richiesti."
            Case 160805
                Return "Selezionare due o più elementi di unione."
            Case 160806
                Return "Sei sicuro di voler disattivare la voce selezionata (s)?"
            Case 160863
                Return "Merchandise Listino"
            Case 160880
                Return "Ricalcola"
            Case 160894
                Return "Argento"
            Case 160940
                Return "Data di effettività"
            Case 160941
                Return "Collegato punto di vendita"
            Case 160953
                Return "Fattore di vendita Set di Prezzo di Acquisto Set di Prezzo"
            Case 160958
                Return "Lavora con punto di vendita con più insiemi di prezzi di vendita."
            Case 160985
                Return "Non collegato punto di vendita"
            Case 160987
                Return "Crea vendita oggetti e link a ricette già esistenti."
            Case 160988
                Return "Punto di vendita è usato in vendita e di solito è legato a una ricetta."
            Case 161028
                Return "Sei sicuro di voler modificare il database dei nutrienti? Questa azione sarà cambiare la definizione di elementi nutritivi che si sono già fissati nella vostra ingredienti."
            Case 161029
                Return "Ciascuna delle rese o Ingredienti casella di controllo deve essere selezionata."
            Case 161049
                Return "Forza soppressione delle parole chiave e la sua sub-parole chiave"
            Case 161050
                Return "Soppresso le parole chiave saranno anche unassigned merci da / ricetta / le voci del menu."
            Case 161051
                Return "Le parole chiave selezionate e di tutti i suoi sotto-parole chiave sono eliminati. Soppresso le parole chiave sono ora anche da unassigned ingredienti, ricetta, e le voci del menu."
            Case 161078
                Return "Esatto"
            Case 161079
                Return "Inizia con"
            Case 161080
                Return "Contiene"
            Case 161082
                Return "Seconda"
            Case 161083
                Return "Terzo"
            Case 161084
                Return "Quarta"
            Case 161085
                Return "Una sola volta"
            Case 161086
                Return "Giornaliero"
            Case 161087
                Return "Settimanale"
            Case 161088
                Return "Mensile"
            Case 161089
                Return "Quando il cambiamento dei file"
            Case 161090
                Return "Quando il computer si avvia"
            Case 161091
                Return "Inserisci% s informazioni"
            Case 161092
                Return "Fornitore Gruppo"
            Case 161093
                Return "Informazioni per la fatturazione"
            Case 161094
                Return "Data di inizio"
            Case 161095
                Return "del mese"
            Case 161096
                Return "POS Import - Impossibile dati"
            Case 161097
                Return "Organizzare e gestire le informazioni dei vostri fornitori, compresi i contatti, gli indirizzi, i termini di pagamento, ecc per facilitare il processo di ordinazione."
            Case 161098
                Return "Terminal si riferisce alle emittenti del vostro POS che sono collegati al tuo CALCMENU web. Aggiungere, modificare o cancellare i terminali a questo programma."
            Case 161099
                Return "Configurare il POS parametri di importazione. Imposta il calendario, l'ubicazione del file di importazione, ecc"
            Case 161100
                Return "Prodotti di magazzino e gli oggetti sono conservati e distribuiti in luoghi diversi durante diverse volte. Mantenere il controllo per stabilire le possibili luoghi in cui i vostri prodotti possono essere trovati in qualsiasi momento."
            Case 161101
                Return "I clienti sono aziende che acquistano prodotti o prodotti finiti. Gestisci i tuoi clienti a questo programma."
            Case 161102
                Return "Cliente contatti sono le persone che si ha a che fare con una società. Creare, modificare ed eliminare contatti cliente."
            Case 161103
                Return "Fix POS dati che non sono correttamente importati nel sistema."
            Case 161104
                Return "Questo si riferisce al tipo di operazione di emissione da forniture. Questo può essere o non sono stati effettivamente venduti a clienti quali benefici per i dipendenti o giveaways."
            Case 161105
                Return "Storia di vendita rapidamente mostra un elenco di operazioni di vendita e le vendite voce coinvolti"
            Case 161106
                Return "Contrassegnato oggetti"
            Case 161107
                Return "Rendimento computerizzata"
            Case 161132
                Return "Visualizza il mio Ricette"
            Case 161147
                Return "Gestione di ricette e menu"
            Case 161162
                Return "TCPOS"
            Case 161180
                Return "Definire la configurazione automatica di caricamento"
            Case 161181
                Return "Nome host"
            Case 161275
                Return "Orientamento Daily Importi"
            Case 161276
                Return "GDA"
            Case 161279
                Return "Senza"
            Case 161281
                Return "Potenza Cook"
            Case 161282
                Return "Propery Admin"
            Case 161283
                Return "System Admin"
            Case 161284
                Return "Azienda Chef"
            Case 161285
                Return "Propery Chef"
            Case 161286
                Return "Cuoco"
            Case 161287
                Return "Valutazione"
            Case 161288
                Return "Chef del sito"
            Case 161289
                Return "Site Admin"
            Case 161290
                Return "Visualizzazione e stampa"
            Case 161291
                Return "Non definito"
            Case 161292
                Return "Definito"
            Case 161294
                Return "Indesiderati% s"
            Case 161300
                Return "Main Acquisti Set di Prezzo"
            Case 161333
                Return "Didascalie"
            Case 161334
                Return "Ricette %x-%y di %z"
            Case 161468
                Return "Validazione per tutti"
            Case 161484
                Return "Temperatura"
            Case 161485
                Return "Produzione" & vbCrLf & "Data"
            Case 161486
                Return "Consumi" & vbCrLf & "Data"
            Case 161487
                Return "Daily Prodotto"
            Case 161488
                Return "Prima di consumarli"
            Case 161489
                Return "Fresco godere appena preparati"
            Case 161490
                Return "Info Allergie; contiene:"
            Case 161491
                Return "Assegnato a tutti contrassegnati"
            Case 161494
                Return "al max. 5 ° C"
            Case 161538
                Return "Compilate il modulo che trovate sotto."
            Case 161554
                Return "Compilate il modulo che trovate sotto."
            Case 161576
                Return "Prezzo unità"
            Case 161577
                Return "Ore"
            Case 161578
                Return "Costo delle merci"
            Case 161579
                Return "calcolo"
            Case 161580
                Return "Costo delle merci"
            Case 161581
                Return "Iva"
            Case 161582
                Return "Margine Lordo in Fr."
            Case 161583
                Return "Margine Lordo in %"
            Case 161584
                Return "Unità"
            Case 161585
                Return "Prezzo/" & vbCrLf & "Unità"
            Case 161710
                Return "Modello"
            Case 161766
                Return "Piccola porzione"
            Case 161767
                Return "Grandi porzione"
            Case 161777
                Return "Unassign parola chiave"
            Case 161778
                Return "Assegna / unassign parole chiave"
            Case 161779
                Return "Pangrattato"
            Case 161780
                Return "Monitor Breadcrumbs"
            Case 161781
                Return "Indesiderato parole chiave"
            Case 161782
                Return "Stampa etichette"
            Case 161783
                Return "Modello procedura"
            Case 161784
                Return "Studente"
            Case 161785
                Return "Ingrediente valori nutrizionali per% s"
            Case 161786
                Return "Ingrediente valori nutrizionali per 100g/ml"
            Case 161787
                Return "Utilizza modello"
            Case 161788
                Return "Destinazione / derivati Parole chiave"
            Case 161823
                Return "Aggiungi Row (s)"
            Case 161824
                Return "Incolla dal clipboard"
            Case 161825
                Return "Non vi è una ingredienti che deve essere collegato."
            Case 161826
                Return "Scegli un altro"
            Case 161827
                Return "Default Prezzo / Unità:"
            Case 161828
                Return "Scegli tra le unità esistenti"
            Case 161829
                Return "Aggiungi questo come una nuova unità"
            Case 161830
                Return "Voce convalidati"
            Case 161831
                Return "Vorrei modificare la ingredienti prima di aggiungere"
            Case 161832
                Return "posto di% s per completare"
            Case 161834
                Return "Si prega di controllare i prezzi"
            Case 161835
                Return "Tagliare"
            Case 161837
                Return "Aggiungi alla ricetta"
            Case 161838
                Return "Sostituire ingredienti esistenti"
            Case 161839
                Return "N. ingredienti trovati"
            Case 161840
                Return ""
            Case 161841
                Return "Link ad una ingredienti o sub-ricetta"
            Case 161842
                Return "Tutti gli articoli sono ora collegati ad una ingredienti / sub-ricetta"
            Case 161843
                Return "Il punto è ora legata alla ingredienti / sub-ricetta"
            Case 161844
                Return "Memorizzazione di Tempo"
            Case 161845
                Return "Temperatura Stoccaggio"
            Case 161851
                Return "Può essere ordinato"
            Case 161852
                Return "La ricetta può contenere allergeni"
            Case 161853
                Return "Incolla"
            Case 161855
                Return "Bozza"
            Case 161873
                Return "Disconnetti"
            Case 161899
                Return ""
            Case 161902
                Return ""
            Case 161955
                Return ""
            Case 161956
                Return ""
            Case 161970
                Return ""
            Case 161986
                Return "Aggiungi passaggio"
            Case 161987
                Return "Voce% n% di p"
            Case 161988
                Return "Prodotti collegati"
            Case 161989
                Return "Non collegato Prodotti"
            Case 162032
                Return ""
            Case 162039
                Return ""
            Case 162054
                Return ""
            Case 162057
                Return "Non è possibile lasciare vuoto il numero dell'ordine."
            Case 162061
                Return ""
            Case 162062
                Return ""
            Case 162102
                Return ""
            Case 162198
                Return "La resa è stata cambiata. Fare clic sul pulsante Calcola per ridimensionare ingrediente quantità."
            Case 162199
                Return "La resa è stata cambiata. Vuoi continuare a salvare senza calcolare le quantità degli ingredienti?"
            Case 162203
                Return "Informazioni"
            Case 162205
                Return "Numero delle offerte"
            Case 162208
                Return "Business Weekly Giorni"
            Case 162211
                Return "Scegli la lingua"
            Case 162212
                Return "Nome azienda"
            Case 162213
                Return "Numero Business"
            Case 162214
                Return "Prezzo disponibili"
            Case 162215
                Return "Logo per il carico del server"
            Case 162216
                Return "Preferenze"
            Case 162219
                Return "Back Office"
            Case 162221
                Return "Configurazione generale"
            Case 162222
                Return "Inserisci qui"
            Case 162230
                Return "Immettere le informazioni di stile"
            Case 162231
                Return "Nome dello stile"
            Case 162232
                Return "Intestazione stile opzioni"
            Case 162235
                Return "Forse cercavi"
            Case 162257
                Return "Data ultima modifica"
            Case 162276
                Return "Importazione di Ricette/Menu"
            Case 162282
                Return "Note"
            Case 162314
                Return "Produttore"
            Case 162318
                Return "Alcool"
            Case 162319
                Return "Annata"
            Case 162338
                Return "Tipo di vino"
            Case 162340
                Return "Street"
            Case 162341
                Return "Luogo"
            Case 162357
                Return "Esempio"
            Case 162358
                Return "Tenere Lunghezza del Prefisso"
            Case 162361
                Return "Tab."
            Case 162362
                Return "Pipe"
            Case 162363
                Return "Semi-colon"
            Case 162364
                Return "Spazio"
            Case 162382
                Return "Approvato"
            Case 162383
                Return "Appro."
            Case 162386
                Return "Andare"
            Case 162387
                Return "Hi approvazione, Avete ricevuto una ricetta per l'approvazione. [Nome del creatore della voce] ha presentato questa ricetta: [...] Si prega di accedere al sito Web CALCMENU di esaminare e approvare la ricetta. Cordiali saluti, EGS Team"
            Case 162388
                Return "Ciao, La tua ricetta appena creato è stato inviato per l'approvazione. La ricetta sarà esaminato e approvato prima di poter essere utilizzato online. È stato presentato questa ricetta: [...] Una volta approvata, la ricetta sarà disponibile on-line. Cordiali saluti, EGS Team"
            Case 162389
                Return "Hi approvazione, Lei ha approvato questa ricetta: [...] La ricetta sarà disponibile on-line. Cordiali saluti, EGS Team"
            Case 162390
                Return "Ciao, [...] La ricetta è stata approvata. È ora possibile utilizzare questa ricetta on-line. Cordiali saluti, EGS Team"
            Case 162455
                Return "Accesso"
            Case 162485
                Return ""
            Case 162530
                Return "Rimuovere pangrattato su login"
            Case 162596
                Return ""
            Case 162631
                Return "Dimenticato la password?"
            Case 162632
                Return ""
            Case 162635
                Return "Rispondi a queste domande per ricevere la password."
            Case 162636
                Return "Domanda"
            Case 162637
                Return "Risposta"
            Case 162638
                Return ""
            Case 162742
                Return ""
            Case 162747
                Return "Ultima modifica:"
            Case 162888
                Return ""
            Case 162955
                Return "Margine netto in%"
            Case 163032
                Return "Copia Listino"
            Case 163046
                Return "Siamo spiacenti, le parole chiave% k% n% u non trovato. Si prega di premere il tasto 'Sfoglia le parole chiave' per selezionare le parole chiave disponibili."
            Case 163057
                Return "Costo totale per% s"
            Case 163058
                Return "Costo per 1% s"
            Case 163060
                Return "Costo alimentare in% s"
            Case 163061
                Return "Costo imposto alimentare in% s"
            Case 167272
                Return "Particolari del prodotto"
            Case 167346
                Return "Mostri tutti"
            Case 167385
                Return "Sottotitolo"
            Case 167469
                Return "Bene di Nota"
            Case 167719
                Return "Preventivo"
            Case 168373
                Return "Utilizzate 'On line'"
            Case 168374
                Return ""
            Case 168375
                Return ""
            Case 169310
                Return "Sviluppo"
            Case 169318
                Return "Feedback"
            Case 170155
                Return ""
            Case 170253
                Return "Vedi PDF"
            Case 170283
                Return ""
            Case 170668
                Return ""
            Case 170674
                Return "Accedi senza login"
            Case 170675
                Return ""
            Case 170770
                Return "Numero porzioni"
            Case 170779
                Return "Lista ingredienti"
            Case 170780
                Return "I dettagli degli ingredienti"
            Case 170781
                Return "Lista dei valori nutrizionali degli ingredienti"
            Case 170782
                Return "Categorie degli ingredienti"
            Case 170783
                Return "Parole chiave degli ingredienti"
            Case 170784
                Return "Ingredienti pubblicati sul Web"
            Case 170785
                Return "Ingredienti non pubblicati sul Web"
            Case 170786
                Return "Costo degli ingredienti"
            Case 170801
                Return "Composizione finale"
            Case 170849
                Return ""
            Case 170850
                Return ""
            Case 170851
                Return ""
            Case 170852
                Return ""
            Case 170853
                Return ""
            Case 170854
                Return ""
            Case 170855
                Return ""
            Case 170856
                Return ""
            Case 170857
                Return ""
            Case 170858
                Return ""
            Case 170859
                Return ""
            Case 170860
                Return ""
            Case 171014
                Return ""
            Case 171219
                Return ""
            Case 171220
                Return "Numero di porzioni"
            Case 171221
                Return "Totale porzioni"
            Case 171231
                Return "Scarica i caratteri per i codici a barra"
            Case 171232
                Return ""
            Case 171233
                Return ""
            Case 171234
                Return "Protetto"
            Case 171235
                Return "Calcolo automatico"
            Case 171236
                Return ""
            Case 171237
                Return "Vedi dimensione attuale"
            Case 171238
                Return "Non utilizzato online"
            Case 171240
                Return "Articoli non salvati"
            Case 171241
                Return ""
            Case 171242
                Return ""
            Case 171243
                Return ""
            Case 171244
                Return ""
            Case 171245
                Return ""
            Case 171246
                Return ""
            Case 171249
                Return "%s esiste già."
            Case 171301
                Return "Metodo di preparazione"
            Case 171302
                Return "Consiglio"
            Case 171345
                Return ""
            Case 171346
                Return ""
            Case 171347
                Return ""
            Case 171348
                Return ""
            Case 171352
                Return "Nome utente o email non validi"
            Case 171353
                Return "Per recuperare la tua password inserisci il tuo nome utente o l'indirizzo email"
            Case 171354
                Return "Inserisci il nome utente o indirizzo email"
            Case 171371
                Return "Visualizza tutti"
            Case 171372
                Return "Visualizza meno"
            Case 171373
                Return "Salva la ricetta prima."
            Case 171399
                Return "Chiosco per %CM"
            Case 171401
                Return "Le ricette visibili nel Chiosco sono state create da %CM."
            Case 171402
                Return "Condividi questa ricetta con %p"
            Case 171425
                Return "Powered by"
            Case 171428
                Return "Parametri non validi. Contattate chi vi ha inviato la ricetta o il team di supporto di CALCMENU Cloud."
            Case 171429
                Return "Il collegamento a questa ricetta/gruppo è scaduto: Contattate il team di supporto di CALCMENU Cloud o chi vi ha inviato la ricetta."
            Case 171447
                Return "La tua email/smtp non è stata configurata. Configura la tua email nel menu di configurazione prima di utilizzare questa funzione."
            Case 171453
                Return "Impossibile inviare la mail."
            Case 171501
                Return "Se non lo sapete inviateci una mail con il numero di serie e codici prodotto del vostro CALCMENU."
            Case 171502
                Return "Utilizzate i dati di login del sito EGS associati al vostro al vostro codice e numero di serie di CALCMENU."
            Case 171505
                Return "Questa ricetta è inserita in CALCMENU. Visitate %link per saperne di più."
            Case 171506
                Return "Utilizzate le credenziali del sito WEB di EGS per accedere."
            Case 171507
                Return "Dimenticato nome utente e password?"
            Case 171555
                Return ""
            Case 171557
                Return ""
            Case 171558
                Return ""
            Case 171559
                Return ""
            Case 171560
                Return ""
            Case 171561
                Return ""
            Case 171586
                Return ""
            Case 171588
                Return ""
            Case 171589
                Return ""
            Case 171591
                Return ""
            Case 171592
                Return ""
            Case 171593
                Return ""
            Case 171594
                Return ""
            Case 171595
                Return ""
            Case 171596
                Return ""
            Case 171597
                Return ""
            Case 171598
                Return ""
            Case 171599
                Return ""
            Case 171600
                Return ""
            Case 171601
                Return ""
            Case 171602
                Return ""
            Case 171605
                Return ""
            Case 171611
                Return ""
            Case 171612
                Return ""
            Case 171614
                Return ""
            Case 171615
                Return ""
            Case 171616
                Return ""
            Case 171617
                Return ""
            Case 171618
                Return ""
            Case 171619
                Return ""
            Case 171620
                Return ""
            Case 171621
                Return ""
            Case 171622
                Return "N° di Articoli %c"
            Case 171628
                Return ""
            Case 171631
                Return ""
            Case 171649
                Return ""
            Case 171650
                Return ""
            Case 171651
                Return ""
            Case 171652
                Return ""
            Case 171653
                Return ""
            Case 171654
                Return ""
            Case 171655
                Return ""
            Case 171656
                Return ""
            Case 171657
                Return ""
            Case 171658
                Return ""
            Case 171662
                Return ""
            Case 171663
                Return ""
            Case 171664
                Return ""
            Case 171665
                Return ""
            Case 171666
                Return ""
            Case 171667
                Return ""
            Case 171668
                Return ""
            Case 171669
                Return ""
            Case 171670
                Return ""
            Case 171671
                Return ""
            Case 171672
                Return ""
            Case 171673
                Return ""
            Case 171674
                Return ""
            Case 171675
                Return ""
            Case 171676
                Return ""
            Case 171677
                Return ""
            Case 171678
                Return ""
            Case 171679
                Return ""
            Case 171680
                Return ""
            Case 171681
                Return ""
            Case 171682
                Return ""
            Case 171683
                Return ""
            Case 171684
                Return ""
            Case 171685
                Return ""
            Case 171686
                Return ""
            Case 171687
                Return ""
            Case 171688
                Return ""
            Case 171689
                Return ""
            Case 171690
                Return ""
            Case 171691
                Return ""
            Case 171692
                Return ""
            Case 171693
                Return ""
            Case 171694
                Return ""
            Case 171696
                Return ""
            Case 171697
                Return ""
            Case 171698
                Return ""
            Case 171699
                Return ""
            Case 171700
                Return ""
            Case 171701
                Return ""
            Case 171702
                Return ""
            Case 171703
                Return ""
            Case 171704
                Return ""
            Case 171705
                Return ""
            Case 171706
                Return ""
            Case 171707
                Return ""
            Case 171708
                Return ""
            Case 171709
                Return ""
            Case 171710
                Return ""
            Case 171711
                Return ""
            Case 171712
                Return ""
            Case 171713
                Return ""
            Case 171714
                Return ""
            Case 171715
                Return ""
            Case 171716
                Return ""
            Case 171717
                Return ""
            Case 171718
                Return ""
            Case 171719
                Return ""
            Case 171720
                Return ""
            Case 171721
                Return ""
            Case 171722
                Return ""
            Case 171723
                Return ""
            Case 171724
                Return ""
            Case 171725
                Return ""
            Case 171726
                Return ""
            Case 171727
                Return ""
            Case 171728
                Return ""
            Case 171729
                Return ""
            Case 171730
                Return ""
            Case 171731
                Return ""
            Case 171732
                Return ""
            Case 171733
                Return ""
            Case 171734
                Return ""
            Case 171735
                Return ""
            Case 171736
                Return ""
            Case 171737
                Return ""
            Case 171738
                Return ""
            Case 171739
                Return ""
            Case 171740
                Return ""
            Case 171741
                Return ""
            Case 171742
                Return ""
            Case 171743
                Return ""
            Case 171744
                Return ""
            Case 171745
                Return ""
            Case 171746
                Return ""
            Case 171747
                Return ""
            Case 171748
                Return ""
            Case 171749
                Return ""
            Case 171750
                Return ""
            Case 171751
                Return ""
            Case 171752
                Return ""
            Case 171753
                Return ""
            Case 171754
                Return ""
            Case 171755
                Return ""
            Case 171756
                Return ""
            Case 171758
                Return ""
            Case 171759
                Return ""
            Case 171760
                Return ""
            Case 171761
                Return ""
            Case 171762
                Return ""
            Case 171763
                Return ""
            Case 171764
                Return ""
            Case 171765
                Return ""
            Case 171767
                Return ""
            Case 171768
                Return ""
            Case 171769
                Return ""
            Case 171770
                Return ""
            Case 171771
                Return ""
            Case 171772
                Return ""
            Case 171773
                Return ""
            Case 171774
                Return ""
            Case 171775
                Return ""
            Case 171776
                Return ""
            Case 171777
                Return ""
            Case 171778
                Return ""
            Case 171779
                Return ""
            Case 171780
                Return ""
            Case 171781
                Return ""
            Case 171782
                Return ""
            Case 171783
                Return ""
            Case 171785
                Return ""
            Case 171786
                Return ""
            Case 176055
                Return "Seleccione por lo menos una Ley de Alimentos"
        End Select
    End Function

 
'dutch
    Public Function FTBLow19USA(ByVal Code As Integer) As String
        Select Case Code
            Case 1080
                Return "Ingrediëntenprijs"
            Case 1081
                Return "Ingrediëntkosten"
            Case 1090
                Return "Verkoopprijs"
            Case 1145
                Return "Teller"
            Case 1260
                Return "Ingrediënten"
            Case 1280
                Return "Opmerking"
            Case 1290
                Return "Prijs"
            Case 1300
                Return "Verspilling"
            Case 1310
                Return "Hoeveelheid"
            Case 1400
                Return "Menu"
            Case 1450
                Return "Categorie"
            Case 1480
                Return "Vastgelegde prijs"
            Case 1485
                Return "Gecalculeerde prijs"
            Case 1500
                Return "Datum"
            Case 1530
                Return "Eenheid mist"
            Case 1600
                Return "Menu wijzigen"
            Case 2430
                Return "&Uit de lijst kiezen"
            Case 2700
                Return "Menulijst printen"
            Case 2780
                Return "Bestellijst"
            Case 3057
                Return "Databank"
            Case 3140
                Return "Voor"
            Case 3150
                Return "Percentage"
            Case 3161
                Return "Const."
            Case 3195
                Return "Recept #"
            Case 3200
                Return "Chef kok"
            Case 3204
                Return "Voornaam"
            Case 3206
                Return "Vertaling"
            Case 3215
                Return "Eenheidsprijs"
            Case 3230
                Return "Afbeelding"
            Case 3234
                Return "Lijst"
            Case 3300
                Return "Menukaart"
            Case 3305
                Return "Referentienaam"
            Case 3306
                Return "Vertegenwoordiger"
            Case 3320
                Return "Wilt u de hoeveelheden aan het nieuwe aantal portie(s) aanpassen?"
            Case 3460
                Return "&Wachtwoord"
            Case 3680
                Return "Backup"
            Case 3685
                Return "Backup gecompleteerd"
            Case 3721
                Return "Bron"
            Case 3760
                Return "Importeren"
            Case 3800
                Return "Exporteren"
            Case 4130
                Return "Vrije schijfruimte"
            Case 4185
                Return "Product-ID"
            Case 4755
                Return "Start import"
            Case 4832
                Return "Recept"
            Case 4834
                Return "Recipe Ingredients"
            Case 4854
                Return "Minimum"
            Case 4855
                Return "Maximum"
            Case 4856
                Return "Vanaf"
            Case 4860
                Return "Bestandsnaa"
            Case 4862
                Return "Versie"
            Case 4865
                Return "Gebruikers"
            Case 4867
                Return "Modify"
            Case 4870
                Return "Een gebruiker wijzigen"
            Case 4877
                Return "Gemiddeld"
            Case 4890
                Return "Bestandstype"
            Case 4891
                Return "Voorproef"
            Case 5100
                Return "Eenheid"
            Case 5105
                Return "Formaat"
            Case 5270
                Return "Ingrediëntenlijst"
            Case 5350
                Return "Totaal"
            Case 5390
                Return "portie"
            Case 5500
                Return "Nummer"
            Case 5530
                Return "Vastgelegde verkoopsprijs"
            Case 5590
                Return "Ingrediënten"
            Case 5600
                Return "Voorbereiding"
            Case 5610
                Return "Pagina"
            Case 5720
                Return "Bedrag"
            Case 5741
                Return "Bruto"
            Case 5795
                Return "per portie"
            Case 5801
                Return "Winst"
            Case 5900
                Return "Ingrediënten categorie"
            Case 6000
                Return "Categorie wijzigen"
            Case 6002
                Return "Categorie naam"
            Case 6055
                Return "Tekst toevoegen"
            Case 6390
                Return "Valuta"
            Case 6416
                Return "Factor"
            Case 6470
                Return "Gelieve te wachten"
            Case 7010
                Return "Nee"
            Case 7073
                Return "Browsen"
            Case 7181
                Return "Alles"
            Case 7183
                Return "Gemarkeerd"
            Case 7270
                Return "Engels"
            Case 7296
                Return "Europa"
            Case 7335
                Return "Alle markeringen zijn succesvol verwijderd"
            Case 7570
                Return "Zondag"
            Case 7571
                Return "Maandag"
            Case 7572
                Return "Dinsdag"
            Case 7573
                Return "Woensdag"
            Case 7574
                Return "Donderdag"
            Case 7575
                Return "Vrijdag"
            Case 7576
                Return "Zaterdag"
            Case 7720
                Return "Verpakking"
            Case 7725
                Return "Transport"
            Case 7755
                Return "Systeem"
            Case 8210
                Return "Calculatie"
            Case 8220
                Return "Receptbereiding"
            Case 8395
                Return "Toevoegen"
            Case 8397
                Return "Verwijderen"
            Case 8913
                Return "Geen"
            Case 8914
                Return "Decimalen"
            Case 8994
                Return "Instrumenten"
            Case 9030
                Return "Updaten"
            Case 9070
                Return "Not allowed in the demo version"
            Case 9140
                Return "Switzerland"
            Case 9920
                Return "Beschrijving"
            Case 10103
                Return "Kopiëren"
            Case 10104
                Return "Tekst"
            Case 10109
                Return "Optie's"
            Case 10116
                Return "Bericht"
            Case 10121
                Return "Zoeken"
            Case 10125
                Return "Opmerking"
            Case 10129
                Return "Selectie"
            Case 10130
                Return "On hand"
            Case 10131
                Return "Input"
            Case 10132
                Return "Output"
            Case 10135
                Return "Stijl"
            Case 10140
                Return "Stock"
            Case 10363
                Return "Belasting"
            Case 10369
                Return "Leveranciersnummer"
            Case 10370
                Return "In bestelling"
            Case 10399
                Return "Verwijderd"
            Case 10417
                Return "Gefaald:"
            Case 10430
                Return "Location"
            Case 10431
                Return "Voorraad"
            Case 10468
                Return "Status"
            Case 10513
                Return "Korting"
            Case 10523
                Return "Tel."
            Case 10524
                Return "Fax"
            Case 10554
                Return "CCP-beschrijving"
            Case 10555
                Return "Afkoeltijd"
            Case 10556
                Return "Verwarmingstijd"
            Case 10557
                Return "Verwarmingsgraad/temperatuur"
            Case 10558
                Return "Verwarmingswijze"
            Case 10572
                Return "Voedingswaarde"
            Case 10573
                Return "Info"
            Case 10970
                Return "Printen"
            Case 10990
                Return "Leverancier"
            Case 11040
                Return "Restore completed"
            Case 11280
                Return "Registratie"
            Case 12515
                Return "Barcode"
            Case 12525
                Return "Ongeldige datum"
            Case 13060
                Return "Voedingswaardes"
            Case 13255
                Return "Historie"
            Case 14070
                Return "Lettertype"
            Case 14090
                Return "Titel"
            Case 14816
                Return "Vervangen door"
            Case 14819
                Return "Vervangen"
            Case 14884
                Return "Geupdate items"
            Case 15360
                Return "Gemarkeerde menu's"
            Case 15504
                Return "Administrator"
            Case 15510
                Return "Wachtwoord"
            Case 15615
                Return "Voer uw wachtwoord in"
            Case 15620
                Return "Confirmation"
            Case 16010
                Return "Calculatie"
            Case 18460
                Return "Opslaan in bewerking"
            Case 20122
                Return "Bedrijf"
            Case 20200
                Return "Sub-recept"
            Case 20469
                Return "Specificeer de mailing methode"
            Case 20530
                Return "Energie"
            Case 20703
                Return "Algemeen"
            Case 20709
                Return "Eenheden"
            Case 21570
                Return "Een fax formulier printen"
            Case 21600
                Return "van"
            Case 24002
                Return "Laatste bestelling"
            Case 24016
                Return "Leverancier"
            Case 24027
                Return "Calculeren"
            Case 24028
                Return "Annuleren"
            Case 24044
                Return "Beide"
            Case 24050
                Return "Nieuw"
            Case 24085
                Return "Nieuwe toewijzing"
            Case 24105
                Return "Tonen"
            Case 24121
                Return "Afk."
            Case 24129
                Return "Uitgifte"
            Case 24150
                Return "Wijzigen"
            Case 24152
                Return "Functie"
            Case 24153
                Return "Stad"
            Case 24163
                Return "Default location"
            Case 24260
                Return "Deze leverancier kan niet worden verwijderd"
            Case 24270
                Return "Terug"
            Case 24271
                Return "Volgende"
            Case 24291
                Return "Subtotaal"
            Case 26000
                Return "Doorgaan"
            Case 26100
                Return "Product beschrijving"
            Case 26101
                Return "Kooktip/advies"
            Case 26102
                Return "Raffinement"
            Case 26103
                Return "Opslag"
            Case 26104
                Return "Opbrengst/Productiviteit"
            Case 27000
                Return "Referentienaam"
            Case 27020
                Return "Adres"
            Case 27050
                Return "Telefoonnummer"
            Case 27055
                Return "Koptekst naam"
            Case 27130
                Return "Betaling"
            Case 27135
                Return "Verloopdatum"
            Case 28000
                Return "Fout in bewerking"
            Case 28008
                Return "Ongeldige directory"
            Case 28655
                Return "Er is geen eenheid gedefinieerd"
            Case 29170
                Return "Niet beschikbaar"
            Case 29771
                Return "Ingrediënten wijzigen"
            Case 30210
                Return "De bewerking is gefaald"
            Case 30270
                Return "not found"
            Case 31085
                Return "Update succesvol"
            Case 31098
                Return "Opslaan"
            Case 31370
                Return "Ingrediënten kostprijs"
            Case 31375
                Return "IK"
            Case 31380
                Return "Algemeen"
            Case 31462
                Return "Fout"
            Case 31492
                Return "Onze fax-service helpdesk verzekert u een antwoord binnen 24 uur, afhankelijk van het geconstateerde probleem (behalve weekenden)"
            Case 31755
                Return "Resultaten"
            Case 31758
                Return "Op"
            Case 31769
                Return "verkocht"
            Case 31800
                Return "Dag"
            Case 31860
                Return "Period"
            Case 51056
                Return "Product"
            Case 51086
                Return "Taal"
            Case 51092
                Return "Eenheid"
            Case 51097
                Return "EGS Enggist && Grandjean Software SA"
            Case 51098
                Return "Route de Soleure 12 / PO BOX"
            Case 51099
                Return "2072 St-Blaise, Switzerland"
            Case 51123
                Return "Details"
            Case 51129
                Return "Gewenste ingrediënten"
            Case 51130
                Return "Niet gewenste ingrediënten"
            Case 51139
                Return "Gewild"
            Case 51157
                Return "Bericht"
            Case 51178
                Return "Gelieve nogmaals proberen"
            Case 51198
                Return "Verbinden met SMTP server"
            Case 51204
                Return "Ja"
            Case 51243
                Return "Marge"
            Case 51244
                Return "Boven"
            Case 51245
                Return "Bottom"
            Case 51246
                Return "Left"
            Case 51247
                Return "Recht"
            Case 51252
                Return "Downloaden"
            Case 51257
                Return "E-mail"
            Case 51259
                Return "SMTP Server"
            Case 51261
                Return "Gebruikersnaam"
            Case 51294
                Return "Opbrengst"
            Case 51311
                Return "Ongeldige eenheid"
            Case 51336
                Return "Ongewenst"
            Case 51353
                Return "Copyright overeenkomst"
            Case 51364
                Return "Accepteert u de bovenstaande copyright overeenkomst en wilt u doorgaan met de vastlegging van het recept?"
            Case 51377
                Return "E-mail verzenden"
            Case 51392
                Return "Opbrengst eenheid"
            Case 51402
                Return "Weet u zeker dat u wilt verwijderen"
            Case 51500
                Return "Bestellijst details"
            Case 51502
                Return "Bestellijst"
            Case 51532
                Return "Bestellijst printen"
            Case 51907
                Return "&Details tonen"
            Case 52012
                Return "Bladeren"
            Case 52110
                Return "Het geselecteerde bestand zal nu worden geïmporteerd"
            Case 52130
                Return "Nieuw recept"
            Case 52150
                Return "Voltooid"
            Case 52307
                Return "Sluiten"
            Case 52960
                Return "Simple"
            Case 52970
                Return "Compleet"
            Case 53250
                Return "Export selectie"
            Case 54210
                Return "Gelieve niets veranderen"
            Case 54220
                Return "Alles in hoofdletters"
            Case 54230
                Return "Alles in kleine letters"
            Case 54240
                Return "Schrijf de eerste letter van elk woord met grote letters"
            Case 54245
                Return "Eerste letter groot geschreven"
            Case 54710
                Return "Geselecteerde sleutelwoorden"
            Case 54730
                Return "Sleutelwoorden"
            Case 55211
                Return "Verbinding"
            Case 55220
                Return "Hoeveelheid"
            Case 56100
                Return "Uw Naam"
            Case 56130
                Return "Land"
            Case 56500
                Return "Woordenboek"
            Case 101600
                Return "Menu wijzigen"
            Case 103150
                Return "Percentage"
            Case 103215
                Return "Eenheidprijs"
            Case 103305
                Return "Referentienaam"
            Case 103306
                Return "Vertegenwoordiger"
            Case 104829
                Return "Leverancierslijst"
            Case 104835
                Return "Een nieuw product creëren"
            Case 104854
                Return "Minimaal"
            Case 104855
                Return "Maximaal"
            Case 104862
                Return "Versie"
            Case 104869
                Return "Nieuwe gebruiker"
            Case 104870
                Return "Een gebruiker wijzigen"
            Case 105100
                Return "Eenheid"
            Case 105110
                Return "Datum"
            Case 105200
                Return "voor"
            Case 105360
                Return "Verkoopprijs per portie"
            Case 106002
                Return "Categorie naam"
            Case 107183
                Return "Gemarkeerd"
            Case 110101
                Return "Wijzigen"
            Case 110102
                Return "Verwijderen"
            Case 110112
                Return "Printen"
            Case 110114
                Return "Help"
            Case 110129
                Return "Selectie"
            Case 110417
                Return "Gefaald:"
            Case 110524
                Return "Fax"
            Case 113275
                Return "Belasting"
            Case 115610
                Return "Nieuw wachtwoord geaccepteerd"
            Case 121600
                Return "van"
            Case 124016
                Return "Leverancier"
            Case 124024
                Return "Goedgekeurd door"
            Case 124042
                Return "Type"
            Case 124257
                Return "Outlet"
            Case 127010
                Return "Bedrijf"
            Case 127040
                Return "Land"
            Case 127050
                Return "Telefoonnummer"
            Case 127055
                Return "Koptekst naam"
            Case 128000
                Return "Fout bij uitvoeren"
            Case 131462
                Return "Fout"
            Case 131757
                Return "Van"
            Case 132552
                Return "Totaal belasting"
            Case 132554
                Return "Recept wijzigen"
            Case 132555
                Return "Recept toevoegen"
            Case 132557
                Return "Een nieuw menu creëren"
            Case 132559
                Return "Een nieuw ingrediënt creëren"
            Case 132561
                Return "Gelieve het serienummer, gebruikersnaam en productsleutel invoeren. U vindt deze informatie verstrekt bij ReceptenNet."
            Case 132565
                Return "Aanvulling"
            Case 132567
                Return "Ingrediëntencategorie"
            Case 132568
                Return "Recept categorie"
            Case 132569
                Return "Menucategorie"
            Case 132570
                Return "Onmogelijk te verwijderen"
            Case 132571
                Return "Categorie is in gebruik"
            Case 132589
                Return "Maximaal aantal recepten"
            Case 132590
                Return "Huidig aantal recepten"
            Case 132592
                Return "Maximaal aantal ingrediënten"
            Case 132593
                Return "Huidig aantal ingrediënten"
            Case 132597
                Return "Een nieuw recept creëren"
            Case 132598
                Return "Maximaal aantal menu's"
            Case 132599
                Return "Huidig aantal menu's"
            Case 132600
                Return "Sleutelwoord toewijzen"
            Case 132601
                Return "Gemarkeerd naar nieuwe categorie verplaatsen"
            Case 132602
                Return "Gemarkeerd verwijderen"
            Case 132605
                Return "Bestellijst"
            Case 132607
                Return "Actie markeringen"
            Case 132614
                Return "Netto hoeveelheid"
            Case 132615
                Return "Rechten"
            Case 132616
                Return "Eigenaar"
            Case 132621
                Return "Bron wijzigen"
            Case 132630
                Return "Automatische conversie"
            Case 132638
                Return "Gebruikersinformatie"
            Case 132640
                Return "Gebruikersnaam is reeds in gebruik"
            Case 132654
                Return "Databank management"
            Case 132657
                Return "&Herstellen"
            Case 132667
                Return "Samenvoegen"
            Case 132668
                Return "Verwijderen"
            Case 132669
                Return "Omhoog verplaatsen"
            Case 132670
                Return "Omlaag verplaatsen"
            Case 132671
                Return "Standaardiseren"
            Case 132678
                Return "HACCP"
            Case 132683
                Return "Vorige"
            Case 132706
                Return "Voedingswaardes zijn per 100g of 100 ml"
            Case 132714
                Return "Selecteer van de lijst."
            Case 132719
                Return "De prijs voor dezelfde eenheid is reeds gedefinieerd."
            Case 132723
                Return "De totale verspilling kan niet groter of gelijk zijn aan 100%"
            Case 132736
                Return "Bruto hoeveelheid"
            Case 132737
                Return "Nieuwe leverancier toevoegen"
            Case 132738
                Return "Leverancier wijzigen"
            Case 132739
                Return "Leveranciersdetails"
            Case 132740
                Return "Staat"
            Case 132741
                Return "URL"
            Case 132779
                Return "Sleutelwoord is in gebruik"
            Case 132783
                Return "Sleutelwoord"
            Case 132788
                Return "Voedingswaarde verbinding"
            Case 132789
                Return "&Login"
            Case 132813
                Return "&Configuratie"
            Case 132828
                Return "Herbereken &Voedingswaarde"
            Case 132841
                Return "Ingrediënten toevoegen"
            Case 132846
                Return "Markeringen opslaan"
            Case 132847
                Return "Markeringen laden"
            Case 132848
                Return "Filter"
            Case 132855
                Return "Menu toevoegen"
            Case 132860
                Return "Ingrediënt toevoegen"
            Case 132864
                Return "Ingrediënten herplaatsen"
            Case 132865
                Return "Separator toevoegen"
            Case 132877
                Return "Item toevoegen"
            Case 132896
                Return "Categorieën standaardiseren"
            Case 132912
                Return "Teksten standaardiseren"
            Case 132915
                Return "Eenheden standaardiseren"
            Case 132924
                Return "Opbrengsteenheden standaardiseren"
            Case 132930
                Return "Proefdruk (Index van kleinafbeeldingen)"
            Case 132933
                Return "Receptenlijst"
            Case 132939
                Return "Menulijst"
            Case 132954
                Return "Markeringsets"
            Case 132955
                Return "Kies een markeringsnaam van de lijst of type een nieuwe markeringsnaam om op te slaan in"
            Case 132957
                Return "Markeren als opslaan"
            Case 132967
                Return "Voedingswaarde"
            Case 132971
                Return "Voedingswaarde overzicht"
            Case 132972
                Return "Voedingswaardes zijn per portie op 100%"
            Case 132974
                Return "Waste"
            Case 132987
                Return "Overzicht"
            Case 132989
                Return "Tonen"
            Case 132997
                Return "op of voor"
            Case 132998
                Return "op of na"
            Case 132999
                Return "tussen"
            Case 133000
                Return "groter dan"
            Case 133001
                Return "minder dan"
            Case 133005
                Return "Vastgelegd"
            Case 133023
                Return "Toon optie's"
            Case 133043
                Return "Locale afbeeldingen transformaties"
            Case 133045
                Return "Maximale afbeelding bestandsgrootte"
            Case 133046
                Return "Maximale afbeeldingsgrootte"
            Case 133047
                Return "Optimalisatie"
            Case 133049
                Return "Automatische afbeeldingenconversie voor gebruik op de website activeren"
            Case 133057
                Return "Logo voor de Website uploaden"
            Case 133060
                Return "Webkleuren"
            Case 133075
                Return "Nieuw wachtwoord"
            Case 133076
                Return "Nieuw wachtwoord bevestigen"
            Case 133080
                Return "Laatste"
            Case 133081
                Return "Allereerst"
            Case 133085
                Return "Document uitvoer"
            Case 133096
                Return "Recept voorbereiding"
            Case 133097
                Return "Recept kostprijs"
            Case 133099
                Return "Variatie"
            Case 133100
                Return "Recept details"
            Case 133101
                Return "Menudetails"
            Case 133108
                Return "Wat moet geprint worden?"
            Case 133109
                Return "Selectie van te printen ingrediënten"
            Case 133111
                Return "Enkele categorieën"
            Case 133112
                Return "Gemarkeerde ingrediënten"
            Case 133116
                Return "Gemarkeerde recepten"
            Case 133121
                Return "Gemarkeerde menu's"
            Case 133123
                Return "Menuprijs"
            Case 133124
                Return "Menubeschrijving"
            Case 133126
                Return "EGS Standaard"
            Case 133127
                Return "EGS modern"
            Case 133128
                Return "EGS Twee Kolommen"
            Case 133133
                Return "Ongeldige bestandsnaam. Gelieve een geldige bestandsnaam in te voeren"
            Case 133144
                Return "Receptnummer"
            Case 133161
                Return "Papiergrootte"
            Case 133162
                Return "Eenheid voor marges"
            Case 133163
                Return "Linker marge"
            Case 133164
                Return "Rechter marge"
            Case 133165
                Return "Boven marge"
            Case 133166
                Return "Benedenmarge"
            Case 133168
                Return "Lettergrootte"
            Case 133172
                Return "Kleine Afbeelding / Hoeveelheid - Naam"
            Case 133173
                Return "Kleine Afbeelding / Naam - Hoeveelheid"
            Case 133174
                Return "Middelgrote Afbeelding / Hoeveelheid - Naam"
            Case 133175
                Return "Middelgrote Afbeelding / Naam - Hoeveelheid"
            Case 133176
                Return "Grote Afbeelding / Hoeveelheid - Naam"
            Case 133177
                Return "Grote Afbeelding / Naam - Hoeveelheid"
            Case 133196
                Return "Optielijst"
            Case 133201
                Return "De volgende ingrediënten zijn in gebruik en zijn niet verwijderd"
            Case 133207
                Return "Recept kan worden gebruikt als sub-recept"
            Case 133208
                Return "Gewicht"
            Case 133222
                Return "Detailopties"
            Case 133230
                Return "De volgende recept(en) zijn in gebruik en zijn niet verwijderd"
            Case 133241
                Return "Herberekening prijzen. Een moment geduld….."
            Case 133242
                Return "Herberekening voedingswaardes. Een moment geduld…."
            Case 133251
                Return "Separator"
            Case 133254
                Return "Sorteren door"
            Case 133260
                Return "Bron is in gebruik"
            Case 133266
                Return "Sleutelwoorden standaardiseren"
            Case 133286
                Return "Definitie"
            Case 133289
                Return "Eenheid is in gebruik"
            Case 133290
                Return "U kunt niet twee of meer systeemeenheden samenvoegen."
            Case 133295
                Return "Deze eenheid kan niet worden verwijderd.. ¶Alleen gebruikersgedefinieerde eenheden kunnen worden verwijderd."
            Case 133314
                Return "Alleen gebruikers-gedefinieerde opbrengsteenheden kunnen worden verwijderd"
            Case 133315
                Return "U kunt niet twee of meer systeem opbrengsteenheden samenvoegen"
            Case 133319
                Return "Opbrengsteenheid in gebruik"
            Case 133325
                Return "Weet u zeker dat u alle ongebruikte categorieën wilt verwijderen?"
            Case 133326
                Return "Geen bron"
            Case 133330
                Return "Ontbrekend bestand"
            Case 133349
                Return "Menunummer"
            Case 133350
                Return "Items voor %y (netto hoeveelheid)"
            Case 133351
                Return "Ingrediënten voor %y" ' in %p% (netto hoeveelheid)"
            Case 133352
                Return "Vastgelegde verkoopsprijs per portie + belasting"
            Case 133353
                Return "Vastgelegde verkoopsprijs per portie"
            Case 133359
                Return "Sorteren door nummer"
            Case 133360
                Return "Sorteren door datum"
            Case 133361
                Return "Sorteren door categorie"
            Case 133365
                Return "Verkoopprijs + belasting"
            Case 133367
                Return "Sorteren door leverancier"
            Case 133405
                Return "Upload Digital assets" '"Afbeeldingen uploaden"
            Case 133519
                Return "Een kleur selecteren:"
            Case 133692
                Return "Aanbevolen prijs"
            Case 134032
                Return "Contact"
            Case 134055
                Return "Inkopend"
            Case 134056
                Return "Verkopen"
            Case 134061
                Return "Versie, modulen & licenties"
            Case 134083
                Return "Test"
            Case 134111
                Return "Onmogelijk om gemarkeerde items te verwijderen"
            Case 134176
                Return "Ingrediënt-voedingswaarde lijst"
            Case 134177
                Return "Recept-Voedingswaarde lijst"
            Case 134178
                Return "Menu-voedingswaarde lijst"
            Case 134182
                Return "Groep"
            Case 134194
                Return "Ongeldige hoeveelheid"
            Case 134195
                Return "Ongeldige prijs"
            Case 134320
                Return "Factuur adres"
            Case 134332
                Return "Informatie"
            Case 134333
                Return "Belangrijk"
            Case 134525
                Return "Weet u zeker dat u de gemaakte wijzigingen wilt annuleren?"
            Case 134571
                Return "Ongeldige waarde"
            Case 135056
                Return "Voedingswaarde"
            Case 135058
                Return "Voedingswaarde toevoegen"
            Case 135059
                Return "Voedingswaarde wijzigen"
            Case 135070
                Return "Netto"
            Case 135256
                Return "Hoeveelheid verkocht"
            Case 135608
                Return "Poort"
            Case 135948
                Return "Sub-recept(en) toevoegen"
            Case 135955
                Return "Ongeldige nummerieke waarde"
            Case 135963
                Return "Databank"
            Case 135967
                Return "In recepten herplaatsen"
            Case 135968
                Return "In menu's herplaatsen"
            Case 135971
                Return "&Connectie"
            Case 135978
                Return "Nieuw"
            Case 135979
                Return "Hernoemen"
            Case 135985
                Return "Bestaand"
            Case 135986
                Return "Mist"
            Case 135989
                Return "Items"
            Case 135990
                Return "Verversen"
            Case 136018
                Return "Bezit"
            Case 136025
                Return "Databank conversie"
            Case 136171
                Return "Eenheid wijzigen"
            Case 136265
                Return "Sub-recepten"
            Case 136601
                Return "Terug zetten"
            Case 136905
                Return "Valuta symbool"
            Case 137019
                Return "Veranderen"
            Case 137030
                Return "Standaard"
            Case 137070
                Return "Algemene instellingen"
            Case 138137
                Return "Verwijderd"
            Case 138244
                Return "Verkoopitem"
            Case 138402
                Return "Alle uitgiftes succesvol gedaan"
            Case 138412
                Return "<niet gedefinieerd>"
            Case 140056
                Return "Bestand"
            Case 140100
                Return "Backup in bewerking"
            Case 140101
                Return "Restore in bewerking"
            Case 140129
                Return "Fout tijdens restore van backup"
            Case 140130
                Return "Fout tijdens creatie van backup"
            Case 140180
                Return "Pad om backup bestanden op te slaan"
            Case 143001
                Return "Delen"
            Case 143002
                Return "Niet meer delen"
            Case 143008
                Return "Verspilling"
            Case 143013
                Return "Wijziging"
            Case 143014
                Return "Gebruiker"
            Case 143508
                Return "Recept is in gebruik als sub-recept"
            Case 143509
                Return "Regelafstand"
            Case 143987
                Return "Itemtype"
            Case 143995
                Return "Actie"
            Case 144591
                Return "Tijd"
            Case 144682
                Return "Voedingswaardes zijn per 100g of 100 ml op 100%"
            Case 144684
                Return "Voedingswaardes zijn per 1 opbrengsteenheid op 100%"
            Case 144685
                Return "per opbrengsteenheid op 100%"
            Case 144686
                Return "per %Y op 100%"
            Case 144687
                Return "per 100g of 100ml op 100%"
            Case 144688
                Return "NB"
            Case 144689
                Return "Voedingswaardes zijn per 1 opbrengsteenheid/100g of 100ml op 100%"
            Case 144716
                Return "Historie"
            Case 144734
                Return "Verkoopitem lijst"
            Case 144738
                Return "Gewicht per %Y"
            Case 145006
                Return "Uitgifte"
            Case 146056
                Return "Contributiemarge"
            Case 146067
                Return "Balans"
            Case 146080
                Return "Klant"
            Case 146114
                Return "Nieuwe pagina tonen bij verschillende leveranciers"
            Case 146211
                Return "Niet-verkoop uitgiften"
            Case 147070
                Return "Ok"
            Case 147075
                Return "Ongeldige datum"
            Case 147126
                Return "Bestaande markeringen eerst verwijderen"
            Case 147174
                Return "Open"
            Case 147441
                Return "Dit verkoopitem is reeds verbonden"
            Case 147462
                Return "Ratio"
            Case 147520
                Return "Algemeen"
            Case 147647
                Return "SQL Server bestaat niet of toegang geweigerd"
            Case 147652
                Return "Verwijderen"
            Case 147692
                Return "Maaltijd informatie"
            Case 147699
                Return "Overschrijven"
            Case 147700
                Return "Totaalprijs"
            Case 147703
                Return "Aantal voorbereide portie's:"
            Case 147704
                Return "Overgebleven opbrengst"
            Case 147706
                Return "Teruggegeven opbrengst"
            Case 147707
                Return "Verloren opbrengst"
            Case 147708
                Return "Verkochte opbrengst"
            Case 147710
                Return "Speciaal verkochte opbrengst"
            Case 147713
                Return "EGS Layout"
            Case 147727
                Return "Kosten"
            Case 147729
                Return "Classificatie"
            Case 147733
                Return "Kies een taal"
            Case 147737
                Return "Type hoeveelheid en selecteer de eenheid"
            Case 147743
                Return "Uploaden"
            Case 147753
                Return "Arbeidskosten"
            Case 147771
                Return "Prijs/uur"
            Case 147772
                Return "Prijs/min"
            Case 147773
                Return "Persoon"
            Case 147774
                Return "Tijd (Uren:Minuten)"
            Case 149501
                Return "Directe Input '-Output gebruiken"
            Case 149513
                Return "Goedkeuring"
            Case 149531
                Return "Eindproducten"
            Case 149645
                Return "Verbonden met"
            Case 149706
                Return "Verbinding verwijderen"
            Case 149766
                Return "Prefix"
            Case 149774
                Return "Wissen"
            Case 150333
                Return "Succesvol verwijderd!"
            Case 150341
                Return "Valuta conversie"
            Case 150353
                Return "Sorteren"
            Case 150634
                Return "E-mail succesvol verzonden"
            Case 150644
                Return "De SMTP server is benodigd voor het versturen van e-mail van uw computer."
            Case 150688
                Return "De licentie voor deze applicatie is reeds verlopen."
            Case 150707
                Return "Rekening"
            Case 151011
                Return "Zwitserland - Hoofdkantoor"
            Case 151019
                Return "Ingrediënten sleutelwoord"
            Case 151020
                Return "Recept sleutelwoord"
            Case 151023
                Return "Registeren"
            Case 151250
                Return "Niets was veranderd"
            Case 151286
                Return "Standard"
            Case 151299
                Return "Gelieve de gevraagde informatie invoeren"
            Case 151322
                Return "In voorraad omvatten"
            Case 151336
                Return "Markeringsset laden"
            Case 151344
                Return "Markeringen voor ingrediënten opslaan"
            Case 151345
                Return "Markeringen voor recepten opslaan"
            Case 151346
                Return "Markeringen voor menu's opslaan"
            Case 151364
                Return "Selecteer een of meerdere teksten"
            Case 151389
                Return "Teksten verwijderen"
            Case 151400
                Return "Ingrediëntenprijs"
            Case 151404
                Return "BTW"
            Case 151424
                Return "Converteer de beste eenheid"
            Case 151427
                Return "Sorteren door item naam"
            Case 151435
                Return "Onderwerp"
            Case 151437
                Return "ReceptenNet"
            Case 151438
                Return "CALCMENU"
            Case 151459
                Return "Uw e-mail"
            Case 151499
                Return "Voorstel herplaatsen"
            Case 151854
                Return "Excel"
            Case 151906
                Return "Email adres niet gevonden"
            Case 151907
                Return "Gelieve in te loggen met uw juiste gebruikersnaam en wachtwoord"
            Case 151910
                Return "Inloggen"
            Case 151911
                Return "Uitloggen"
            Case 151912
                Return "Uw wachtwoord vergeten?"
            Case 151915
                Return "Wilt u de onderstaande informatie aanleveren."
            Case 151916
                Return "Velden met * zijn verplicht."
            Case 151918
                Return "Gelieve een geldig email adres opgeven."
            Case 151976
                Return "Standaard productie locatie"
            Case 152004
                Return "Tree view (overzicht hiërarchische structuur)"
            Case 152141
                Return "Ingrediënten management"
            Case 152146
                Return "Zip"
            Case 155024
                Return "Pictures Management"
            Case 155046
                Return "Vertaling"
            Case 155052
                Return "Voorleggen"
            Case 155118
                Return "Bestellijst naar handheld zenden"
            Case 155163
                Return "Achternaam"
            Case 155170
                Return "Welkom %name!"
            Case 155205
                Return "Home"
            Case 155225
                Return "PDF"
            Case 155236
                Return "Hoofdtaal"
            Case 155245
                Return "Over ons"
            Case 155263
                Return "pixel"
            Case 155264
                Return "Vertalen"
            Case 155374
                Return "Boekhouding ID"
            Case 155507
                Return "Mogelijk maken"
            Case 155575
                Return "Standaard automatische output locatie"
            Case 155601
                Return "Geen geselecteerde item"
            Case 155642
                Return "Recept uitwisseling"
            Case 155713
                Return "%r bestaat"
            Case 155731
                Return "CALCMENU Pro"
            Case 155763
                Return "Vergelijken met nummer"
            Case 155764
                Return "Vergelijken met naam"
            Case 155841
                Return "Bestand om te restoren"
            Case 155862
                Return "per"
            Case 155942
                Return "Opgeslagen bestellijsten laden"
            Case 155967
                Return "Velden separator"
            Case 155994
                Return "Niet actief"
            Case 155996
                Return "E-mail adres"
            Case 156000
                Return "Naar een nieuwe leverancier verplaatsen"
            Case 156012
                Return "Helpdesk"
            Case 156015
                Return "Contact"
            Case 156016
                Return "Main Office"
            Case 156141
                Return "Back up/Herstellen Databank"
            Case 156337
                Return "Voedingswaarde verbinden"
            Case 156344
                Return "Ongeldige selectie"
            Case 156355
                Return "Archiveren"
            Case 156356
                Return "Toevoegen"
            Case 156405
                Return "Gelieve ruimte vrij te maken en dan op 'Opnieuw proberen' klikken."
            Case 156413
                Return "Sub-recept definitie"
            Case 156485
                Return "Bestanden na import verwijderen"
            Case 156552
                Return "Backup nu"
            Case 156590
                Return "Ingrediënten van CSV bestand (Excel) importeren"
            Case 156669
                Return "Website"
            Case 156672
                Return "Online gebruikt (voor web inhoud)"
            Case 156683
                Return "Origineel"
            Case 156720
                Return "Aantal te lang"
            Case 156721
                Return "Naam te lang"
            Case 156722
                Return "Leverancier te lang"
            Case 156723
                Return "Categorie te lang"
            Case 156725
                Return "Beschrijving te lang"
            Case 156734
                Return "Er zijn 2 eenheden identiek"
            Case 156742
                Return "Verloopt na"
            Case 156751
                Return "Tel:  +41 848 000 357<br>(English, French, German, Operating hours: 8:30am-6pm GMT +01:00)<br><br>Tel:  +41 32 544 00 17<br>(English ONLY, Operating hours: 3am-830am GMT +01:00)"
            Case 156752
                Return "Toll Free:  1-800-964-9357<br>(English ONLY, Operating hours: 9am-3am Pacific Standard Time)"
            Case 156753
                Return "Tel: +63 2 687 3179<br>(English ONLY, Operating hours: 12am-6pm GMT +08:00) "
            Case 156754
                Return "Bestandsnaam"
            Case 156825
                Return "Duizend"
            Case 156870
                Return "Zeker weten?"
            Case 156925
                Return "Download OK"
            Case 156938
                Return "Actief"
            Case 156941
                Return "Keuken handheld"
            Case 156955
                Return "Persoonlijk"
            Case 156957
                Return "Hotels"
            Case 156959
                Return "Gedeeld"
            Case 156960
                Return "Verzonden"
            Case 156961
                Return "Prijzenreeks"
            Case 156962
                Return "Niet verzonden"
            Case 156963
                Return "Prijzen"
            Case 156964
                Return "Zoeken in"
            Case 156965
                Return "Opbrengsten"
            Case 156966
                Return "Records beïnvloed"
            Case 156967
                Return "Gelieve de correcte datum invoeren"
            Case 156968
                Return "Ongeldig afbeelding bestandsformaat"
            Case 156969
                Return "Gelieve het afbeeldingbestand voor upload in te voeren. Of laat het anders leeg."
            Case 156970
                Return "Categorie informatie invoeren"
            Case 156971
                Return "Prijzenreeks informatie invoeren"
            Case 156972
                Return "Sleutelwoord informatie invoeren"
            Case 156973
                Return "Eenheid informatie invoeren"
            Case 156974
                Return "Opbrengstinformatie invoeren"
            Case 156975
                Return "Nieuwe recepten creëren en verzenden naar het hoofdkantoor voor gebruik in andere hotels"
            Case 156976
                Return "Het ingrediënt is het basis element van uw recepten en menu's"
            Case 156977
                Return "Indien u enkele opmerkingen of technische vragen over deze software heeft"
            Case 156978
                Return "Hoofdsleutelwoord"
            Case 156979
                Return "Naam van sleutelwoord"
            Case 156980
                Return "Configuratie"
            Case 156981
                Return "BTW tarieven"
            Case 156982
                Return "Zoekresultaten"
            Case 156983
                Return "Sorry, geen resultaat gevonden"
            Case 156984
                Return "Ongeldige gebruikersnaam of wachtwoord"
            Case 156986
                Return "Het item bestaat reeds"
            Case 156987
                Return "was succesvol opgeslagen"
            Case 156996
                Return "Copyright © 2004 van EGS Enggist & Grandjean Software SA, Zwitserland."
            Case 157002
                Return "Prijs van de eenheid is niet gedefinieerd. Gelieve een eenheid selecteren"
            Case 157020
                Return "Gebruikte BTW"
            Case 157026
                Return "Medium"
            Case 157033
                Return "Het systeem zal de prijzen van alle ingrediënten updaten. Gelieve te wachten…."
            Case 157034
                Return "Verificatie"
            Case 157038
                Return "Maand"
            Case 157039
                Return "Jaar"
            Case 157040
                Return "Er is geen sleutelwoord beschikbaar"
            Case 157041
                Return "Toegang geweigerd"
            Case 157049
                Return "Weet u zeker dat u wilt opslaan?"
            Case 157055
                Return "STUDENTENVERSIE"
            Case 157056
                Return "Wilt u annuleren?"
            Case 157057
                Return "Gemarkeerde items zijn nu gedeeld"
            Case 157076
                Return "Help overzicht"
            Case 157079
                Return "De volgende gemarkeerde items zijn niet verzonden en kunnen niet getransfereerd worden:"
            Case 157084
                Return "De volgende gemarkeerde items worden gebruikt en zijn niet verwijderd:"
            Case 157125
                Return "Visies"
            Case 157130
                Return "Uw creditkaart informatie is succesvol verzonden. Uw bestelling zal binnen 3 dagen verwerkt worden. Dank u!"
            Case 157132
                Return "Persoonlijk (Gedeeld)"
            Case 157133
                Return "Persoonlijk (Niet gedeeld)"
            Case 157134
                Return "Bezoeker"
            Case 157136
                Return "Krediet"
            Case 157139
                Return "Slecht!"
            Case 157140
                Return "Goed!"
            Case 157141
                Return "Fantastisch!"
            Case 157142
                Return "Ongebruikte ingrediënten eenheden voor de import verwijderen"
            Case 157151
                Return "Andere verbindingen"
            Case 157152
                Return "Gebruikersbeoordelingen"
            Case 157153
                Return "De ontvanger wordt herinnerd om deze items te accepteren"
            Case 157154
                Return "De volgende items kunnen niet worden vrij gegeven omdat ze in bezit zijn van andere gebruikers"
            Case 157155
                Return "Iemand wil u graag de volgende recepten geven:"
            Case 157156
                Return "Promo"
            Case 157157
                Return "Gebruikersopties"
            Case 157158
                Return "Originaliteit"
            Case 157159
                Return "Resultaat"
            Case 157160
                Return "Moeilijkheid"
            Case 157161
                Return "Recept van de dag"
            Case 157164
                Return "Kaarthouder naam"
            Case 157165
                Return "Creditkaart nummer"
            Case 157166
                Return "Record limiet"
            Case 157168
                Return "Bank"
            Case 157169
                Return "PayPal"
            Case 157170
                Return "Online bestellen is niet beschikbaar in uw land"
            Case 157171
                Return "Wordt lid"
            Case 157172
                Return "Upgrade kostenloos"
            Case 157173
                Return "Lidmaatschap fee"
            Case 157174
                Return "Upgrade pakket"
            Case 157176
                Return "Totaal aantal gebruikte records"
            Case 157177
                Return "We bieden u een verscheidenheid aan oplossingen om aan uw behoeften te voldoen"
            Case 157178
                Return "Proefgebruiker"
            Case 157179
                Return "Vertel een vriend"
            Case 157180
                Return "E-mail adres van een vriend"
            Case 157182
                Return "FAQs"
            Case 157183
                Return "Algemene Service voorwaarden"
            Case 157214
                Return "Een bestellijst creëren alleen voor gemarkeerde recepten"
            Case 157217
                Return "Een bestellijst creëren alleen voor gemarkeerde menu's"
            Case 157226
                Return "Gemarkeerde recepten zijn voor goedkeuring verzonden"
            Case 157233
                Return "Waste kan niet groter of gelijk zijn aan 100%"
            Case 157268
                Return "Gebruikte valuta"
            Case 157269
                Return "Prijzenreeks wordt gebruikt"
            Case 157273
                Return "Kan de volgende items niet delen omdat ze nooit verzonden zijn of van niemand in bezit waren"
            Case 157274
                Return "Wisselkoers"
            Case 157275
                Return "Alle items in de lijst zullen als 1 item samengevoegd worden. Gelieve een item welke wordt gebruikt door gebruikers selecteren. Andere items zullen uit de database worden verwijderd"
            Case 157276
                Return "Succesvol samengevoegd"
            Case 157277
                Return "Totale kosten"
            Case 157297
                Return "Gelieve tenminste een item selecteren"
            Case 157299
                Return "Wijzig uw profiel en pas uw vertoning aan"
            Case 157300
                Return "Gelieve een nieuw wachtwoord in te geven. Een wachtwoord kan de 20 karakters niet overschrijden. Klik op 'Verzenden' als u gereed bent"
            Case 157301
                Return "Gelieve het afbeeldingbestand (jpeg/jpg, bmp enz) invoeren die u wilt uploaden. Laat het anders leeg (Let op: GIF bestanden worden niet ondersteund. Alle afbeeldingen worden gekopieerd en dan geconverteerd naar normaal en thumbnail jpeg formaat)"
            Case 157302
                Return "Zoek ingredient op naam of op deel van een naam (gebruik [*] teken). Om snel toe te voegen, voer [netto hoeveelheid]_[eenheid]_[ingredient] zoals 200 g Oel High Oleic"
            Case 157303
                Return "Om de ingrediëntenprijs toe te voegen of te wijzigen, voer een nieuwe prijs in en definieer de eenheid of meeteenheid. U moet nu de verhouding tussen die eenheid en de originele eenheid invoeren. Bijvoorbeeld de originele prijs en eenheid is € 11,00 per kilogram (kg). Als u de eenheid per zak wilt toevoegen, moet u de prijs van die zak definiëren of definiëren hoeveel kilogram in 1 zak gaan (verhouding)."
            Case 157304
                Return "Zoek sleutelwoorden per naam of deel van een naam. Gebruik komma [,] voor meervoudige sleutelwoorden. Bijvoorbeeld zoek 'biefstuk, saus, bruiloft'"
            Case 157305
                Return "Gelieve een item te selecteren"
            Case 157306
                Return "Ongeldig bestandstype"
            Case 157310
                Return "Ingrediënten details"
            Case 157314
                Return "Bij toevoegen van ingrediëntenprijzen, hoofdeenheid gebruiken"
            Case 157320
                Return "Delen"
            Case 157322
                Return "Gebruikersovereenkomst"
            Case 157323
                Return "Geven"
            Case 157329
                Return "Kassastation"
            Case 157334
                Return "Waarschuwing: U kunt al uw wijzigingen verliezen als een andere gebruiker dit record gewijzigd heeft. Wilt u deze pagina verversen?"
            Case 157339
                Return "Berichten per pagina"
            Case 157340
                Return "Snel browsen"
            Case 157341
                Return "op elke pagina"
            Case 157342
                Return "Record is door een andere gebruiker veranderd. Klik op 'OK' om door te gaan"
            Case 157343
                Return "Dit record is verwijderd door een andere gebruiker"
            Case 157345
                Return "Naar hoofdkantoor zenden"
            Case 157346
                Return "Niet gedeeld"
            Case 157378
                Return "Lid"
            Case 157379
                Return "Abonneer nu"
            Case 157380
                Return "Uw lidmaatschap zal over %n dagen vervallen"
            Case 157381
                Return "Uw lidmaatschap is vervallen"
            Case 157382
                Return "Verleng mijn lidmaatschap gebruik makend van de overblijvende punten (credits)"
            Case 157383
                Return "U heeft uw schrijfruimte limiet bereikt. Gelieve enkele van uw recepten of ingrediënten verwijderen. Dank u."
            Case 157384
                Return "Ongeldige transactie"
            Case 157385
                Return "Bedankt!"
            Case 157387
                Return "U wordt naar de PayPal doorverwezen om uw betaling te completeren. Gelieve een moment nemen om uw valuata te kiezen zodat wij het juiste bedrag kunnen berekenen. Gelieve uit de onderstaande lijst kiezen"
            Case 157388
                Return "Een uitnodiging om deel te nemen"
            Case 157404
                Return "Lopende transactie"
            Case 157405
                Return "Voor een vraag kunt u een e-mail zenden naar"
            Case 157408
                Return "Alleen leden en proefgebruikers hebben toegang tot deze pagina. Wilt u uw eigen recepten beheren in ReceptenGallerie.com? Ga naar het lidmaatschapsmenu en schrijf u in als lid"
            Case 157435
                Return "Automatische uitgifte naar outlet voor een output"
            Case 157437
                Return "Grondstof"
            Case 157446
                Return "Maand(en)"
            Case 157594
                Return "Accepteren"
            Case 157595
                Return "Ontkennen"
            Case 157596
                Return "Geen gebruikerbeoordeling"
            Case 157604
                Return "E-mail support"
            Case 157607
                Return "Phone Support"
            Case 157608
                Return "Online Support"
            Case 157616
                Return "USA"
            Case 157617
                Return "ASIA and the Rest of the World"
            Case 157629
                Return "Goedkeuren"
            Case 157633
                Return "Afkeuren"
            Case 157695
                Return "Rekeningnummer"
            Case 157772
                Return "Optioneel"
            Case 157802
                Return "Wachtwoord bevestigen"
            Case 157901
                Return "Bestaande verbergen"
            Case 157926
                Return "Sign Up"
            Case 158005
                Return "Licentie"
            Case 158019
                Return "Aanvraag status controleren"
            Case 158169
                Return "Kindly choose your payment terms.¶¶Advance Payment via:"
            Case 158170
                Return "Kindly e-mail us your credit card details at <a href='mailto:info@calcmenu.com'>info@calcmenu.com</a>. Credit Card Type (Visa, Mastercard, American Express), Cardholder's Name, Credit Card Number (Please include the 3-digit security code (CVC2/CVV2) which you can find at the back of your card) and Expiry Date."
            Case 158171
                Return "Bank/Wire Transfer"
            Case 158174
                Return "<b>Note:</b> Please advise us once the transfer has been made. It will take 1-2 weeks before we receive our bank confirmation regarding the transfer."
            Case 158186
                Return "Wachtwoord veranderen"
            Case 158220
                Return "Create new ingredient name with up to 250 characters and include alphanumeric reference number, tax rate, four wastage percentages, category, supplier, and other helpful information such as product description, preparation, cooking tip, refinement methods, and storage."
            Case 158229
                Return "Afbeeldingen"
            Case 158230
                Return "Ingredient, Recipes, and Menus can be searched using their name or reference numbers. You can also search using categories and keywords. For the ingredient, you can also use supplier, date encoded or last modified, price range, and nutrient values when searching. For the recipes and menus, you can search using items used and not used."
            Case 158232
                Return "Action Marks are shortcuts in performing a similar function that could apply to a marked ingredient, recipe or menu. You can use action marks to assign ingredient, recipe, or menu to a category and keywords, delete them, export, send via e-mail, print, share, and unshare to other users without having to repeat them for each item. This saves you a lot of time and effort in performing an action to the marked items."
            Case 158234
                Return "Nutrient Linking and Calculation"
            Case 158238
                Return "Supplier Management"
            Case 158240
                Return "Category, Keywords, Sources Management"
            Case 158243
                Return "Tax Rate Management"
            Case 158246
                Return "Unit Management"
            Case 158249
                Return "Printing, PDF and Excel Export"
            Case 158306
                Return "Selecteren"
            Case 158346
                Return "meer"
            Case 158376
                Return "Theoretische vastgelegde verkoopprijs"
            Case 158511
                Return "Als u gelooft dat dit niet het geval is, gelieve ons een e-mail te sturen <a href='mailto:%email'>%email</a>"
            Case 158577
                Return "Locatie taal"
            Case 158585
                Return "Hoofdkantoor"
            Case 158588
                Return "Kan de volgende items niet verzenden omdat ze in bezit zijn door een andere gebruiker"
            Case 158653
                Return "Mobiel"
            Case 158677
                Return "Verkoopitem¶nummer"
            Case 158694
                Return "Info veranderen"
            Case 158696
                Return "For Philippine Clients only"
            Case 158730
                Return "Uitsluiten"
            Case 158783
                Return "Recept(en)/sub-recept(en) toevoegen"
            Case 158810
                Return "Prijs calculeren"
            Case 158835
                Return "Sorteren door belasting"
            Case 158837
                Return "Sorteren door prijs"
            Case 158839
                Return "Sorteren door kostprijs"
            Case 158840
                Return "Sorteren door factor"
            Case 158845
                Return "Sorteren door verkoopprijs"
            Case 158846
                Return "Sorteren door opgelegde prijs"
            Case 158849
                Return "Hoog"
            Case 158850
                Return "Laag"
            Case 158851
                Return "Gecreëerd door"
            Case 158860
                Return "POS instelling wijzigen"
            Case 158902
                Return "Openingstijd"
            Case 158912
                Return "Verzoeken"
            Case 158935
                Return "Totale opbrengsten"
            Case 158947
                Return "U wordt doorverwezen naar PayPal om uw bestelling te completeren"
            Case 158952
                Return "Goedgekeurd"
            Case 158953
                Return "Niet goedgekeurd"
            Case 158960
                Return "Deze functie is ongeschikt gemaakt. Gelieve contact op te nemen met uw hoofdkantoor als u nieuwe recepten nodig heeft"
            Case 158998
                Return "Search Features"
            Case 158999
                Return "Ingredient, recipe, and menu lists can be printed together with their details, prices, and nutrient values. Shopping lists or the list of ingredients together with cumulative quantities used in various recipes can also be printed. PDF and Excel files can also be created for the various reports."
            Case 159000
                Return "Set of Price and Multiple Currency Management"
            Case 159009
                Return "Rand"
            Case 159035
                Return "Onvolledig"
            Case 159064
                Return "Naam kan niet leeg zijn"
            Case 159082
                Return "Producten gebaseerd op de laatst gewijzigde datum updaten"
            Case 159089
                Return "Verzoek voor goedkeuring annuleren"
            Case 159112
                Return "Voor goedkeuring"
            Case 159113
                Return "Erfelijk"
            Case 159133
                Return "Leveringsinformatie"
            Case 159139
                Return "Samenstelling"
            Case 159140
                Return "Eenheid te lang"
            Case 159141
                Return "Eenheid %n bestaat niet"
            Case 159142
                Return "%n mag niet leeg zijn"
            Case 159144
                Return "Importeren bestand. Gelieve te wachten….."
            Case 159145
                Return "Items opslaan. Een moment geduld….."
            Case 159162
                Return "&Details verbergen"
            Case 159168
                Return "Sorteren door netto hoeveelheid"
            Case 159169
                Return "Sorteren door bruto hoeveelheid"
            Case 159171
                Return "Rooster"
            Case 159181
                Return "Sorteren door aantal"
            Case 159264
                Return "Ingrediënten CSV/Leveranciersnetwerk importeren"
            Case 159273
                Return "Totale verbruiksmarge"
            Case 159275
                Return "Door licenties begrenst"
            Case 159298
                Return "Menusleutelwoord"
            Case 159349
                Return "Filter terug zetten"
            Case 159360
                Return "Regio-chef"
            Case 159361
                Return "Chef-kok"
            Case 159362
                Return "Geselecteerde item wordt gebruikt"
            Case 159363
                Return "Merkinformatie invoeren"
            Case 159364
                Return "Merk"
            Case 159365
                Return "Rol"
            Case 159366
                Return "Gebruik makend van SMTP op de server"
            Case 159367
                Return "Gebruik makend van SMTP op het netwerk"
            Case 159368
                Return "Logo"
            Case 159369
                Return "Vergelijken met"
            Case 159370
                Return "succesvol geïmporteerd"
            Case 159372
                Return "Globaal"
            Case 159379
                Return "oplopend"
            Case 159380
                Return "aflopend"
            Case 159381
                Return "Aan alle gebruikers tonen"
            Case 159382
                Return "Naar systeemrecept converteren"
            Case 159383
                Return "Niet tonen"
            Case 159384
                Return "Regio"
            Case 159385
                Return "Invoer verzenden"
            Case 159386
                Return "Prijzen en voedingswaardes zijn niet herberekend"
            Case 159387
                Return "Prijzen en voedingswaardes zijn herberekend"
            Case 159388
                Return "Een nieuwe menukaart creëren"
            Case 159389
                Return "Menukaart veranderen"
            Case 159390
                Return "E-mail verzonden"
            Case 159391
                Return "Goedgekeurde prijs"
            Case 159424
                Return "Deze functie is ongeschikt gemaakt. Gelieve contact op te nemen met uw hoofdkantoor als u nieuwe ingrediënten nodig heeft"
            Case 159426
                Return "Ingrediënt zoeken op naam of deel van de naam. Om snel toe te voegen, voer (netto hoeveelheid)_(eenheid)_(ingrediënt) in"
            Case 159430
                Return "Registratie informatie is succesvol opgeslagen"
            Case 159433
                Return "Verzenden naar systeem"
            Case 159434
                Return "Aan systeem verzonden"
            Case 159435
                Return "Naar een nieuwe categorie verplaatsen"
            Case 159436
                Return "E-mail aan afzender voor Systeem Alarmberichten"
            Case 159437
                Return "Bestand is succesvol geupload"
            Case 159444
                Return "Afbeeldinggrootte vastleggen"
            Case 159445
                Return "Tijdzone"
            Case 159446
                Return "Afbeeldingbewerking"
            Case 159457
                Return "SQL Server Volledige tekstzoek-functie heeft de kundigheid om complexe zoekfuncties uit te voeren. Deze zoekfunctie kan op woorden of zinnen zoeken, verkeerd geschreven woorden en nabije vergelijkingen"
            Case 159458
                Return "Volledige populatie"
            Case 159459
                Return "Volledige tekstzoek-functie"
            Case 159460
                Return "minute"
            Case 159461
                Return "Elke"
            Case 159462
                Return "Starten"
            Case 159463
                Return "Toename van de bevolking"
            Case 159464
                Return "Woordonderbreker"
            Case 159471
                Return "IP Adres"
            Case 159472
                Return "Lijst met geblokkeerde IP adressen"
            Case 159473
                Return "IP blokkeren als de maximale login-pogingen bereikt zijn"
            Case 159474
                Return "Gelieve tenminste ¶ karakters invoeren"
            Case 159485
                Return "Verzenden naar receptenuitwisseling"
            Case 159486
                Return "Aan Receptuitwisseling verzonden"
            Case 159487
                Return "U heeft dit recept goedgekeurd. Het kan nu door alle gebruikers gezien worden"
            Case 159488
                Return "Onbekende taal"
            Case 159607
                Return "Standalone Recipe Management Software"
            Case 159608
                Return "Recipe Management Software for Concurrent Users in a Network"
            Case 159609
                Return "Web Based Recipe Management Software"
            Case 159610
                Return "Inventory and Back Office Management Software"
            Case 159611
                Return "Recipe Viewer for Pocket PC"
            Case 159612
                Return "Order Taking and Nutrient Monitoring Software"
            Case 159613
                Return "E-Cookbook Software"
            Case 159699
                Return "Bestaande items updaten"
            Case 159707
                Return "France"
            Case 159708
                Return "Germany"
            Case 159751
                Return "Locatie"
            Case 159778
                Return "Geavanceerd"
            Case 159779
                Return "Basis"
            Case 159782
                Return "Verkoopitems aan producten verbinden"
            Case 159783
                Return "Verkoopitems aan recepten/menu's verbinden"
            Case 159795
                Return "POS import '- configuratie"
            Case 159918
                Return "U heeft geen rechten om toegang te verkrijgen tot deze functie"
            Case 159924
                Return "Beheren"
            Case 159925
                Return "Ongeldige conversie"
            Case 159929
                Return "Pagina opties"
            Case 159934
                Return "Voedingswaarde informatie"
            Case 159940
                Return "Updates exporteren"
            Case 159941
                Return "Alles exporteren"
            Case 159942
                Return "Output directory"
            Case 159943
                Return "Kwaliteit"
            Case 159944
                Return "Hoofd"
            Case 159946
                Return "CALCMENU Web 2007"
            Case 159947
                Return "Bestand selecteren of uploaden"
            Case 159949
                Return "Formaat moet niet groter zijn dan 10 karakters"
            Case 159950
                Return "Voedingswaarde naam moet niet groter zijn dan 25 karakters"
            Case 159951
                Return "Rollen"
            Case 159962
                Return "BTW informatie invoeren"
            Case 159963
                Return "Vertaling invoeren"
            Case 159966
                Return "Gemarkeerde items naar nieuw merk verplaatsen"
            Case 159967
                Return "Standaard locatie naam invoeren:"
            Case 159968
                Return "Standaard Website thema invoeren"
            Case 159969
                Return "In staat stellen om locaties te groeperen door regio beheerd door een admin:"
            Case 159970
                Return "Gebruikers moeten eerst informatie naar de goedkeurende zenden voordat het kan worden gebruikt of gepubliceerd:"
            Case 159971
                Return "Voer de vertaling voor elke corresponderende taal in of de standaard tekst zal worden gebruikt:"
            Case 159973
                Return "Selecteer de locaties die moeten behoren tot deze regio"
            Case 159974
                Return "Selecteer beschikbare talen om te gebruiken voor het vertalen van ingrediënten, recepten, menu's en andere informatie"
            Case 159975
                Return "Selecteer 1 of meer prijsgroepen om te gebruiken voor het toekennen van prijzen aan uw ingrediënt, recept en menu"
            Case 159976
                Return "Kies de items om in te voegen"
            Case 159977
                Return "Lijst van bezitters"
            Case 159978
                Return "Kies een onderstaand formaat"
            Case 159979
                Return "Kies de basis lijst om te verwijderen"
            Case 159981
                Return "De volgende zijn de gedeelde locaties voor dit item"
            Case 159982
                Return "Gemarkeerde naar een nieuwe bron verplaatsen"
            Case 159987
                Return "Aanvraag type"
            Case 159988
                Return "Aangevraagd door"
            Case 159990
                Return "Merk veranderen"
            Case 159994
                Return "Ingrediënt in menu's vervangen"
            Case 159997
                Return "Globale deling"
            Case 160004
                Return "Eerste niveau"
            Case 160005
                Return "Het geselecteerde ingrediënt moet de volgende eenheden hebben:"
            Case 160008
                Return "Stap"
            Case 160009
                Return "Meer acties"
            Case 160012
                Return "Dit recept/menu is gepubliceerd op het web"
            Case 160013
                Return "Dit recept/menu is niet gepubliceerd op het web"
            Case 160014
                Return "Onthouden"
            Case 160016
                Return "Bezitters tonen"
            Case 160018
                Return "Dit ingrediënt is gepubliceerd op het web"
            Case 160019
                Return "Dit ingrediënt is niet gepubliceerd op het web"
            Case 160020
                Return "Dit ingrediënt wordt getoond"
            Case 160021
                Return "Dit ingrediënt wordt niet getoond"
            Case 160023
                Return "Om te printen"
            Case 160028
                Return "Niet om te worden gepubliceerd"
            Case 160030
                Return "Aan de bestellijst toevoegen"
            Case 160033
                Return "Sleutelwoorden toevoegen"
            Case 160035
                Return "U heeft geprobeerd om %n keer in te loggen"
            Case 160036
                Return "Deze rekening is gedeactiveerd"
            Case 160037
                Return "Gelieve uw systeem administrator te contacteren om de toegang opnieuw te activeren"
            Case 160038
                Return "Mijn profiel"
            Case 160039
                Return "Laatste login"
            Case 160040
                Return "U bent niet aangemeld"
            Case 160041
                Return "Pagina taal"
            Case 160042
                Return "Hoofd vertaling"
            Case 160043
                Return "Hoofdprijzenreeks"
            Case 160045
                Return "Rijen per pagina"
            Case 160046
                Return "Standaard vertoning"
            Case 160047
                Return "Ingrediënt hoeveelheden"
            Case 160048
                Return "Laatste toegang"
            Case 160049
                Return "'%f' ontvangen"
            Case 160050
                Return "Lengte"
            Case 160051
                Return "Gefaald om '%f' te ontvangen"
            Case 160055
                Return "Hoeveelheid moet groter zijn dan 0"
            Case 160056
                Return "Een nieuw sub-recept creëren"
            Case 160057
                Return "Sessie is verlopen"
            Case 160058
                Return "Uw login is verlopen omdat u langer dan %n minuten inactief was"
            Case 160065
                Return "Geen naam"
            Case 160066
                Return "Weet u zeker dat u wilt sluiten?"
            Case 160067
                Return "Uw invoer benodigd goedkeuring"
            Case 160068
                Return "Klik op de '%s' knop om goedkeuring aan te vragen"
            Case 160070
                Return "Gemarkeerde items worden bewerkt"
            Case 160071
                Return "Deze invoer is verzonden voor goedkeuring"
            Case 160072
                Return "Er is reeds een bestaande aanvraag voor deze invoer"
            Case 160074
                Return "Eenheid selecteren"
            Case 160082
                Return "Nieuwe verzoeken wachten op uw goedkeuring"
            Case 160085
                Return "Uw aanvraag wordt getoetst"
            Case 160086
                Return "Voedingswaarde lijst printen"
            Case 160087
                Return "Lijst printen"
            Case 160088
                Return "Details printen"
            Case 160089
                Return "Activeren"
            Case 160090
                Return "Creëren"
            Case 160091
                Return "Geselecteerde item van de lijst verwijderen"
            Case 160093
                Return "Naar systeem verzenden voor algemene deling"
            Case 160094
                Return "Inhoud beschikbaar maken op Kiosk-browser"
            Case 160095
                Return "Een systeem kopie creëren"
            Case 160096
                Return "Ingrediënt gebruikt in recepten en menu's vervangen"
            Case 160098
                Return "Niet op het web publiceren"
            Case 160100
                Return "Creëer een in te kopen ingrediëntenlijst"
            Case 160101
                Return "U kunt tekst gebruiken voor ingrediënten die geen hoeveelheid en prijs definitie nodig hebben"
            Case 160102
                Return "Creëer uw eigen recepten database, deel het met anderen, print het and creëer zelfs een bestellijst"
            Case 160103
                Return "Menu is een lijst van ingrediënten of recepten beschikbaar in een maaltijd"
            Case 160105
                Return "Basis informatie zoals gebruikers, leveranciers enz. organiseren"
            Case 160106
                Return "Welkom"
            Case 160107
                Return "Welkom bij %s"
            Case 160108
                Return "Maak uw vertoning en andere instellingen op maat"
            Case 160109
                Return "Website profiel"
            Case 160110
                Return "Maak website naam, thema's etc. klantgericht"
            Case 160111
                Return "Goedkeurende Routing"
            Case 160112
                Return "Goedkeuring van ingrediënten, recepten en andere informatie"
            Case 160113
                Return "Instellingen voor SMTP en Alarmberichten"
            Case 160114
                Return "Verbinding met uw mail server configureren, activeren of deactiveren van alarm"
            Case 160115
                Return "Voer het maximale aantal login pogingen in en houdt toezicht op de geblokkeerde IP adressen"
            Case 160116
                Return "Print Profiel"
            Case 160117
                Return "Meerdere print formaten als profiel definiëren"
            Case 160118
                Return "Lijst van talen definiëren voor het vertalen van ingrediënten, recepten's en andere informatie" '"Lijst van talen definiëren voor het vertalen van ingrediënten, recepten, menu's en andere informatie"
            Case 160119
                Return "Beschikbare valuta's voor valuta conversie en prijzenreeks-definities"
            Case 160120
                Return "Werken met ingrediënten, recepten en menu's met meerdere prijzenreeksen"
            Case 160121
                Return "Regio's zijn groepen locaties"
            Case 160122
                Return "Locaties organiseren gebruikers die gemeenschappelijk aan een bepaalde receptenreeks werken"
            Case 160123
                Return "Gebruikers die werken op %s beheren"
            Case 160124
                Return "Afbeeldingbewerking voorkeuren"
            Case 160125
                Return "Standaard afbeeldinggrootte voor ingrediënten, recepten en menu's definiëren"
            Case 160130
                Return "Handelskenmerk of kenmerknaam die een ingredient identificeert"
            Case 160132
                Return "Gebruikelijk om ingrediënten, recepten of menu's te groeperen door gemeenschappelijke eigenschappen"
            Case 160135
                Return "Sleutelwoorden verschaffen details aan ingrediënten, recepten of menu's. Gebruiker kan meerdere sleutelwoorden per ingrediënt, recept of menu toewijzen"
            Case 160139
                Return "Definieer maximaal 34 voedingswaardes zoals Energie, Koolhydraten, Proteïnen en Vet."
            Case 160141
                Return "Creëer regels die gebruikt kunnen worden als een additionele zoek filter"
            Case 160151
                Return "Lijst van voorgedefinieerde (of systeem) eenheden gebruikt bij de definiëring van ingrediëntprijzen, recepten en menu's"
            Case 160152
                Return "Gebruikers kunnen aan deze lijst toevoegen"
            Case 160153
                Return "Gebruikt in prijs calculaties"
            Case 160154
                Return "De bron verwijst naar de oorsprong van een bepaald recept. Dit kan een chef, boek, tijdschrift, bedrijf, organisatie of website zijn"
            Case 160155
                Return "Ingrediënten, recepten of menu's van CALCMENU Pro, CALCMENU Enterprise of andere EGS producten importeren"
            Case 160156
                Return "Behoud van wisselkoersen voor verschillende valuta's"
            Case 160157
                Return "Ongebruikte teksten verwijderen"
            Case 160158
                Return "Alle teksten formateren"
            Case 160159
                Return "Ingrediëntlijst printen in HTML, Excel, PDF en RTF formaten"
            Case 160160
                Return "Print ingrediënt details in HTML, Excel, PDF en RTF formaten"
            Case 160161
                Return "Print recept details in HTML, Excel, PDF en RTF formaten"
            Case 160162
                Return "Print receptlijst in HTML, Excel, PDF en RTF formaten"
            Case 160163
                Return "Print menu details in HTML, Excel, PDF en RTF formaten"
            Case 160164
                Return "Menu engineering stelt u in staat om huidige en toekomstige receptprijzen en receptontwerp te evalueren"
            Case 160169
                Return "Menukaarten lijst laden"
            Case 160170
                Return "Opgeslagen menukaarten wijzigen of voorvertonen"
            Case 160175
                Return "Opgeslagen bestellijsten wijzigen, voorvertonen of printen"
            Case 160177
                Return "Veiligheid"
            Case 160180
                Return "Formaten van de items standaardiseren"
            Case 160181
                Return "Items opschonen"
            Case 160182
                Return "Rolrechten"
            Case 160184
                Return "TCPOS Export"
            Case 160185
                Return "Verkoopitem exporteren"
            Case 160187
                Return "Nieuwe locale ingrediënten creëren die gebruikt kunnen worden als ingrediënt voor uw recepten"
            Case 160188
                Return "Lijst met opgeslagen markeringen tonen"
            Case 160189
                Return "Lijst met in te kopen items tonen"
            Case 160190
                Return "Creëer uw eigen menu's gebaseerd op de beschikbare recepten in uw database"
            Case 160191
                Return "Creëer een tekst gebruikt voor recepten en menu's"
            Case 160200
                Return "Gesorteerd per naam"
            Case 160202
                Return "Uit de lijst kiezen"
            Case 160209
                Return "Gelieve een serienummer, gebruikersnaam en productsleutel invoeren. U vindt deze informatie in de documentatie verstrekt met %s"
            Case 160210
                Return "Gewenste items"
            Case 160211
                Return "Ongewenste items"
            Case 160212
                Return "Ontwerpen"
            Case 160217
                Return "Actief pad"
            Case 160218
                Return "Fout bij import van CSV bestand"
            Case 160219
                Return "Lijst met ingrediënten die in afwachting zijn om bewerkt te worden"
            Case 160220
                Return "Opties definiëren voor ingrediënt import"
            Case 160254
                Return "Gelieve uw Windows-service %n opnieuw op te starten voordat de veranderingen doorgevoerd worden"
            Case 160258
                Return "Valuta is niet passend bij de gekozen prijzenreeks"
            Case 160259
                Return "Naam of nummer bestaat reeds"
            Case 160260
                Return "Datum van import"
            Case 160262
                Return "Voedingswaardes zijn per 1 opbrengsteenheid"
            Case 160292
                Return "Allergenen"
            Case 160293
                Return "Lijst met voedsel allergenen en gevoeligheden"
            Case 160295
                Return "Deze login is op dit moment in gebruik. Gelieve later te proberen"
            Case 160353
                Return "Inkoop prijzenreeks"
            Case 160354
                Return "Verkoop prijzenreeks"
            Case 160423
                Return "Standalone Recipe/Menu Management Software"
            Case 160433
                Return "Consumptie binnen"
            Case 160500
                Return "Text Management"
            Case 160687
                Return "Wisselende itemkleur"
            Case 160688
                Return "Normale itemkleur"
            Case 160690
                Return "Gelieve op te merken dat als u een restore draait, dat automatisch de huidige gebruikers uit het systeem uitgelogd worden"
            Case 160691
                Return "Afbeeldingen backup/restore"
            Case 160716
                Return "Items standaard op globaal zetten"
            Case 160774
                Return "Deaktiveren"
            Case 160775
                Return "Volgende nullen verwijderen"
            Case 160777
                Return "Click here to learn more about CALCMENU."
            Case 160788
                Return "Geselecteerde item(s) is/zijn succesvol geactiveerd"
            Case 160789
                Return "Geselecteerde item(s) is/zijn succesvol gedeactiveerd"
            Case 160790
                Return "Weet u zeker dat u de geselecteerde items wilt verwijderen?"
            Case 160791
                Return "Geselecteerde item(s) zijn succesvol verwijderd"
            Case 160801
                Return "U kunt slechts 2 of meer gelijke recepten samenvoegen"
            Case 160802
                Return "Weet u zeker dat u de geselecteerde items wilt samenvoegen?"
            Case 160803
                Return "Weet u zeker dat u items wilt opschonen?"
            Case 160804
                Return "Gelieve de benodigde velden in te vullen"
            Case 160805
                Return "Selecteer 2 of meer items om samen te voegen"
            Case 160806
                Return "Weet u zeker dat u de geselecteerde items wilt deactiveren?"
            Case 160863
                Return "Ingrediënten prijslijst"
            Case 160940
                Return "Effectiviteitsdatum"
            Case 160941
                Return "Verbonden verkoopitem"
            Case 160953
                Return "Verhouding tussen verkoop prijzenreeks ten opzichte van de inkopende prijzenreeks"
            Case 160958
                Return "Werk met verkoop item met meerdere verkoop prijzenreeksen"
            Case 160985
                Return "Geen verbonden verkoopitem"
            Case 160987
                Return "Creëer verkoop items en verbind het met bestaande recepten"
            Case 160988
                Return "Verkoopitem is gebruikt in verkoop en is normaliter verbonden met een recept"
            Case 161028
                Return "Weet u zeker dat u de voedingswaarde database wilt wijzigen? Deze actie zal de voedingswaarde definities die u reeds heeft ingevoerd in uw ingrediënten wijzigen"
            Case 161029
                Return "Of de opbrengst of ingrediënten box moet zijn aangevinkt"
            Case 161049
                Return "Verwijdering van sleutelwoorden en onderliggende sleutelwoorden afdwingen"
            Case 161050
                Return "Verwijderde sleutelwoorden zullen ook losgekoppeld worden van ingrediënt/recept/menu items"
            Case 161051
                Return "Geselecteerde sleutelwoorden en hun subsleutelwoorden zijn succesvol verwijderd. Verwijderde sleutelwoorden zijn nu ook losgekoppeld van ingrediënt, recept en menu items"
            Case 161078
                Return "Precies"
            Case 161079
                Return "Beginnen met"
            Case 161080
                Return "Omvat"
            Case 161082
                Return "Tweede"
            Case 161083
                Return "Derde"
            Case 161084
                Return "Vierde"
            Case 161085
                Return "Eenmalig"
            Case 161086
                Return "Dagelijks"
            Case 161087
                Return "Wekelijks"
            Case 161088
                Return "Maandelijks"
            Case 161089
                Return "Als het bestand veranderd"
            Case 161090
                Return "Als de computer start"
            Case 161091
                Return "%s informatie invoeren"
            Case 161092
                Return "Leveranciersgroep"
            Case 161093
                Return "Rekeninginformatie"
            Case 161094
                Return "Startdatum"
            Case 161095
                Return "van de maand"
            Case 161096
                Return "POS Import - Gefaalde data"
            Case 161097
                Return "Het organiseren en behouden van up to date informatie van uw leveranciers inclusief bedrijfcontacten, adressen, betalingstermijnen enz vergemakkelijkt uw bestelproces"
            Case 161098
                Return "Terminal refereert naar de kassa's van uw POS die verbonden zijn met uw CALCMENU Web. Voeg toe, wijzig of verwijder terminals in dit programma"
            Case 161099
                Return "Configureer de POS import parameters. Voer het rooster, locatie van import bestanden enz. in"
            Case 161100
                Return "Producten en voorraad items worden gehouden en circuleren tussen verschillende locaties gedurende verschillende tijdstippen. Behoud controle in de mogelijke locaties waar producten op elk moment kunnen worden gevonden"
            Case 161101
                Return "Klanten zijn bedrijven die uw producten of eindproducten inkopen. Beheer uw klantenlijst in dit programma"
            Case 161102
                Return "Klantcontacten zijn de personen waarmee u te maken heeft in een bedrijf. Creëer, wijzig en verwijder klantcontacten"
            Case 161103
                Return "Herstel POS data die niet succesvol in het systeem geïmporteerd zijn"
            Case 161104
                Return "Dit refereert aan het type niet-verkoop transacties van goederen. Dit kan of kan niet daadwerkelijk verkocht zijn aan klanten zoals werknemersrechten of giveaways"
            Case 161105
                Return "Verkoophistorie toont snel een lijst van verkooptransacties en de betrokken verkoopitems"
            Case 161106
                Return "Gemarkeerde items"
            Case 161107
                Return "Gecalculeerde opbrengst"
            Case 161132
                Return "View My Recipes"
            Case 159274
                Return "%number only"
            Case 161147
                Return "Recipe and Menu Management (except Menu Planning)"
            Case 161162
                Return "TCPOS"
            Case 155761
                Return "Ingrediënt importeren"
            Case 161180
                Return "Define automatic upload configuration"
            Case 161181
                Return "Host name"
            Case 11060
                Return "Directory"
            Case 24068
                Return "Marge"
            Case 158734
                Return "De databank versie is niet passend bij deze programmaversie"
            Case 161275
                Return "Guideline Daily Amounts"
            Case 161276
                Return "GDA"
            Case 7250
                Return "Frans"
            Case 7280
                Return "Italiaans"
            Case 7260
                Return "Duits"
            Case 157515
                Return "Nederlands"
            Case 158868
                Return "Chinees"
            Case 161279
                Return "Without"
            Case 54295
                Return "with"
            Case 159468
                Return "Gebruikt als ingrediënt"
            Case 159469
                Return "Niet gebruikt als ingrediënt"
            Case 134159
                Return "Alles"
            Case 144582
                Return "Geen groepen"
            Case 161281
                Return "Chef kok"
            Case 161282
                Return "Regio Admin"
            Case 161283
                Return "Systeem Admin"
            Case 161284
                Return "Corporate Chef"
            Case 161285
                Return "Regio chef"
            Case 161286
                Return "Kok"
            Case 161287
                Return "Gast"
            Case 161288
                Return "Locatie chef"
            Case 161289
                Return "Locatie Admin"
            Case 161290
                Return "Beelike en printed"
            Case 161291
                Return "Niet gedefinieerd"
            Case 161292
                Return "Defined"
            Case 161294
                Return "Ongewenste items"
            Case 24269
                Return "Select all"
            Case 24268
                Return "Deselect all"
            Case 160880
                Return "Recalculate"
            Case 160894
                Return "Silver"
            Case 14110
                Return "Footer"
            Case 161300
                Return "Inkoop prijzenreeks"
            Case 160776
                Return "Go back to %s"
            Case 132617
                Return "ALLE CATEGORIEËN"
            Case 155842
                Return "Personen"
            Case 155050
                Return "ALLE SLEUTELWOORDEN"
            Case 135024
                Return "Locatie"
            Case 161333
                Return "Labels"
            Case 161334
                Return "Recipes %x-%y of %z"
            Case 104836
                Return "Een product wijzigen"
            Case 51281
                Return "Ingrediënten voor"
            Case 158349
                Return "Toegewezen sleutelwoord"
            Case 158350
                Return "Afgeleid sleutelwoord"
            Case 119130
                Return "Zoeken"
            Case 155927
                Return "ALLE BRONNEN"
            Case 161484
                Return "Temperature"
            Case 161485
                Return "Production<br />Date"
            Case 161486
                Return "Consumption<br />Date"
            Case 31700
                Return "Days"
            Case 7030
                Return "Printer"
            Case 161487
                Return "Daily Product"
            Case 161488
                Return "Consume before"
            Case 161489
                Return "Fresh enjoy freshly-prepared"
            Case 161490
                Return "Info Allergies; contains:"
            Case 161491
                Return "Assigned to all marked"
            Case 4825
                Return "Recepten"
            Case 21550
                Return "No dishes found"
            Case 24011
                Return "van"
            Case 161494
                Return "at max. 5°C"
            Case 161538
                Return "Wilt u de onderstaande informatie aanleveren."
            Case 161554
                Return "Wilt u de onderstaande informatie aanleveren."
            Case 161576
                Return "Eenheidprijs"
            Case 133328
                Return "Receptnaam"
            Case 51128
                Return "Receptnaam"
            Case 161577
                Return "Time"
            Case 161578
                Return "Total Ingredient  Cost"
            Case 161579
                Return "calculate"
            Case 161580
                Return "Ingredient Cost"
            Case 161581
                Return "Tax"
            Case 161582
                Return "Grossmargin in Fr."
            Case 161583
                Return "Gross margin in %"
            Case 159733
                Return "Artikel nr."
            Case 161584
                Return "Unit."
            Case 143003
                Return "Netto¶Hoeveelheid"
            Case 155811
                Return "Bruto¶Hoeveelheid"
            Case 161585
                Return "Price/¶Unit"
            Case 132708
                Return "Geen leverancier"
            Case 24075
                Return "Article number"
            Case 27056
                Return "and"
            Case 161766
                Return "Small portion"
            Case 161767
                Return "Large portion"
            Case 156892
                Return "Download:"
            Case 161777
                Return "Unassign keyword"
            Case 161778
                Return "Assign/unassign keywords"
            Case 161779
                Return "Breadcrumbs"
            Case 161780
                Return "Monitor Breadcrumbs"
            Case 161781
                Return "Unwanted Keyword"
            Case 161782
                Return "Print Labels"
            Case 161783
                Return "Procedure Template"
            Case 161784
                Return "Student"
            Case 161785
                Return "Ingredient nutrient values per %s"
            Case 161786
                Return "Ingredient nutrient values per 100g/ml"
            Case 155926
                Return "Naar Excel exporteren"
            Case 161787
                Return "Apply Template"
            Case 135969
                Return "Weet u zeker dat u %o wilt vervangen?"
            Case 132934
                Return "Laatste recept"
            Case 132937
                Return "Laatste menu"
            Case 161788
                Return "Assigned/Derived Keywords"
            Case 161468
                Return "Validate all"
            Case 161823
                Return "Add Row(s)"
            Case 161824
                Return "Paste from Clipboard"
            Case 161825
                Return "There is no ingredient that needs to be linked."
            Case 161826
                Return "Choose Another"
            Case 8514
                Return "Nieuwe prijs"
            Case 161827
                Return "Default Price/Unit:"
            Case 161828
                Return "Choose from existing units"
            Case 161829
                Return "Add this as a new unit"
            Case 161831
                Return "Let me edit ingredient before adding"
            Case 161832
                Return "place %s in complement"
            Case 161834
                Return "Please check the prices"
            Case 161835
                Return "Cut"
            Case 159594
                Return "&Add to recipe"
            Case 161837
                Return "Add to recipe"
            Case 10447
                Return "Bestelling"
            Case 161838
                Return "Replace existing ingredients"
            Case 161839
                Return "No ingredients found"
            Case 132672
                Return "Weet u zeker dat u %n wilt verwijderen?"
            Case 161840
                Return ""
            Case 161841
                Return "Link to ingredient or sub-recipe"
            Case 161842
                Return "All items are now linked to ingredient/sub-recipe"
            Case 161843
                Return "Item is now linked to ingredient/sub-recipe"
            Case 161844
                Return "Storing Time"
            Case 161845
                Return "Storing Temperature"
            Case 161851
                Return "Can be ordered"
            Case 161852
                Return "Recipe may contain allergens"
            Case 159088
                Return "Verzoek voor goedkeuring verzenden"
            Case 161855
                Return "Ontwerpen"
            Case 161986
                Return "Add Step"
            Case 161853
                Return "Paste"
            Case 161987
                Return "Item %n of %p"
            Case 161988
                Return "Linked Products"
            Case 161989
                Return "Not Linked Products"
            Case 158851
                Return "Gecreëerd door"
            Case 161830
                Return "Item validated"
            Case 162198
                Return "The yield has been changed. Click the Calculate button to resize ingredient quantities."
            Case 162199
                Return "The yield has been changed. Do you want to continue saving without calculating ingredient quantities?"
            Case 162203
                Return "Information"
            Case 162205
                Return "Number of bids"
            Case 162208
                Return "Weekly Business Days"
            Case 151500
                Return "Voorstel"
            Case 162211
                Return "Select Language"
            Case 162212
                Return "Business Name"
            Case 162213
                Return "Business Number"
            Case 162214
                Return "Price available"
            Case 162215
                Return "Logo to the server load"
            Case 146043
                Return "Januari"
            Case 146044
                Return "Februari"
            Case 146045
                Return "Maart"
            Case 146046
                Return "April"
            Case 146047
                Return "Mei"
            Case 146048
                Return "Juni"
            Case 146049
                Return "Juli"
            Case 146050
                Return "Augustus"
            Case 146051
                Return "September"
            Case 146052
                Return "Oktober"
            Case 146053
                Return "November"
            Case 146054
                Return "December"
            Case 162216
                Return "Preferences"
            Case 162219
                Return "Back Office"
            Case 162221
                Return "General Configuration"
            Case 162222
                Return "Insert Here"
            Case 8990
                Return "of"
            Case 162230
                Return "Enter style information"
            Case 162231
                Return "Name of style"
            Case 162232
                Return "Header style options"
            Case 160237
                Return "Vetgedrukt"
            Case 134826
                Return "Gesloten"
            Case 162235
                Return "Did you mean"
            Case 159700
                Return "&Recept importeren"
            Case 162276
                Return "Recept importeren"
            Case 162282
                Return "Notes"
            Case 159681
                Return "Recipe (%s) has too many ingredients. (Max. is %n)"
            Case 135257
                Return "Brutomarge"
            Case 31732
                Return "Menuplanning"
            Case 162340
                Return "Street"
            Case 162341
                Return "Place"
            Case 162357
                Return "Example"
            Case 162358
                Return "Keep Length of Prefix"
            Case 162359
                Return ""
            Case 162361
                Return "Tab"
            Case 162362
                Return "Pipe"
            Case 162363
                Return "Semi-colon"
            Case 162364
                Return "Space"
            Case 133590
                Return "&Paste"
            Case 155260
                Return "Vastgelegde factor"
            Case 156060
                Return "Vastgelegde IK"
            Case 156061
                Return "Vastgelegde winst"
            Case 162383
                Return "Goedkeuring"
            Case 162382
                Return "Goedkeuren"
            Case 162386
                Return "Go"
            Case 162387
                Return "Hi Approver,You have received a recipe for approval. [Name of the creator of the item] has submitted this recipe: [...]Please login to the CALCMENU Web site to review and approve the recipe.Regards,EGS Team"
            Case 162388
                Return "Hi,Your newly created recipe has been sent for approval. The recipe will be reviewed and approved first before it can be used online. You have submitted this recipe: [...]Once approved, the recipe will be available online.Regards,EGS Team"
            Case 162389
                Return "Hi Approver,You have approved this recipe: [...]The recipe will be available online.Regards,EGS Team"
            Case 162390
                Return "Hi,The recipe [...] has been approved. You can now use this recipe online.Regards,EGS Team"
            Case 162530
                Return "Delete breadcrumbs upon login"
            Case 28483
                Return "The record does not exist"
            Case 162955
                Return "Net margin in %"
            Case 132900
                Return "Prijs toevoegen"
            Case 163032
                Return "Copy Price List"
            Case 155995
                Return "Controlerend…………"
            Case 156784
                Return "Totaal fouten: %n"
            Case 51174
                Return "Import gedaan"
            Case 133334
                Return "%r importeren"
            Case 163046
                Return "Sorry, Keyword %k%n%u not found. Please press 'Browse Keyword' to select available Keywords."
            Case 135283
                Return "Laatste prijs"
            Case 156542
                Return "Gewogen gemiddelde prijs"
            Case 147381
                Return "Voorraad prijs gebruikt voor het voorgaande product"
            Case 157281
                Return "Prijs van standaard leverancier"
            Case 163057
                Return "Cost for total %s"
            Case 163058
                Return "Cost for 1 %s"
            Case 132553
                Return "Vastgelegde verkoopsprijs + belasting"
            Case 138031
                Return "Alle producten voor voorraden"
            Case 138032
                Return "Producten van gemarkeerde categorieën"
            Case 138033
                Return "Producten van gemarkeerde locaties"
            Case 138034
                Return "Producten van gemarkeerde leveranciers"
            Case 138035
                Return "Producten van een of meer voorgaande voorraden"
            Case 138030
                Return "Selecteer welke producten u voor deze voorraad wilt"
            Case 163060
                Return "Food Cost in %s"
            Case 163061
                Return "Imposed Food Cost in %s"
            Case 167719
                Return "Budget"
            Case 158410
                Return "Indien enkele producten geen gedefinieerde prijs (prijs'=0) hebben, gebruik in plaats daarvan de prijs van de standaard leverancier"
            Case 136230
                Return "Een nieuwe voorraad creëren"
            Case 136231
                Return "Voorraad info wijzigen"
            Case 3205
                Return "Naam"
            Case 135235
                Return "Voorraad waarde"
            Case 135100
                Return "Ref. nummer"
            Case 135110
                Return "Hoeveelheid¶voorraad"
            Case 160414
                Return "Qty Prev.¶Inventory"
            Case 136100
                Return "Huidige geopende voorraden"
            Case 136115
                Return "# items"
            Case 136110
                Return "Geopend op"
            Case 1146
                Return "In bewerking"
            Case 134021
                Return "Voorraad gestart op"
            Case 124164
                Return "Voorraad aanpassingen"
            Case 158946
                Return "Hoeveelheid voorradig als hoeveelheid voorraad instellen"
            Case 136213
                Return "Een product aan de huidige voorraad toevoegen"
            Case 136214
                Return "Een product van de voorraad verwijderen"
            Case 136212
                Return "Lijst van benodigde wijzigingen tonen"
            Case 136215
                Return "Een nieuwe locatie aan product toevoegen"
            Case 136217
                Return "Hoeveelheid voor de geselecteerde product'-locatie verwijderen"
            Case 155861
                Return "Hoeveelheid voor geselecteerde items naar nul terug zetten"
            Case 136216
                Return "De geselecteerde locatie voor het product verwijderen"
            Case 157336
                Return "Niet toepasbaar"
            Case 136030
                Return "Inhoud"
            Case 133147
                Return "liters"
            Case 136432
                Return "Ongeldige code"
            Case 143981
                Return "Ongeldige rekeningcode"
            Case 169310
                Return "Degustation/Development"
            Case 169318
                Return "Feedback"
            Case 110447
                Return "Order"
            Case 158216
                Return "Centralizing Recipe Management Anytime, Anywhere"
            Case 168373
                Return "Online gebruikt"
            Case 168374
                Return "Reference No1"
            Case 168375
                Return "Reference No2"
            Case 157060
                Return "Referentie nummer"
            Case 157659
                Return "Opsluiten"
            Case 157660
                Return "Deblokkeren"
            Case 170155
                Return "Assign ingredient, recipes and menus to Categories, Keywords and Sources (could be a cookbook, Website, chef, etc.). This allows you to group and organize items in EGS CALCMENU Web. Searching for ingredient, recipes or menus can be made faster and easier since Categories, Keywords, and Sources are very useful in narrowing down search results."
            Case 160232
                Return "Exporteren naar"
            Case 170770
                Return "Yield to Print"
            Case 133248
                Return "Ingrediënt"
            Case 170779
                Return "Ingredient List"
            Case 170780
                Return "Ingredient Details"
            Case 170781
                Return "Ingredient Nutrient List"
            Case 170782
                Return " Ingredient Category"
            Case 170783
                Return "Ingredient Keyword"
            Case 170784
                Return "Ingredient Published On The Web"
            Case 170785
                Return "Ingredient Not Published On The Web"
            Case 170786
                Return "Ingredient Cost"
            Case 170849
                Return "Abbreviated Preparation Method"
            Case 171301
                Return "Preparation Method"
            Case 171302
                Return "Tips"
            Case 170850
                Return "Cook Mode only"
            Case 133115
                Return "Alle recepten"
            Case 170851
                Return "None Cook Mode only"
            Case 170852
                Return "Show Off"
            Case 170853
                Return "Quick & Easy"
            Case 170854
                Return "Chef Recommended"
            Case 170855
                Return "Moderate"
            Case 170856
                Return "Challenging"
            Case 170857
                Return "Gold"
            Case 170858
                Return "Unrated"
            Case 170859
                Return "Bronze"
            Case 170860
                Return "Move marked to new standard"
            Case 171219
                Return "LeadIn"
            Case 55011
                Return "Serving Size"
            Case 171220
                Return "Servings per Yield" '"Number of Servings"
            Case 171221
                Return "Total Yield/Servings" '"Total Yield"
            Case 151436
                Return "Attachment"
            Case 150009
                Return "Exportation Done. BrandSite Successfully Exported."
            Case 171597
                Return "Recipe has been checked in by another user and cannot be modified."
            Case 27220
                Return "Hour"

            Case 171650
                Return "Prep Tijd"
            Case 171651
                Return "Cook Tijd"
            Case 171652
                Return "Marineer Tijd"
            Case 171653
                Return "Stand Tijd"
            Case 171654
                Return "Chill Time "
            Case 171655
                Return "Brew Tijd"
            Case 171656
                Return "Freeze Time "
            Case 171657
                Return "ReadyIn"
            Case 171658
                Return "second"
            Case 171616
                Return "Placement"

        End Select
    End Function
 
 
'russian
    Public Function FTBLow42USA(ByVal Code As Integer) As String
        Select Case Code
            Case 1080
                Return "Стоимость товара"
            Case 1081
                Return "Стоимость товара"
            Case 1090
                Return "Отпускная цена"
            Case 1145
                Return "Counter"
            Case 1260
                Return "Товар"
            Case 1280
                Return "Примечание"
            Case 1290
                Return "Цена"
            Case 1300
                Return "Убыток"
            Case 1310
                Return "Количество"
            Case 1400
                Return "Меню"
            Case 1450
                Return "Категория"
            Case 1480
                Return "Установленная цена"
            Case 1485
                Return "Calculated price"
            Case 1500
                Return "Дата"
            Case 1530
                Return "Unit missing"
            Case 1600
                Return "Модифицировать меню"
            Case 2430
                Return "&Choose from the list"
            Case 2700
                Return "Print menu list"
            Case 2780
                Return "Shopping list"
            Case 3057
                Return "База данных"
            Case 3140
                Return "Для"
            Case 3150
                Return "Процентная ставка"
            Case 3161
                Return "Постоянный"
            Case 3195
                Return "Recipe #"
            Case 3200
                Return "Шеф-повар"
            Case 3204
                Return "First Name"
            Case 3206
                Return "Перевод"
            Case 3215
                Return "Цена единицы продукции"
            Case 3230
                Return "Рисунок"
            Case 3234
                Return "Список"
            Case 3300
                Return "Menu Card"
            Case 3305
                Return "Имя поручителя"
            Case 3306
                Return "Представитель"
            Case 3320
                Return "Вы хотите привести в соответствие количество с новым числом порций?"
            Case 3460
                Return "&Password"
            Case 3680
                Return "Backup"
            Case 3685
                Return "Backup completed"
            Case 3721
                Return "Источник"
            Case 3760
                Return "Импорт"
            Case 3800
                Return "Экспорт"
            Case 4130
                Return "Free space on disk"
            Case 4185
                Return "Идентификатор продукта"
            Case 4755
                Return "Start importing"
            Case 4832
                Return "Рецепт"
            Case 4834
                Return "Recipe Ingredients"
            Case 4854
                Return "Minimum"
            Case 4855
                Return "Maximum"
            Case 4856
                Return "From"
            Case 4860
                Return "Имя файла"
            Case 4862
                Return "Версия"
            Case 4865
                Return "Пользователи"
            Case 4867
                Return "Modify"
            Case 4870
                Return "Modify a user"
            Case 4877
                Return "Средний"
            Case 4890
                Return "Type of file"
            Case 4891
                Return "Предварительный просмотр"
            Case 5100
                Return "Единица измерения"
            Case 5105
                Return "Формат"
            Case 5270
                Return "Список товаров"
            Case 5350
                Return "Итог"
            Case 5390
                Return "serving"
            Case 5500
                Return "Номер"
            Case 5530
                Return "Установленная отпускная цена"
            Case 5590
                Return "Ингредиенты"
            Case 5600
                Return "Приготовление"
            Case 5610
                Return "Page"
            Case 5720
                Return "Величина"
            Case 5741
                Return "Брутто"
            Case 5795
                Return "На порцию"
            Case 5801
                Return "Прибыль"
            Case 5900
                Return "Ingredient category"
            Case 6000
                Return "Modify category"
            Case 6002
                Return "Название категории"
            Case 6055
                Return "Add text"
            Case 6390
                Return "Валюта"
            Case 6416
                Return "Фактор"
            Case 6470
                Return "?????????? ?????????"
            Case 7010
                Return "???"
            Case 7073
                Return "Просмотреть"
            Case 7181
                Return "All"
            Case 7183
                Return "Marked"
            Case 7270
                Return "Английский"
            Case 7296
                Return "Europe"
            Case 7335
                Return "All marks have been successfully deleted" ' "All marks have been successfully removed"
            Case 7570
                Return "Воскресенье"
            Case 7571
                Return "Понедельник"
            Case 7572
                Return "Вторник"
            Case 7573
                Return "Среда"
            Case 7574
                Return "Четверг"
            Case 7575
                Return "Пятница"
            Case 7576
                Return "Суббота"
            Case 7720
                Return "Packaging"
            Case 7725
                Return "Transportation"
            Case 7755
                Return "System"
            Case 8210
                Return "Вычисление"
            Case 8220
                Return "Procedure"
            Case 8395
                Return "Add"
            Case 8397
                Return "Delete"
            Case 8913
                Return "None"
            Case 8914
                Return "Десятичный"
            Case 8994
                Return "Tools"
            Case 9030
                Return "Updating"
            Case 9070
                Return "Not allowed in the demo version"
            Case 9140
                Return "Switzerland"
            Case 9920
                Return "Описание"
            Case 10103
                Return "Copy"
            Case 10104
                Return "Текст"
            Case 10109
                Return "Опции"
            Case 10116
                Return "Примечание"
            Case 10121
                Return "Search"
            Case 10125
                Return "Note"
            Case 10129
                Return "Выбор"
            Case 10130
                Return "On hand"
            Case 10131
                Return "Input"
            Case 10132
                Return "Output"
            Case 10135
                Return "Стиль"
            Case 10140
                Return "Stock"
            Case 10363
                Return "Налог"
            Case 10369
                Return "Supplier number"
            Case 10370
                Return "In order"
            Case 10399
                Return "Deleted"
            Case 10417
                Return "Failed:"
            Case 10430
                Return "Location"
            Case 10431
                Return "Inventory"
            Case 10468
                Return "Статус"
            Case 10513
                Return "Скидка"
            Case 10523
                Return "Тел."
            Case 10524
                Return "Факс"
            Case 10554
                Return "CCP описание"
            Case 10555
                Return "Время охлаждения"
            Case 10556
                Return "Время нагревания"
            Case 10557
                Return "Температура нагревания /градусы"
            Case 10558
                Return "Режим нагрева"
            Case 10572
                Return "Nutrient"
            Case 10573
                Return "Info1"
            Case 10970
                Return "Print"
            Case 10990
                Return "Supplier"
            Case 11040
                Return "Restore completed"
            Case 11280
                Return "Регистрация"
            Case 12515
                Return "Barcode"
            Case 12525
                Return "Invalid date"
            Case 13060
                Return "Нутриенты"
            Case 13255
                Return "History"
            Case 14070
                Return "Шрифт"
            Case 14090
                Return "Заголовок"
            Case 14816
                Return "Replace with"
            Case 14819
                Return "Replace"
            Case 14884
                Return "Updated items"
            Case 15360
                Return "Marked Menus"
            Case 15504
                Return "Администратор"
            Case 15510
                Return "Пароль"
            Case 15615
                Return "Enter your password"
            Case 15620
                Return "Confirmation"
            Case 16010
                Return "Расчет"
            Case 18460
                Return "Saving in progress"
            Case 20122
                Return "Company"
            Case 20200
                Return "Подрецепт"
            Case 20469
                Return "Specify the mailing method"
            Case 20530
                Return "Energy"
            Case 20703
                Return "Main"
            Case 20709
                Return "Units"
            Case 21570
                Return "Print a FAX form"
            Case 21600
                Return "of"
            Case 24002
                Return "Last order"
            Case 24016
                Return "Поставщик"
            Case 24027
                Return "Calculate"
            Case 24028
                Return "Отменить"
            Case 24044
                Return "Оба"
            Case 24050
                Return "New"
            Case 24085
                Return "Assign new"
            Case 24105
                Return "Display"
            Case 24121
                Return "Abbreviation"
            Case 24129
                Return "Transfer"
            Case 24150
                Return "Edit"
            Case 24152
                Return "Позиция"
            Case 24153
                Return "Город"
            Case 24163
                Return "Default location"
            Case 24260
                Return "This supplier cannot be deleted" ' "This supplier cannot be removed"
            Case 24270
                Return "Назад"
            Case 24271
                Return "Следующий"
            Case 24291
                Return "Промежуточный итог"
            Case 26000
                Return "Continue"
            Case 26100
                Return "Описание продукта"
            Case 26101
                Return "Совет по приготовлению / Консультация"
            Case 26102
                Return "Усовершенствование"
            Case 26103
                Return "Хранение"
            Case 26104
                Return "Прибыль/Продуктивность"
            Case 27000
                Return "Имя поручителя"
            Case 27020
                Return "Адрес"
            Case 27050
                Return "Phone number"
            Case 27055
                Return "Название заголовка"
            Case 27130
                Return "Payment"
            Case 27135
                Return "Дата конечного использования"
            Case 28000
                Return "Error in operation"
            Case 28008
                Return "Каталог не существует"
            Case 28655
                Return "No unit has been defined"
            Case 29170
                Return "Not available"
            Case 29771
                Return "Modify Ingredient"
            Case 30210
                Return "The operation failed"
            Case 30270
                Return "not found"
            Case 31085
                Return "Updated successfully"
            Case 31098
                Return "Сохранить"
            Case 31370
                Return "Затраты на продукты питания"
            Case 31375
                Return "ЗПП"
            Case 31380
                Return "Main"
            Case 31462
                Return "Ошибка"
            Case 31492
                Return "Our fax assistance service assures you a reply within one to 24 hours, depending on the problem encountered (except weekends)"
            Case 31755
                Return "Results"
            Case 31758
                Return "To"
            Case 31769
                Return "sold"
            Case 31800
                Return "День"
            Case 31860
                Return "Period"
            Case 51056
                Return "Продукт"
            Case 51086
                Return "Язык"
            Case 51092
                Return "Unit"
            Case 51097
                Return "EGS Enggist && Grandjean Software SA"
            Case 51098
                Return "Route de Soleure 12 / PO BOX"
            Case 51099
                Return "2072 St-Blaise, Switzerland"
            Case 51123
                Return "Детали"
            Case 51129
                Return "Wanted Ingredients"
            Case 51130
                Return "Unwanted Ingredients"
            Case 51139
                Return "Желаемый"
            Case 51157
                Return "Сообщение"
            Case 51178
                Return "Пожалуйста, повторите попытку."
            Case 51198
                Return "Connecting to SMTP server"
            Case 51204
                Return "Yes"
            Case 51243
                Return "Margin"
            Case 51244
                Return "Вверху"
            Case 51245
                Return "Bottom"
            Case 51246
                Return "Left"
            Case 51247
                Return "Справа"
            Case 51252
                Return "Download"
            Case 51257
                Return "Электронная почта"
            Case 51259
                Return "SMTP сервер"
            Case 51261
                Return "Имя пользователя"
            Case 51294
                Return "Прибыль"
            Case 51311
                Return "Invalid Unit"
            Case 51336
                Return "Нежелательный"
            Case 51353
                Return "Copyright Agreement"
            Case 51364
                Return "Do you accept the copyright agreement above and wish to proceed with the submission of the recipe?"
            Case 51377
                Return "Отправить электронное письмо"
            Case 51392
                Return "Единицы измерения прибыли"
            Case 51402
                Return "Are you sure you want to delete"
            Case 51500
                Return "Shopping List Details"
            Case 51502
                Return "Список покупок"
            Case 51532
                Return "Print shopping list"
            Case 51907
                Return "&Показать подробности"
            Case 52012
                Return "Browse"
            Case 52110
                Return "The selected file will be imported"
            Case 52130
                Return "New recipe"
            Case 52150
                Return "Сделанный"
            Case 52307
                Return "Закрыть"
            Case 52960
                Return "Simple"
            Case 52970
                Return "Complete"
            Case 53250
                Return "Export Selection"
            Case 54210
                Return "Не вносить изменения"
            Case 54220
                Return "Все прописными"
            Case 54230
                Return "Все строчными"
            Case 54240
                Return "Первую букву каждого слова делать прописной "
            Case 54245
                Return "Начинать с прописной"
            Case 54710
                Return "Selected Keywords"
            Case 54730
                Return "Ключевые слова"
            Case 55211
                Return "Соединение"
            Case 55220
                Return "Количество"
            Case 56100
                Return "Your Name"
            Case 56130
                Return "Country"
            Case 56500
                Return "Словарь"
            Case 101600
                Return "Модифицировать меню"
            Case 103150
                Return "Процент"
            Case 103215
                Return "Цена единицы продукции"
            Case 103305
                Return "Имя поручителя"
            Case 103306
                Return "Представитель"
            Case 104829
                Return "Список поставщиков"
            Case 104835
                Return "Создать новый продукт"
            Case 104854
                Return "Минимум"
            Case 104855
                Return "Максимум"
            Case 104862
                Return "Версия"
            Case 104869
                Return "Новый пользователь"
            Case 104870
                Return "Модифицировать пользователя"
            Case 105100
                Return "Единица измерения"
            Case 105110
                Return "Дата"
            Case 105200
                Return "Для"
            Case 105360
                Return "Отпускная цена за порцию"
            Case 106002
                Return "Название категории"
            Case 107183
                Return "Отмеченный"
            Case 110101
                Return "Модифицировать"
            Case 110102
                Return "Удалить"
            Case 110112
                Return "Распечатать"
            Case 110114
                Return "Помощь"
            Case 110129
                Return "Выбор"
            Case 110417
                Return "Failed:"
            Case 110524
                Return "Факс"
            Case 113275
                Return "Налог"
            Case 115610
                Return "Новый пароль принят"
            Case 121600
                Return "К"
            Case 124016
                Return "Поставщик"
            Case 124024
                Return "Принято"
            Case 124042
                Return "Тип"
            Case 124257
                Return "Магазин"
            Case 127010
                Return "Компания"
            Case 127040
                Return "Страна"
            Case 127050
                Return "Номер телефона"
            Case 127055
                Return "Наименование заголовка"
            Case 128000
                Return "Ошибка в операции"
            Case 131462
                Return "Ошибка"
            Case 131757
                Return "Из"
            Case 132552
                Return "Всего налогов"
            Case 132554
                Return "Модифицировать рецепт"
            Case 132555
                Return "Добавить рецепт"
            Case 132557
                Return "Создать новое меню"
            Case 132559
                Return "Создать новый товар"
            Case 132561
                Return "Пожалуйста введите серийный номер, наименование заголовка и ключ продукта. Вы найдете эту информацию в сопроводительных документах RecipeNet."
            Case 132565
                Return "Дополнение"
            Case 132567
                Return "Категория товаров"
            Case 132568
                Return "Категория рецептов"
            Case 132569
                Return "Категория меню"
            Case 132570
                Return "Невозможно удалить"
            Case 132571
                Return "Категория используется."
            Case 132589
                Return "Максимальное число рецептов"
            Case 132590
                Return "Текущий номер рецептов"
            Case 132592
                Return "Максимальное количество товаров"
            Case 132593
                Return "Текущее количество товаров"
            Case 132597
                Return "Создать новый рецепт"
            Case 132598
                Return "Максимальное количество меню"
            Case 132599
                Return "Текущее количество меню"
            Case 132600
                Return "Назначит ключевые слова"
            Case 132601
                Return "Переместить маркированные в новую категорию"
            Case 132602
                Return "Удалить маркированные"
            Case 132605
                Return "Список покупок"
            Case 132607
                Return "Маркировка дейстия"
            Case 132614
                Return "Чистый вес"
            Case 132615
                Return "Права"
            Case 132616
                Return "Владелец"
            Case 132621
                Return "Модифицировать источник"
            Case 132630
                Return "Автоматическое преобразование"
            Case 132638
                Return "Информация о пользователе"
            Case 132640
                Return "Имя пользователя уже используется."
            Case 132654
                Return "Управление базой данных"
            Case 132657
                Return "&Восстановить"
            Case 132667
                Return "Объединить"
            Case 132668
                Return "Очистить"
            Case 132669
                Return "Поднять"
            Case 132670
                Return "Опустить"
            Case 132671
                Return "Стандартизировать"
            Case 132678
                Return "HACCP"
            Case 132683
                Return "Предыдущий"
            Case 132706
                Return "Питательная ценность на 100г или 100мл"
            Case 132714
                Return "Пожалуйста, выберите из списка."
            Case 132719
                Return "Цена для этой велчины уже определена."
            Case 132723
                Return "Общие потери не могут быть больше или равны 100%."
            Case 132736
                Return "Масса брутто"
            Case 132737
                Return "Добавить нового поставщика"
            Case 132738
                Return "Модифицировать поставщика"
            Case 132739
                Return "Детали поставщика"
            Case 132740
                Return "Область"
            Case 132741
                Return "URL"
            Case 132779
                Return "Ключевое слово используется."
            Case 132783
                Return "Ключевое слово"
            Case 132788
                Return "Связанные нутриенты"
            Case 132789
                Return "&Вход в систему"
            Case 132813
                Return "&Конфигурация"
            Case 132828
                Return "Пересчитать &нутриенты"
            Case 132841
                Return "Добавить товары"
            Case 132846
                Return "Сохранить маркировки"
            Case 132847
                Return "Загрузить маркировки"
            Case 132848
                Return "Фильтр"
            Case 132855
                Return "Добавить меню"
            Case 132860
                Return "Добавить ингредиент"
            Case 132864
                Return "Заменить ингредиент"
            Case 132865
                Return "Добавить разделитель"
            Case 132877
                Return "Добавить пункт"
            Case 132896
                Return "Стандартизировать категории"
            Case 132912
                Return "Стандартизировать тексты"
            Case 132915
                Return "Стандартизировать величины"
            Case 132924
                Return "Стандартизировать величины прибыли"
            Case 132930
                Return "Сильно уменьшенное изображение"
            Case 132933
                Return "Список рецептов"
            Case 132939
                Return "Список меню"
            Case 132954
                Return "Набор маркеров"
            Case 132955
                Return "Выберите название маркера из списка, или новый тип маркера, чтобы сохранить"
            Case 132957
                Return "Сохранить маркированные как"
            Case 132967
                Return "Нутриент"
            Case 132971
                Return "Обзор нутриентов"
            Case 132972
                Return "Питательная ценность на порцию на 100%"
            Case 132974
                Return "Отходы"
            Case 132987
                Return "Обзор"
            Case 132989
                Return "Вывести на экран"
            Case 132997
                Return "В или до"
            Case 132998
                Return "В или после"
            Case 132999
                Return "Между"
            Case 133000
                Return "Больше чем"
            Case 133001
                Return "Меньше чем"
            Case 133005
                Return "Установлено"
            Case 133023
                Return "Вывести на экран опции"
            Case 133043
                Return "Трансформация локальных рисунков"
            Case 133045
                Return "Максимальный размер файла рисунка"
            Case 133046
                Return "Максимальный размер рисунка"
            Case 133047
                Return "Оптимизация"
            Case 133049
                Return "Активировать автоматическое преобразование рисунков для использования на сайте"
            Case 133057
                Return "Переслать логотип для веб-страницы"
            Case 133060
                Return "Веб-цвета"
            Case 133075
                Return "Новый пароль"
            Case 133076
                Return "Подтвердить новый пароль"
            Case 133080
                Return "Последний"
            Case 133081
                Return "Первый"
            Case 133085
                Return "Вывести документ"
            Case 133096
                Return "Приготовление рецепта"
            Case 133097
                Return "Калькулирование рецепта"
            Case 133099
                Return "Вариация"
            Case 133100
                Return "Детали рецепта"
            Case 133101
                Return "Детали меню"
            Case 133108
                Return "Что должно быть распечатано?"
            Case 133109
                Return "Выбор товаров для распечатки"
            Case 133111
                Return "Некоторые категории"
            Case 133112
                Return "Маркированные товары"
            Case 133116
                Return "Маркированные рецепты"
            Case 133121
                Return "Маркированные меню"
            Case 133123
                Return "Калькулирование меню"
            Case 133124
                Return "Описание меню"
            Case 133126
                Return "EGS стандарт"
            Case 133127
                Return "EGS модерн"
            Case 133128
                Return "EGS две колонки"
            Case 133133
                Return "Недействительное имя файла.Пожалуйста введите действительное имя файла."
            Case 133144
                Return "Номер рецепта"
            Case 133161
                Return "Размер бумаги"
            Case 133162
                Return "Единицы измерения полей"
            Case 133163
                Return "Левое поле"
            Case 133164
                Return "Правое поле"
            Case 133165
                Return "Верхнее поле"
            Case 133166
                Return "Нижнее поле"
            Case 133168
                Return "Размер шрифта"
            Case 133172
                Return "Маленький рисунок / Количество – Название"
            Case 133173
                Return "Маленький рисунок / Название – Количество"
            Case 133174
                Return "Средний рисунок / Количество – Название"
            Case 133175
                Return "Средний рисунок / Название – Количество"
            Case 133176
                Return "Большой рисунок / Количество – Название"
            Case 133177
                Return "Большой рисунок / Название – Количество"
            Case 133196
                Return "Опции списка"
            Case 133201
                Return "Следующие товары используются и не будут удалены."
            Case 133207
                Return "Рецепт может быть использован в качестве подрецепта"
            Case 133208
                Return "Вес"
            Case 133222
                Return "Опции деталей"
            Case 133230
                Return "Следующий(ие) рецепт(ы) используется(ются) и не будет(ут) удален(ы)."
            Case 133241
                Return "Пересчет цен. Пожалуйста, подождите..."
            Case 133242
                Return "Пересчет питательной ценности. Пожалуйста, подождите..."
            Case 133251
                Return "Разделительный знак"
            Case 133254
                Return "Сортировать по"
            Case 133260
                Return "Источник используется."
            Case 133266
                Return "Стандартизировать ключевые слова"
            Case 133286
                Return "Описание"
            Case 133289
                Return "Единица измерения используется."
            Case 133290
                Return "Вы не можете управлять двумя или более системными единицами измерения."
            Case 133295
                Return "Эту единицу измерения нельзя удалить. ¶Только определяемые пользователем единицы измерения могут быть удалены."
            Case 133314
                Return "Только определяемые пользователем единицы измерения прибыли могут быть удалены."
            Case 133315
                Return "Вы не можете управлять двумя или более системными единицами измерения прибыли."
            Case 133319
                Return "Единица измерения прибыли используется."
            Case 133325
                Return "Вы уверены, что хотите очистить все неиспользуемые категории?"
            Case 133326
                Return "Нет источника"
            Case 133330
                Return "Неустановлен файл."
            Case 133349
                Return "Номер меню"
            Case 133350
                Return "Пункты для %y (чистое количество)"
            Case 133351
                Return "Ингредиенты для %y" ' в %p% (чистое количество)"
            Case 133352
                Return "Установленная отпускная цена на порцию + Налог"
            Case 133353
                Return "Установленная отпускная цена на порцию"
            Case 133359
                Return "Отсортировано по номеру"
            Case 133360
                Return "Отсортировано по дате"
            Case 133361
                Return "Отсортировано по категории"
            Case 133365
                Return "Отпускная цена + Налог"
            Case 133367
                Return "Отсортировано по поставщику"
            Case 133405
                Return "Upload Digital assets" '"Переслать рисунки"
            Case 133519
                Return "Select a Color :"
            Case 133692
                Return "Рекомендуемая цена"
            Case 134032
                Return "Контакт"
            Case 134055
                Return "Закупка"
            Case 134056
                Return "Объем продаж"
            Case 134061
                Return "Версия, модули & лицензии"
            Case 134083
                Return "????"
            Case 134111
                Return "Невозможно удалить маркированные пункты."
            Case 134176
                Return "Список товары-нутриенты"
            Case 134177
                Return "Список рецепты-нутриенты"
            Case 134178
                Return "Список меню-нутриенты"
            Case 134182
                Return "Группа"
            Case 134194
                Return "Неправильное количество"
            Case 134195
                Return "Неправильная цена"
            Case 134320
                Return "Адрес для выставления счета"
            Case 134332
                Return "Информация"
            Case 134333
                Return "Важный"
            Case 134525
                Return "Вы уверены, что хотите отменить сделанные изменения?"
            Case 134571
                Return "Неправильная величина"
            Case 135056
                Return "Правила нутриентов"
            Case 135058
                Return "Добавить правило нутриента"
            Case 135059
                Return "Модифицировать правило нутриента"
            Case 135070
                Return "Нетто"
            Case 135256
                Return "Проданное количество"
            Case 135608
                Return "Порт"
            Case 135948
                Return "Включить подрецепт(ы)"
            Case 135955
                Return "Неправильная числовая величина."
            Case 135963
                Return "База данных"
            Case 135967
                Return "Заменить в рецептах."
            Case 135968
                Return "Заменить в меню."
            Case 135971
                Return "&Соединение"
            Case 135978
                Return "Новый"
            Case 135979
                Return "Переименовать"
            Case 135985
                Return "Существующий"
            Case 135986
                Return "Недостающий"
            Case 135989
                Return "Пункты"
            Case 135990
                Return "Обновить"
            Case 136018
                Return "Право собственности"
            Case 136025
                Return "Преобразовать базу данных"
            Case 136171
                Return "Изменить единицу измерения"
            Case 136265
                Return "Подрецепт"
            Case 136601
                Return "Восстановить"
            Case 136905
                Return "Текущий символ"
            Case 137019
                Return "Изменить"
            Case 137030
                Return "По умолчанию"
            Case 137070
                Return "Общие установочные параметры"
            Case 138137
                Return "Удаленный"
            Case 138244
                Return "Пункт продаж"
            Case 138402
                Return "Все перемещения успешно сделаны"
            Case 138412
                Return "<не определено>"
            Case 140056
                Return "Файл"
            Case 140100
                Return "????????? ??????????? ???????????"
            Case 140101
                Return "?????????????? ???????????"
            Case 140129
                Return "?????? ??? ?????????????? ????????? ?????"
            Case 140130
                Return "?????? ??? ???????? ????????? ?????"
            Case 140180
                Return "???? ??? ?????????? ????????? ????? ??????"
            Case 143001
                Return "Сделать общими"
            Case 143002
                Return "Не делать общими"
            Case 143008
                Return "Убытки"
            Case 143013
                Return "Модификация"
            Case 143014
                Return "Пользователь"
            Case 143508
                Return "Рецепт используется как подрецепт"
            Case 143509
                Return "Междустрочный интервал"
            Case 143987
                Return "Тип пункта"
            Case 143995
                Return "Действие"
            Case 144591
                Return "Время"
            Case 144682
                Return "Питательная ценность на 100 г или 100 мл на 100%"
            Case 144684
                Return "Питательная ценность на 1 единицу прибыли на 100%"
            Case 144685
                Return "На единицу прибыли на 100%"
            Case 144686
                Return "на %Y на 100%"
            Case 144687
                Return "на 100 г или 100 мл на 100%"
            Case 144688
                Return "Не доступно"
            Case 144689
                Return "Питательная ценность на 1 единицу прибыли/100 г или 100 мл на 100%"
            Case 144716
                Return "История"
            Case 144734
                Return "Список пунктов продаж"
            Case 144738
                Return "Вес на %Y"
            Case 145006
                Return "Перемещение"
            Case 146056
                Return "Маржинальная прибыль"
            Case 146067
                Return "Сальдо"
            Case 146080
                Return "Клиент"
            Case 146114
                Return "Показывать на новой странице другого поставщика"
            Case 146211
                Return "Расходы"
            Case 147070
                Return "ОК"
            Case 147075
                Return "Неправильная дата"
            Case 147126
                Return "Удалить сначала существующие пометки"
            Case 147174
                Return "Открыть"
            Case 147441
                Return "Этот пункт продажи уже подключен."
            Case 147462
                Return "Соотношение"
            Case 147520
                Return "Главный"
            Case 147647
                Return "SQL ?????? ?? ??????????, ??? ?????? ? ???? ????????"
            Case 147652
                Return "Удалить"
            Case 147692
                Return "Информация о приеме пищи"
            Case 147699
                Return "Перезаписать"
            Case 147700
                Return "Общая цена"
            Case 147703
                Return "Количество порций для приготовления"
            Case 147704
                Return "Оставшееся в наличии количество"
            Case 147706
                Return "Возвращенное количество"
            Case 147707
                Return "Потерянное количество"
            Case 147708
                Return "Проданное количество"
            Case 147710
                Return "Проданное количество (спец.)"
            Case 147713
                Return "Разработка EGS"
            Case 147727
                Return "Стоимость"
            Case 147729
                Return "Рейтинг"
            Case 147733
                Return "??????? ????"
            Case 147737
                Return "Тип количества и выбрать единицу измерения"
            Case 147743
                Return "Переслать"
            Case 147753
                Return "Затраты на рабочую силу"
            Case 147771
                Return "Оценка/Часы"
            Case 147772
                Return "Оценка/Минуты"
            Case 147773
                Return "Человек"
            Case 147774
                Return "Время (часы:минуты)"
            Case 149501
                Return "Использовать прямой Ввод-Вывод"
            Case 149513
                Return "Подтверждение"
            Case 149531
                Return "Готовые продукты"
            Case 149645
                Return "Соединиться с"
            Case 149706
                Return "Удалить соединение"
            Case 149766
                Return "Префикс"
            Case 149774
                Return "Очистить"
            Case 150333
                Return "Successfully deleted!" ' "Successfully removed!"
            Case 150341
                Return "Currency Conversion"
            Case 150353
                Return "Sort"
            Case 150634
                Return "Электронное письмо успешно отправлено"
            Case 150644
                Return "SMTP сервер необходим для отправки электронных писем с Вашего компьютера. "
            Case 150688
                Return "Лицензия на использование этого приложения уже утратила силу в связи с истечением срока."
            Case 150707
                Return "Счет"
            Case 151011
                Return "Switzerland - Headquarter"
            Case 151019
                Return "Ключевое слово товара"
            Case 151020
                Return "Ключевое слово рецепта"
            Case 151023
                Return "??????????????"
            Case 151250
                Return "Ничего не было изменено"
            Case 151286
                Return "Standard"
            Case 151299
                Return "Пожалуйста введите запрашиваемую информацию"
            Case 151322
                Return "Include in Inventory"
            Case 151336
                Return "Load a set of marks"
            Case 151344
                Return "Save marks for ingredient"
            Case 151345
                Return "Save marks for dishes"
            Case 151346
                Return "Save marks for menus"
            Case 151364
                Return "Select two or more texts"
            Case 151389
                Return "Очистить текст"
            Case 151400
                Return "Стоимость товаров"
            Case 151404
                Return "НДС"
            Case 151424
                Return "Преобразовать в наилучшие единицы измерения"
            Case 151427
                Return "Отсортировано по названию пункта"
            Case 151435
                Return "Тема"
            Case 151437
                Return "RecipeNet"
            Case 151438
                Return "CALCMENU"
            Case 151459
                Return "Адрес Вашей электронной почты"
            Case 151499
                Return "Заменить предложение"
            Case 151854
                Return "Excel"
            Case 151906
                Return "E-mail address not found"
            Case 151907
                Return "Please log in your appropriate username and password."
            Case 151910
                Return "Sign In"
            Case 151911
                Return "Sign Out"
            Case 151912
                Return "Forgot Your Password?"
            Case 151915
                Return "Please provide the information requested below."
            Case 151916
                Return "Fields with asterisks (*) are required."
            Case 151918
                Return "Please provide a valid e-mail address."
            Case 151976
                Return "Место хранения продукции по умолчанию"
            Case 152004
                Return "Ветвление"
            Case 152141
                Return "Ingredient Management"
            Case 152146
                Return "Почтовый индекс"
            Case 155024
                Return "Pictures Management"
            Case 155046
                Return "Перевод"
            Case 155052
                Return "Подчинить"
            Case 155118
                Return "Send Shopping List to Pocket"
            Case 155163
                Return "Last Name"
            Case 155170
                Return "Welcome %name!"
            Case 155205
                Return "Home"
            Case 155225
                Return "PDF"
            Case 155236
                Return "Main Language"
            Case 155245
                Return "About Us"
            Case 155263
                Return "Пиксел"
            Case 155264
                Return "Translate"
            Case 155374
                Return "Бухгалтерский номер"
            Case 155507
                Return "Активировать"
            Case 155575
                Return "По умолчанию автоматический ввод места хранения товаров   "
            Case 155601
                Return "Нет выбранных пунктов."
            Case 155642
                Return "Обмен рецептами"
            Case 155713
                Return "%r существует."
            Case 155731
                Return "CALCMENU Pro"
            Case 155763
                Return "Сравнить по номеру"
            Case 155764
                Return "Сравнить поназванию"
            Case 155841
                Return "???? ??? ??????????????"
            Case 155862
                Return "На"
            Case 155942
                Return "Загрузить сохраненый список покупок"
            Case 155967
                Return "разделитель полей"
            Case 155994
                Return "Не активный"
            Case 155996
                Return "Адрес электронной почты"
            Case 156000
                Return "Перейти к новому поставщику"
            Case 156012
                Return "Поддержка"
            Case 156015
                Return "Contact Us"
            Case 156016
                Return "Main Office"
            Case 156141
                Return "Резервное копирование/Восстановление базы данных"
            Case 156337
                Return "Соединение нутриента"
            Case 156344
                Return "Неправильный выбор"
            Case 156355
                Return "Archives"
            Case 156356
                Return "Включить"
            Case 156405
                Return "Please free some space then click Retry"
            Case 156413
                Return "Sub-Recipe Definition"
            Case 156485
                Return "Удалить файлы после импортирования"
            Case 156552
                Return "??????? ????????? ??????????? ??????"
            Case 156590
                Return "Импортировать товары из CSV файла (Excel)"
            Case 156669
                Return "Веб-страница"
            Case 156672
                Return "Используемые онлайн (для веб содержимого)"
            Case 156683
                Return "Оригинальный"
            Case 156720
                Return "Номер слишком длинный"
            Case 156721
                Return "Имя слишком длинное"
            Case 156722
                Return "Имя поставщика слишком длинное"
            Case 156723
                Return "Название категории слишком длинное"
            Case 156725
                Return "Описание слишком длинное"
            Case 156734
                Return "Две величины идентичны"
            Case 156742
                Return "Окончит функционирование после"
            Case 156751
                Return "Tel:  +41 848 000 357<br>(English, French, German, Operating hours: 8:30am-6pm GMT +01:00)<br><br>Tel:  +41 32 544 00 17<br>(English ONLY, Operating hours: 3am-830am GMT +01:00)"
            Case 156752
                Return "Toll Free:  1-800-964-9357<br>(English ONLY, Operating hours: 9am-3am Pacific Standard Time)"
            Case 156753
                Return "Tel: +63 2 687 3179<br>(English ONLY, Operating hours: 12am-6pm GMT +08:00) "
            Case 156754
                Return "Имя файла"
            Case 156825
                Return "Тысяча"
            Case 156870
                Return "Вы уверены?"
            Case 156925
                Return "Загрузка успешна!"
            Case 156938
                Return "Active"
            Case 156941
                Return "Pocket Kitchen"
            Case 156955
                Return "Private"
            Case 156957
                Return "Hotels"
            Case 156959
                Return "Shared"
            Case 156960
                Return "Submitted"
            Case 156961
                Return "Set Of Price"
            Case 156962
                Return "Not Submitted"
            Case 156963
                Return "Prices"
            Case 156964
                Return "Find in"
            Case 156965
                Return "Yields"
            Case 156966
                Return "Records affected"
            Case 156967
                Return "Please enter the correct date."
            Case 156968
                Return "Invalid image file format"
            Case 156969
                Return "Please enter the image file to upload. Otherwise, leave it blank."
            Case 156970
                Return "Enter Category Information"
            Case 156971
                Return "Enter Set Price Information"
            Case 156972
                Return "Enter Keyword Information"
            Case 156973
                Return "Enter Unit Information"
            Case 156974
                Return "Enter Yield Information"
            Case 156975
                Return "Create new recipes and submit to the main office for use with other hotels."
            Case 156976
                Return "Ingredient is the basic element or item that comprises your recipes and menus."
            Case 156977
                Return "Should you have any inquiries or technical questions about this software."
            Case 156978
                Return "Parent Keyword"
            Case 156979
                Return "Name of Keyword"
            Case 156980
                Return "Configuration"
            Case 156981
                Return "Tax Rates"
            Case 156982
                Return "Search Results"
            Case 156983
                Return "Sorry, no results were found."
            Case 156984
                Return "???????????? ??? ???????????? ??? ??????."
            Case 156986
                Return "The item already exists."
            Case 156987
                Return "was saved successfully."
            Case 156996
                Return "Copyright © 2004 of EGS Enggist & Grandjean Software SA, Switzerland."
            Case 157002
                Return "Price for the unit is not defined. Please select a unit."
            Case 157020
                Return "Tax used"
            Case 157026
                Return "Medium"
            Case 157033
                Return "The system will update the prices of all ingredient. Please wait..."
            Case 157034
                Return "Аутентификация"
            Case 157038
                Return "Месяц"
            Case 157039
                Return "Год"
            Case 157040
                Return "There's no keyword available."
            Case 157041
                Return "Access denied"
            Case 157049
                Return "Are you sure you want to save?"
            Case 157055
                Return "СТУДЕНЧЕСКАЯ ВЕРСИЯ"
            Case 157056
                Return "Вы хотите отменить?"
            Case 157057
                Return "Marked items are now shared."
            Case 157076
                Return "Help Summary"
            Case 157079
                Return "The following marked items are not submitted and cannot be transferred:"
            Case 157084
                Return "The following marked items are being used and are not deleted:" '  "The following marked items are being used and are not removed:"
            Case 157125
                Return "Views"
            Case 157130
                Return "Your credit card information has been sent successfully. Your subscription will be processed within three days. Thank you!"
            Case 157132
                Return "Personal (Shared)"
            Case 157133
                Return "Personal (Not Shared)"
            Case 157134
                Return "Visitor"
            Case 157136
                Return "Кредиты"
            Case 157139
                Return "Worst!"
            Case 157140
                Return "Good!"
            Case 157141
                Return "Fantastic!"
            Case 157142
                Return "Удалить неиспользуемые единицы товаров перед импортом"
            Case 157151
                Return "Other links"
            Case 157152
                Return "User Reviews"
            Case 157153
                Return "The recipient will be prompted to accept these items."
            Case 157154
                Return "The following items cannot be given because they are owned by other users."
            Case 157155
                Return "Someone would like to give you the following recipes:"
            Case 157156
                Return "Promo"
            Case 157157
                Return "User Opinions"
            Case 157158
                Return "Originality"
            Case 157159
                Return "Result"
            Case 157160
                Return "Difficulty"
            Case 157161
                Return "Recipe of the day"
            Case 157164
                Return "Cardholder name"
            Case 157165
                Return "Credit card number"
            Case 157166
                Return "Record Limit"
            Case 157168
                Return "Bank"
            Case 157169
                Return "PayPal"
            Case 157170
                Return "Online ordering is not available in your country."
            Case 157171
                Return "Become a member"
            Case 157172
                Return "Upgrade fee"
            Case 157173
                Return "Subscription fee"
            Case 157174
                Return "Upgrade packs"
            Case 157176
                Return "Total records used"
            Case 157177
                Return "We offer a variety of solutions to fit your needs"
            Case 157178
                Return "Trial user"
            Case 157179
                Return "Tell a Friend"
            Case 157180
                Return "Friend's e-mail address"
            Case 157182
                Return "FAQs"
            Case 157183
                Return "Terms and Condition of Service"
            Case 157214
                Return "Создать список покупок только для отмеченных рецептов"
            Case 157217
                Return "Создать список покупок только для отмеченных меню"
            Case 157226
                Return "Marked recipes have been sent for approval."
            Case 157233
                Return "Wastage cannot be greater than or equal to 100%."
            Case 157268
                Return "Currency used."
            Case 157269
                Return "Set of price is being used."
            Case 157273
                Return "Cannot share the following items because they were neither submitted nor owned."
            Case 157274
                Return "Exchange Rate"
            Case 157275
                Return "All items listed will be merged into one. Please select an item to be used by users. Other items will be deleted from the database."
            Case 157276
                Return "Successfully merged."
            Case 157277
                Return "Total Cost"
            Case 157297
                Return "Please select at least one item."
            Case 157299
                Return "Edit profile and customize your view."
            Case 157300
                Return "Please enter your new password. A password cannot exceed 20 characters. Click 'Submit' when you are done."
            Case 157301
                Return "Please enter the image file (jpeg/jpg , bmp, etc.) that you want to upload. Otherwise, leave it blank. (Note: GIF file is not supported. All pictures are copied and then converted to normal and thumbnail jpeg format. )"
            Case 157302
                Return "Search ingredient by name or a part of the name (use [*] asterisk). To add quickly, enter [net quanitity]_[unit]_[ingredient] like 200 g Oel High Oleic"
            Case 157303
                Return "To add or edit the ingredient price, enter the new price and define the unit of measurement. Assign the ratio of that unit to the original unit. For example, the original price and unit is US $11 per kilogram (kg). If you want to add the unit bag, you have to define the price of that bag, or define how many kilograms there are in 1 bag (ratio)."
            Case 157304
                Return "Search keywords by name or a part of the name. Use comma [ , ] for multiple keywords. For example, search ''beef, sauce, wedding''."
            Case 157305
                Return "Please select an item"
            Case 157306
                Return "Неправильный тип файла."
            Case 157310
                Return "Детали товаров"
            Case 157314
                Return "Использовать главную единицу измерения при добавлении цены на товары"
            Case 157320
                Return "Совместное использование"
            Case 157322
                Return "User Agreement"
            Case 157323
                Return "Give"
            Case 157329
                Return "Терминал"
            Case 157334
                Return "Warning: You might lose all your changes if another user has modified this record. Do you want to refresh this page?"
            Case 157339
                Return "Messages per Page"
            Case 157340
                Return "Quick browse"
            Case 157341
                Return "on each page"
            Case 157342
                Return "Record was modified by another user.  Click OK to proceed."
            Case 157343
                Return "This record was deleted by another user." ' "This record was removed by another user."
            Case 157345
                Return "Submit to Head Office"
            Case 157346
                Return "Not shared"
            Case 157378
                Return "Member"
            Case 157379
                Return "Subscribe now"
            Case 157380
                Return "Your subscription will expire on %n."
            Case 157381
                Return "Your subscription has expired."
            Case 157382
                Return "Extend my membership using my remaining points (credits)"
            Case 157383
                Return "You've reached your disk space limit. Please delete some of your recipes or ingredient. Thank you."
            Case 157384
                Return "Invalid transaction"
            Case 157385
                Return "Thank you!"
            Case 157387
                Return "You will be redirected to PayPal to complete your subscription. Please take a moment to choose which currency to use in order to charge you the correct amount. Please choose from the list below."
            Case 157388
                Return "An invitation to join"
            Case 157404
                Return "Pending transaction."
            Case 157405
                Return "For inquiries, please e-mail us at"
            Case 157408
                Return "Only members and trial users can access this page. Do you want to manage your own recipe in Recipe Gallery.com?  Go to the subscription menu and subscribe as a member."
            Case 157435
                Return "Автоматическое перемещение в магазин перед выводом"
            Case 157437
                Return "Сырье"
            Case 157446
                Return "Месяц(ы)"
            Case 157594
                Return "Принимать"
            Case 157595
                Return "Deny"
            Case 157596
                Return "No User Review"
            Case 157604
                Return "E-mail Support"
            Case 157607
                Return "Phone Support"
            Case 157608
                Return "Online Support"
            Case 157616
                Return "USA"
            Case 157617
                Return "ASIA and the Rest of the World"
            Case 157629
                Return "Одобрить"
            Case 157633
                Return "Не одобрено"
            Case 157695
                Return "Номер счета"
            Case 157772
                Return "??????????????"
            Case 157802
                Return "Confirm Password"
            Case 157901
                Return "Скрыть существующие"
            Case 157926
                Return "Sign Up"
            Case 158005
                Return "License"
            Case 158019
                Return "Check Request Status"
            Case 158169
                Return "Kindly choose your payment terms.¶¶Advance Payment via:"
            Case 158170
                Return "Kindly e-mail us your credit card details at <a href='mailto:info@calcmenu.com'>info@calcmenu.com</a>. Credit Card Type (Visa, Mastercard, American Express), Cardholder's Name, Credit Card Number (Please include the 3-digit security code (CVC2/CVV2) which you can find at the back of your card) and Expiry Date."
            Case 158171
                Return "Bank/Wire Transfer"
            Case 158174
                Return "<b>Note:</b> Please advise us once the transfer has been made. It will take 1-2 weeks before we receive our bank confirmation regarding the transfer."
            Case 158186
                Return "Change Password"
            Case 158220
                Return "Create new ingredient name with up to 250 characters and include alphanumeric reference number, tax rate, four wastage percentages, category, supplier, and other helpful information such as product description, preparation, cooking tip, refinement methods, and storage."
            Case 158229
                Return "Pictures"
            Case 158230
                Return "Ingredient, Recipes, and Menus can be searched using their name or reference numbers. You can also search using categories and keywords. For the ingredient, you can also use supplier, date encoded or last modified, price range, and nutrient values when searching. For the recipes and menus, you can search using items used and not used."
            Case 158232
                Return "Action Marks are shortcuts in performing a similar function that could apply to a marked ingredient, recipe or menu. You can use action marks to assign ingredient, recipe, or menu to a category and keywords, delete them, export, send via e-mail, print, share, and unshare to other users without having to repeat them for each item. This saves you a lot of time and effort in performing an action to the marked items."
            Case 158234
                Return "Nutrient Linking and Calculation"
            Case 158238
                Return "Supplier Management"
            Case 158240
                Return "Category, Keywords, Sources Management"
            Case 158243
                Return "Tax Rate Management"
            Case 158246
                Return "Unit Management"
            Case 158249
                Return "Printing, PDF and Excel Export"
            Case 158306
                Return "Select"
            Case 158346
                Return "more"
            Case 158376
                Return "Теоретическая установленная отпускная цена"
            Case 158511
                Return "If you believe this is not the case, please send us an e-mail <a href='mailto:%email'>%email</a>"
            Case 158577
                Return "Site Language"
            Case 158585
                Return "Headoffice"
            Case 158588
                Return "Cannot submit the following items because they are owned by another user."
            Case 158653
                Return "Мобильный"
            Case 158677
                Return "Номер¶пункта продаж"
            Case 158694
                Return "Change Info"
            Case 158696
                Return "For Philippine Clients only"
            Case 158730
                Return "Удалить"
            Case 158783
                Return "Включить рецепт(ы)/подрецепт(ы)"
            Case 158810
                Return "Calculate Price"
            Case 158835
                Return "Отсортировано по налогу"
            Case 158837
                Return "Отсортировано по цене"
            Case 158839
                Return "Отсортировано по стоимости товаров"
            Case 158840
                Return "Отсортировано по константе"
            Case 158845
                Return "Отсортировано по отпускной цене"
            Case 158846
                Return "Отсортировано по установленной цене"
            Case 158849
                Return "Высокий"
            Case 158850
                Return "Низкий"
            Case 158851
                Return "Создано"
            Case 158860
                Return "Модифицировать настройки Кассы"
            Case 158902
                Return "Время открытия"
            Case 158912
                Return "Запросы"
            Case 158935
                Return "Общий доход"
            Case 158947
                Return "You will be redirected to Paypal to complete your order."
            Case 158952
                Return "Утвержденные"
            Case 158953
                Return "Не утвержденные"
            Case 158960
                Return "This function has been disabled. Please contact your head office if you need new recipes."
            Case 158998
                Return "Search Features"
            Case 158999
                Return "Ingredient, recipe, and menu lists can be printed together with their details, prices, and nutrient values. Shopping lists or the list of ingredients together with cumulative quantities used in various recipes can also be printed. PDF and Excel files can also be created for the various reports."
            Case 159000
                Return "Set of Price and Multiple Currency Management"
            Case 159009
                Return "Граница"
            Case 159035
                Return "Неполный"
            Case 159064
                Return "Название не может быть пустым"
            Case 159082
                Return "Обновить продукты, базируясь на дате последней модификации"
            Case 159089
                Return "Отменить запрос на утверждение"
            Case 159112
                Return "На утверждение"
            Case 159113
                Return "Наследственный"
            Case 159133
                Return "Shipping Information"
            Case 159139
                Return "Составление"
            Case 159140
                Return "Единица измерения слишком длинная"
            Case 159141
                Return "Единица измерения %n не существует."
            Case 159142
                Return "%n не может быть незаполненным."
            Case 159144
                Return "Импорт файла. Пожалуйста подождите..."
            Case 159145
                Return "Сохранение пункта. Пожалуйста подождите..."
            Case 159162
                Return "&Скрыть подробности"
            Case 159168
                Return "Отсортировано по чистому количеству"
            Case 159169
                Return "Отсортировано по валовому количеству"
            Case 159171
                Return "План работ"
            Case 159181
                Return "Отсортировано по количеству"
            Case 159264
                Return "Импорт товаров CSV/сеть поставщика"
            Case 159273
                Return "Общая маржинальная прибыль"
            Case 159275
                Return "Limited by licenses"
            Case 159298
                Return "Ключевое слово меню"
            Case 159349
                Return "Обнулить фильтр"
            Case 159360
                Return "Property Chef"
            Case 159361
                Return "Executive Chef"
            Case 159362
                Return "Selected item being used."
            Case 159363
                Return "Enter brand information"
            Case 159364
                Return "Brand"
            Case 159365
                Return "Role"
            Case 159366
                Return "Using SMTP on server"
            Case 159367
                Return "Using SMTP on the network"
            Case 159368
                Return "Logo"
            Case 159369
                Return "Сравнить по ..."
            Case 159370
                Return "Успешно импортировано"
            Case 159372
                Return "Global"
            Case 159379
                Return "ascending"
            Case 159380
                Return "descending"
            Case 159381
                Return "Expose to all users"
            Case 159382
                Return "Convert to System Recipe"
            Case 159383
                Return "Do not expose"
            Case 159384
                Return "Property"
            Case 159385
                Return "Submit entry"
            Case 159386
                Return "Prices and nutrients were not recalculated."
            Case 159387
                Return "Prices and nutrients were recalculated."
            Case 159388
                Return "Create a New Menu Card"
            Case 159389
                Return "Modify Menu Card"
            Case 159390
                Return "E-mail sent."
            Case 159391
                Return "Approved Price"
            Case 159424
                Return "This function has been disabled. Please contact your head office if you need new ingredient."
            Case 159426
                Return "Search ingredient by name or part of the name. To add quickly, enter [net quanitity]_[unit]_[ingredient]."
            Case 159430
                Return "??????????????? ?????????? ???? ??????? ?????????."
            Case 159433
                Return "Предложить системе"
            Case 159434
                Return "Submitted to System"
            Case 159435
                Return "Переместить в новую категорию"
            Case 159436
                Return "E-mail Sender for System Alert Notifications"
            Case 159437
                Return "File was uploaded successfully."
            Case 159444
                Return "Impose Picture Size"
            Case 159445
                Return "Time Zone"
            Case 159446
                Return "Image Processing"
            Case 159457
                Return "SQL Server Full text search has the ability to perform complex queries against character data. Full Text Search allows searching of similar texts. For example, searching ''tomato'' will also yield ''tomatoes.'' SQL 2009 provides the ranking of search results based on the matches in the name, note (or procedure), and ingredient of the query."
            Case 159458
                Return "Full population"
            Case 159459
                Return "Full text search"
            Case 159460
                Return "minute"
            Case 159461
                Return "Every"
            Case 159462
                Return "Run"
            Case 159463
                Return "Incremental Population"
            Case 159464
                Return "Language Word breaker"
            Case 159471
                Return "IP Address"
            Case 159472
                Return "Blocked IP list"
            Case 159473
                Return "Block IP when login attempts reach"
            Case 159474
                Return "Please enter at least ¶ characters"
            Case 159485
                Return "Предложить обменяться рецептами"
            Case 159486
                Return "Submitted to Recipe Exchange"
            Case 159487
                Return "You have approved this recipe. It can now be seen by all users."
            Case 159488
                Return "Unknown Language"
            Case 159607
                Return "Standalone Recipe Management Software"
            Case 159608
                Return "Recipe Management Software for Concurrent Users in a Network"
            Case 159609
                Return "Web Based Recipe Management Software"
            Case 159610
                Return "Inventory and Back Office Management Software"
            Case 159611
                Return "Recipe Viewer for Pocket PC"
            Case 159612
                Return "Order Taking and Nutrient Monitoring Software"
            Case 159613
                Return "E-Cookbook Software"
            Case 159699
                Return "Обновить существующие пункты"
            Case 159707
                Return "France"
            Case 159708
                Return "Germany"
            Case 159751
                Return "Site"
            Case 159778
                Return "Улучшенный"
            Case 159779
                Return "Основной"
            Case 159782
                Return "Подключить пункты продаж к продуктам"
            Case 159783
                Return "Подключить пункты продаж к рецептам/меню"
            Case 159795
                Return "Импорт Кассы – Конфигурация"
            Case 159918
                Return "У Вас нет прав доступа к этой функции. "
            Case 159924
                Return "Manage"
            Case 159925
                Return "Invalid Conversion"
            Case 159929
                Return "Page Options"
            Case 159934
                Return "Nutrient Information"
            Case 159940
                Return "Export Updates"
            Case 159941
                Return "Export All"
            Case 159942
                Return "Output Directory"
            Case 159943
                Return "Quality"
            Case 159944
                Return "Parent"
            Case 159946
                Return "CALCMENU Web"
            Case 159947
                Return "Select or upload file"
            Case 159949
                Return "Format should not exceed 10 characters."
            Case 159950
                Return "Nutrient name should not exceed 25 characters."
            Case 159951
                Return "Roles"
            Case 159962
                Return "Enter Tax Information"
            Case 159963
                Return "Enter Translation"
            Case 159966
                Return "Move marked items to new brand"
            Case 159967
                Return "Enter default site name:"
            Case 159968
                Return "Enter default Web site theme"
            Case 159969
                Return "Enable grouping sites by property to be managed by admin:" '"by property admin" to "by admin"
            Case 159970
                Return "Require users to submit information to the approver first before it can be used or published:"
            Case 159971
                Return "Enter the translation for each corresponding language or the default text will be used:"
            Case 159973
                Return "Select the sites that should belong to this property"
            Case 159974
                Return "Select available languages to use for translating ingredient, recipes, menus, and other information"
            Case 159975
                Return "Select one or more price groups to use for assigning prices to your ingredient, recipe, and menu"
            Case 159976
                Return "Check the items to include"
            Case 159977
                Return "List of owners"
            Case 159978
                Return "Choose a format below"
            Case 159979
                Return "Choose basic list to purge"
            Case 159981
                Return "The following are the shared sites for this item"
            Case 159982
                Return "Move marked to new source"
            Case 159987
                Return "Request Type"
            Case 159988
                Return "Requested by"
            Case 159990
                Return "Change brand"
            Case 159994
                Return "Replace ingredient in menus"
            Case 159997
                Return "Global Sharing"
            Case 160004
                Return "First Level"
            Case 160005
                Return "The selected ingredient should have the following units:"
            Case 160008
                Return "Step"
            Case 160009
                Return "More Actions"
            Case 160012
                Return "This recipe/menu is published on the web."
            Case 160013
                Return "This recipe/menu is not published on the web."
            Case 160014
                Return "Remember me"
            Case 160016
                Return "View Owners"
            Case 160018
                Return "This ingredient is published on the web."
            Case 160019
                Return "This ingredient is not published on the web."
            Case 160020
                Return "This ingredient is exposed."
            Case 160021
                Return "This ingredient is not exposed."
            Case 160023
                Return "For printing"
            Case 160028
                Return "Not to be published"
            Case 160030
                Return "Add to shopping list"
            Case 160033
                Return "Add keywords"
            Case 160035
                Return "You have attempted to login %n times"
            Case 160036
                Return "This account has been deactivated"
            Case 160037
                Return "Contact your system administrator to reactivate this account."
            Case 160038
                Return "My Profile"
            Case 160039
                Return "Last login"
            Case 160040
                Return "You are not signed in."
            Case 160041
                Return "Page Language"
            Case 160042
                Return "Main Translation"
            Case 160043
                Return "Main Set of Price"
            Case 160045
                Return "Rows Per Page"
            Case 160046
                Return "Default Display"
            Case 160047
                Return "Ingredient Quantities"
            Case 160048
                Return "Last accessed"
            Case 160049
                Return "Received '%f'"
            Case 160050
                Return "Length"
            Case 160051
                Return "Failed to receive '%f'"
            Case 160055
                Return "Quantity must be greater than 0."
            Case 160056
                Return "Create a new sub-recipe"
            Case 160057
                Return "Session has expired."
            Case 160058
                Return "Your login has expired due to inactivity for %n minutes."
            Case 160065
                Return "No name"
            Case 160066
                Return "Are you sure you want to close?"
            Case 160067
                Return "Your entry requires approval"
            Case 160068
                Return "Click the '%s' button to request approval."
            Case 160070
                Return "Marked items to be processed"
            Case 160071
                Return "This entry has been submitted for approval."
            Case 160072
                Return "There is already an existing request for this entry."
            Case 160074
                Return "Select unit"
            Case 160082
                Return "New requests await your approval."
            Case 160085
                Return "Your request has been reviewed."
            Case 160086
                Return "Print Nutrient List"
            Case 160087
                Return "Print List"
            Case 160088
                Return "Print Details"
            Case 160089
                Return "Activate"
            Case 160090
                Return "Create"
            Case 160091
                Return "Delete selected item from the list." '"Remove selected item from the list."
            Case 160093
                Return "Submit to System for global sharing"
            Case 160094
                Return "Make content available on kiosk browser"
            Case 160095
                Return "Create a System copy"
            Case 160096
                Return "Replace  ingredient used in recipes and menus"
            Case 160098
                Return "Do not publish on the web"
            Case 160100
                Return "Create list of ingredients to be purchased"
            Case 160101
                Return "You can use text as ingredients that don't need quantity and price definitions."
            Case 160102
                Return "Create your own recipe database, share it with other users, print it, and even create a shopping list for it."
            Case 160103
                Return "Menu is a list of ingredients or recipes available in a meal."
            Case 160105
                Return "Organize basic information such as those related to users, suppliers, etc."
            Case 160106
                Return "Welcome"
            Case 160107
                Return "Welcome to %s"
            Case 160108
                Return "Customize your view and other settings."
            Case 160109
                Return "Website Profile"
            Case 160110
                Return "Customize Web site's name, themes, etc."
            Case 160111
                Return "Approval Routing"
            Case 160112
                Return "Approval of ingredient, recipes, and other information."
            Case 160113
                Return "SMTP and Alert Notification Settings"
            Case 160114
                Return "Configure connection to your mail server; enable or disable alerts."
            Case 160115
                Return "Set maximum login attempts and monitor blocked IP addresses."
            Case 160116
                Return "Print Profile"
            Case 160117
                Return "Define multiple printing formats as profiles."
            Case 160118
                Return "Define list of languages for translating ingredient, recipes, and other information." ' "Define list of languages for translating ingredient, recipes, menus, and other information."
            Case 160119
                Return "Available currencies for currency conversion and set of price definition."
            Case 160120
                Return "Work with ingredient, recipes, and menus with multiple sets of prices."
            Case 160121
                Return "Properties are groups of sites."
            Case 160122
                Return "Sites organize users working together on a particular set of recipes."
            Case 160123
                Return "Manage users working on %s"
            Case 160124
                Return "Image Processing Preferences"
            Case 160125
                Return "Define standard picture size for ingredient, recipes, and menus."
            Case 160130
                Return "Trademarks or distinctive names identifying ingredient."
            Case 160132
                Return "Used to group ingredient, recipes, or menus by common attributes."
            Case 160135
                Return "Keywords provide descriptive details to ingredient, recipes, or menus. Users can assign multiple keywords per ingredient, recipe, or menu."
            Case 160139
                Return "Define up to 34 nutrients values for nutrients like Energy, Carbohydrates, Proteins, and Lipids."
            Case 160141
                Return "Create rules that can be used as an additional filter for searching."
            Case 160151
                Return "List of predefined (or system) units used in defining ingredient prices as well as in encoding recipes and menus."
            Case 160152
                Return "Users can add to this list."
            Case 160153
                Return "Used in price calculation"
            Case 160154
                Return "Source refers to the origin of a particular recipe. It can be a chef, book, magazine, food service company, organization, or Web site."
            Case 160155
                Return "Import ingredient, recipes, or menus from CALCMENU Pro, CALCMENU Enterprise, and other EGS products."
            Case 160156
                Return "Maintenance of exchange rate for different currencies"
            Case 160157
                Return "Delete unused texts." '"Remove unused texts."
            Case 160158
                Return "Format all texts."
            Case 160159
                Return "Print ingredient list in HTML, Excel, PDF, and RTF formats."
            Case 160160
                Return "Print ingredient details  in HTML, Excel, PDF, and RTF formats."
            Case 160161
                Return "Print recipe details  in HTML, Excel, PDF, and RTF formats."
            Case 160162
                Return "Print recipe list  in HTML, Excel, PDF, and RTF formats."
            Case 160163
                Return "Print menu details  in HTML, Excel, PDF, and RTF formats."
            Case 160164
                Return "Menu engineering allows you to evaluate current and future recipe pricing and design. Analyze menus and individual menu items to achieve optimum profit. Use Menu Engineering to identify which menu items to retain or drop from your menu."
            Case 160169
                Return "Load Menu Cards List"
            Case 160170
                Return "Modify or preview saved menu cards."
            Case 160175
                Return "Modify, preview or print saved shopping lists."
            Case 160177
                Return "Security"
            Case 160180
                Return "Standardize format of the items"
            Case 160181
                Return "Purge items"
            Case 160182
                Return "Role Rights"
            Case 160184
                Return "TCPOS Export"
            Case 160185
                Return "Export sales item"
            Case 160187
                Return "Create new local ingredient that can be used as ingredient for your recipes."
            Case 160188
                Return "Show list of saved marks"
            Case 160189
                Return "Show list of items to be purchased."
            Case 160190
                Return "Create your own menus based on the available recipes in your database."
            Case 160191
                Return "Create a text used for recipes and menus."
            Case 160200
                Return "Отсортировано по имени"
            Case 160202
                Return "Choose from the list"
            Case 160209
                Return "Пожалуйста введите серийный номер, название заголовка и ключ продукта. Вы сможете найти эту информацию в документации, предоставленной с %s."
            Case 160210
                Return "Wanted Items"
            Case 160211
                Return "Unwanted Items"
            Case 160212
                Return "Drafts"
            Case 160217
                Return "Archive Path"
            Case 160218
                Return "Import Ingredient Data with Errors"
            Case 160219
                Return "Pending List of ingredient that needs to be fixed"
            Case 160220
                Return "Define options for Ingredient import"
            Case 160254
                Return "Please restart the windows service %n for your changes to take effect."
            Case 160258
                Return "Currency does not match the chosen set of price."
            Case 160259
                Return "Name or number already exists."
            Case 160260
                Return "Date Imported"
            Case 160262
                Return "Питательная ценность на 1 единицу прибыли"
            Case 160292
                Return "Allergens"
            Case 160293
                Return "List of food allergies or sensitivities associated to ingredient."
            Case 160295
                Return "This account is currently in use. Please try again later."
            Case 160353
                Return "Purchasing Set of Price"
            Case 160354
                Return "Selling Set of Price"
            Case 160423
                Return "Standalone Recipe/Menu Management Software"
            Case 160433
                Return "Потребление в"
            Case 160500
                Return "Text Management"
            Case 160687
                Return "Alternating Item Color"
            Case 160688
                Return "Normal Item Color"
            Case 160690
                Return "Please note that when you restore, it will automatically cut-off users currently using the System."
            Case 160691
                Return "Backup/Restore Pictures"
            Case 160716
                Return "Set items to Global by default"
            Case 160774
                Return "Deactivate"
            Case 160775
                Return "Delete trailing zeroes" ' "Remove trailing zeroes"
            Case 160777
                Return "Click here to learn more about CALCMENU."
            Case 160788
                Return "Selected item(s) has been activated."
            Case 160789
                Return "Selected item(s) has been deactivated."
            Case 160790
                Return "Are you sure you want to delete selected item(s)?" '  "Are you sure you want to remove selected item(s)?"
            Case 160791
                Return "Selected item(s) has been successfully deleted." ' "Selected item(s) has been successfully removed."
            Case 160801
                Return "You can only merge two or more similar recipes."
            Case 160802
                Return "Are you sure you want to merge selected items?"
            Case 160803
                Return "Are you sure you want to purge items?"
            Case 160804
                Return "Please fill out the required fields."
            Case 160805
                Return "Select two or more items to merge."
            Case 160806
                Return "Are you sure you want to deactivate selected item(s)?"
            Case 160863
                Return "Ingredient Price List"
            Case 160940
                Return "Effectivity Date"
            Case 160941
                Return "Linked Sales Item"
            Case 160953
                Return "Factor of Selling Set of Price to Purchasing Set of Price"
            Case 160958
                Return "Work with sales item with multiple selling sets of prices."
            Case 160985
                Return "Not Linked Sales Item"
            Case 160987
                Return "Create sales items and link it to existing recipes."
            Case 160988
                Return "Sales item is used in selling and it is usually linked to a recipe."
            Case 161028
                Return "Are you sure you want to change the nutrient database? This action will change the nutrient definitions you have already set in your ingredient."
            Case 161029
                Return "Either the Yields or Ingredients check box must be selected."
            Case 161049
                Return "Force deletion of keyword and its sub-keywords"
            Case 161050
                Return "Deleted keywords will also be unassigned from ingredient/recipe/menu items." '  "Removed keywords will also be unassigned from ingredient/recipe/menu items."
            Case 161051
                Return "Selected keywords and all its sub-keywords are successfully deleted. Deleted keywords are now also unassigned from ingredient, recipe, and menu items."
            Case 161078
                Return "Exact"
            Case 161079
                Return "Starts with"
            Case 161080
                Return "Contains"
            Case 161082
                Return "Second"
            Case 161083
                Return "Third"
            Case 161084
                Return "Fourth"
            Case 161085
                Return "One time only"
            Case 161086
                Return "Daily"
            Case 161087
                Return "Weekly"
            Case 161088
                Return "Monthly"
            Case 161089
                Return "When file changes"
            Case 161090
                Return "When the computer starts"
            Case 161091
                Return "Enter %s information"
            Case 161092
                Return "Supplier Group"
            Case 161093
                Return "Billing Information"
            Case 161094
                Return "Start Date"
            Case 161095
                Return "of the month"
            Case 161096
                Return "POS Import - Failed Data"
            Case 161097
                Return "Organize and maintain information of your suppliers including company contacts, addresses, terms of payment, etc. to ease up the ordering process."
            Case 161098
                Return "Terminal refers to the stations of your POS that are linked to your CALCMENU Web. Add, modify, or delete terminals in this program."
            Case 161099
                Return "Configure the POS import parameters. Set the schedule, location of import files, etc."
            Case 161100
                Return "Products and stock items are kept and circulated at different locations during different times. Maintain control in establishing the possible locations where your products can be found at any given moment."
            Case 161101
                Return "Clients are companies that purchase your products or finished goods. Manage your client list in this program."
            Case 161102
                Return "Client contacts are the persons you are dealing with in a company. Create, modify, and delete client contacts."
            Case 161103
                Return "Fix POS data which are not successfully imported in the system."
            Case 161104
                Return "This refers to the type of issuance transaction from supplies. This may or may not have been actually sold to customers such as employee benefits or giveaways."
            Case 161105
                Return "Sales History quickly shows a lsit of sales transaction and sales item involved"
            Case 161106
                Return "Marked Items"
            Case 161107
                Return "Computed Yield"
            Case 161132
                Return "View My Recipes"
            Case 159274
                Return "%number only"
            Case 161147
                Return "Recipe and Menu Management (except Menu Planning)"
            Case 161162
                Return "TCPOS"
            Case 155761
                Return "Импортировать товар"
            Case 161180
                Return "Define automatic upload configuration"
            Case 161181
                Return "Host name"
            Case 11060
                Return "Каталог"
            Case 24068
                Return "Поле"
            Case 158734
                Return "Версия базы данных несовместима с этой версией программы. "
            Case 161275
                Return "Guideline Daily Amounts"
            Case 161276
                Return "GDA"
            Case 7250
                Return "Французский"
            Case 7280
                Return "Итальянский"
            Case 7260
                Return "Немецкий"
            Case 157515
                Return "Голландский"
            Case 158868
                Return "Китайский"
            Case 161279
                Return "Without"
            Case 54295
                Return "with"
            Case 159468
                Return "Использовать как ингредиент"
            Case 159469
                Return "Не использовать как ингредиент"
            Case 134159
                Return "Все"
            Case 144582
                Return "Нет группы"
            Case 161281
                Return "Power Cook"
            Case 161282
                Return "Propery Admin"
            Case 161283
                Return "System Admin"
            Case 161284
                Return "Corporate Chef"
            Case 161285
                Return "Propery Chef"
            Case 161286
                Return "Cook"
            Case 161287
                Return "Guest"
            Case 161288
                Return "Site Chef"
            Case 161289
                Return "Site Admin"
            Case 161290
                Return "View and Print"
            Case 161291
                Return "?? ??????????"
            Case 161292
                Return "Defined"
            Case 161294
                Return "Unwanted %s"
            Case 24269
                Return "Select all"
            Case 24268
                Return "Deselect all"
            Case 160880
                Return "Recalculate"
            Case 160894
                Return "Silver"
            Case 14110
                Return "Footer"
            Case 161300
                Return "Main Purchasing Set of Price"
            Case 160776
                Return "Go back to %s"
            Case 132617
                Return "ВСЕ КАТЕГОРИИ"
            Case 155842
                Return "Персоны"
            Case 155050
                Return "ВСЕ КЛЮЧЕВЫЕ СЛОВА"
            Case 135024
                Return "Местоположение"
            Case 161333
                Return "Labels"
            Case 161334
                Return "Recipes %x-%y of %z"
            Case 104836
                Return "Модифицировать продукт"
            Case 51281
                Return "Ингредиенты для"
            Case 158349
                Return "Назначенное ключевое слово"
            Case 158350
                Return "Производное ключевое слово"
            Case 119130
                Return "Поиск"
            Case 155927
                Return "ВСЕ ИСТОЧНИКИ"
            Case 161484
                Return "Temperature"
            Case 161485
                Return "Production<br />Date"
            Case 161486
                Return "Consumption<br />Date"
            Case 31700
                Return "Days"
            Case 7030
                Return "Принтер"
            Case 161487
                Return "Daily Product"
            Case 161488
                Return "Consume before"
            Case 161489
                Return "Fresh enjoy freshly-prepared"
            Case 161490
                Return "Info Allergies; contains:"
            Case 161491
                Return "Assigned to all marked"
            Case 4825
                Return "Рецепты"
            Case 21550
                Return "No dishes found"
            Case 24011
                Return "От"
            Case 161494
                Return "at max. 5°C"
            Case 161538
                Return "Thank you for your interest in EGS Products."
            Case 161554
                Return "You can also find additional information about our products as documents in PDF formats at the <a href=''%url''>Product Resources page</a>. "
            Case 161576
                Return "???? ??????? ?????????"
            Case 133328
                Return "Название рецепта"
            Case 51128
                Return "Название рецепта"
            Case 161577
                Return "Time"
            Case 161578
                Return "Total Ingredient Cost"
            Case 161579
                Return "calculate"
            Case 161580
                Return "Ingredient Cost"
            Case 161581
                Return "Tax"
            Case 161582
                Return "Grossmargin in Fr."
            Case 161583
                Return "Gross margin in %"
            Case 159733
                Return "Номер статьи."
            Case 161584
                Return "Unit."
            Case 143003
                Return "Чистое¶количество"
            Case 155811
                Return "Валовое¶количество"
            Case 161585
                Return "Price/¶Unit"
            Case 132708
                Return "Нет поставщика"
            Case 24075
                Return "Article number"
            Case 27056
                Return "and"
            Case 161766
                Return "Small portion"
            Case 161767
                Return "Large portion"
            Case 156892
                Return "Загрузить:"
            Case 161777
                Return "Unassign keyword"
            Case 161778
                Return "Assign/unassign keywords"
            Case 161779
                Return "Breadcrumbs"
            Case 161780
                Return "Monitor Breadcrumbs"
            Case 161781
                Return "Unwanted Keyword"
            Case 161782
                Return "Print Labels"
            Case 161783
                Return "Procedure Template"
            Case 161784
                Return "Student"
            Case 161785
                Return "Ingredient nutrient values per %s"
            Case 161786
                Return "Ingredient nutrient values per 100g/ml"
            Case 155926
                Return "Экспорт в Excel"
            Case 161787
                Return "Apply Template"
            Case 135969
                Return "Вы уверены, что хотите заменить %o?"
            Case 132934
                Return "Последний рецепт"
            Case 132937
                Return "Последнее меню"
            Case 161788
                Return "Assigned/Derived Keywords"
            Case 161468
                Return "Validate all"
            Case 161823
                Return "Add Row(s)"
            Case 161824
                Return "Paste from Clipboard"
            Case 161825
                Return "There is no ingredient that needs to be linked."
            Case 161826
                Return "Choose Another"
            Case 8514
                Return "New price"
            Case 161827
                Return "Default Price/Unit:"
            Case 161828
                Return "Choose from existing units"
            Case 161829
                Return "Add this as a new unit"
            Case 161831
                Return "Let me edit ingredient before adding"
            Case 161832
                Return "place %s in complement"
            Case 161834
                Return "Please check the prices"
            Case 161835
                Return "Cut"
            Case 159594
                Return "&Add to recipe"
            Case 161837
                Return "Add to recipe"
            Case 10447
                Return "Order"
            Case 161838
                Return "Replace existing ingredients"
            Case 161839
                Return "No ingredients found"
            Case 132672
                Return "Вы уверены, что хотите удалить  %n?"
            Case 161840
                Return ""
            Case 161841
                Return "Link to ingredient or sub-recipe"
            Case 161842
                Return "All items are now linked to ingredient/sub-recipe"
            Case 161843
                Return "Item is now linked to ingredient/sub-recipe"
            Case 161844
                Return "Storing Time"
            Case 161845
                Return "Storing Temperature"
            Case 161851
                Return "Can be ordered"
            Case 161852
                Return "Recipe may contain allergens"
            Case 159088
                Return "Отправить запрос на утверждение"
            Case 161855
                Return "Draft"
            Case 161986
                Return "Add Step"
            Case 161853
                Return "Paste"
            Case 161987
                Return "Item %n of %p"
            Case 161988
                Return "Linked Products"
            Case 161989
                Return "Not Linked Products"
            Case 158851
                Return "Создано "
            Case 161830
                Return "Item validated"
            Case 162198
                Return "The yield has been changed. Click the Calculate button to resize ingredient quantities."
            Case 162199
                Return "The yield has been changed. Do you want to continue saving without calculating ingredient quantities?"
            Case 162203
                Return "Information"
            Case 162205
                Return "Number of bids"
            Case 162208
                Return "Weekly Business Days"
            Case 151500
                Return "Предложение"
            Case 162211
                Return "Select Language"
            Case 162212
                Return "Business Name"
            Case 162213
                Return "Business Number"
            Case 162214
                Return "Price available"
            Case 162215
                Return "Logo to the server load"
            Case 146043
                Return "Январь"
            Case 146044
                Return "Февраль"
            Case 146045
                Return "Март"
            Case 146046
                Return "Апрель"
            Case 146047
                Return "Май"
            Case 146048
                Return "Июнь"
            Case 146049
                Return "Июль"
            Case 146050
                Return "Август"
            Case 146051
                Return "Сентябрь"
            Case 146052
                Return "Октябрь"
            Case 146053
                Return "Ноябрь"
            Case 146054
                Return "Декабрь"
            Case 162216
                Return "Preferences"
            Case 162219
                Return "Back Office"
            Case 162221
                Return "General Configuration"
            Case 162222
                Return "Insert Here"
            Case 8990
                Return "Или"
            Case 162230
                Return "Enter style information"
            Case 162231
                Return "Name of style"
            Case 162232
                Return "Header style options"
            Case 160237
                Return "Полужирный"
            Case 134826
                Return "Закрытый"
            Case 162235
                Return "Did you mean"
            Case 159700
                Return "Импортировать рецепт"
            Case 162276
                Return "????????????? ??????"
            Case 162282
                Return "Notes"
            Case 159681
                Return "Recipe (%s) has too many ingredients. (Max. is %n)"
            Case 135257
                Return "Валовая маржа"
            Case 31732
                Return "Меню"
            Case 162340
                Return "Street"
            Case 162341
                Return "Place"
            Case 162357
                Return "Example"
            Case 162358
                Return "Keep Length of Prefix"
            Case 162359
                Return ""
            Case 162361
                Return "Tab"
            Case 162362
                Return "Pipe"
            Case 162363
                Return "Semi-colon"
            Case 162364
                Return "Space"
            Case 133590
                Return "&Paste"
            Case 155260
                Return "Установленный коэффициент"
            Case 156060
                Return "Установленные затраты на продукты питания"
            Case 156061
                Return "Установленная прибыль"
            Case 162383
                Return "?????????????"
            Case 162382
                Return "????????"
            Case 162386
                Return "Go"
            Case 162387
                Return "Hi Approver,You have received a recipe for approval. [Name of the creator of the item] has submitted this recipe: [...]Please login to the CALCMENU Web site to review and approve the recipe.Regards,EGS Team"
            Case 162388
                Return "Hi,Your newly created recipe has been sent for approval. The recipe will be reviewed and approved first before it can be used online. You have submitted this recipe: [...]Once approved, the recipe will be available online.Regards,EGS Team"
            Case 162389
                Return "Hi Approver,You have approved this recipe: [...]The recipe will be available online.Regards,EGS Team"
            Case 162390
                Return "Hi,The recipe [...] has been approved. You can now use this recipe online.Regards,EGS Team"
            Case 162530
                Return "Delete breadcrumbs upon login" '  "Remove breadcrumbs upon login"
            Case 28483
                Return "The record does not exist"
            Case 162955
                Return "Net margin in %"
            Case 132900
                Return "Добавить цену"
            Case 163032
                Return "Copy Price List"
            Case 155995
                Return "Проверка..."
            Case 156784
                Return "Общее число ошибок: %n"
            Case 51174
                Return "Импорт завершен!"
            Case 133334
                Return "Импорт %r"
            Case 163046
                Return "Sorry, Keyword %k%n%u not found. Please press 'Browse Keyword' to select available Keywords."
            Case 135283
                Return "Последняя цена"
            Case 156542
                Return "Средневзвешенная цена"
            Case 147381
                Return "Инвентаризационные цены ранее используемые для продуктов"
            Case 157281
                Return "Цены поставщика  по умолчанию"
            Case 163057
                Return "Cost for total %s"
            Case 163058
                Return "Cost for 1 %s"
            Case 132553
                Return "Установленная отпускная цена + Налог"
            Case 138031
                Return "Все продукты для инвентаризации"
            Case 138032
                Return "Продукты из маркированных категорий"
            Case 138033
                Return "Продукты из маркированных складов"
            Case 138034
                Return "Продукты от маркированных поставщиков"
            Case 138035
                Return "Продукты из одной или более предыдущих инвентаризаций"
            Case 138030
                Return "Выберите продукты, которые Вы хотите включить в эту инвентаризацию."
            Case 163060
                Return "Food Cost in %s"
            Case 163061
                Return "Imposed Food Cost in %s"
            Case 167719
                Return "Budget"
            Case 158410
                Return "Если некоторые продукты не имеют установленной цены (цена = 0), использовать цену поставщика по умолчанию."
            Case 136230
                Return "Создать новую инвентаризацию"
            Case 136231
                Return "Модифицировать информацию об инвентаризации"
            Case 3205
                Return "Имя"
            Case 135235
                Return "Стоимость запасов"
            Case 135100
                Return "Номер поручителя"
            Case 135110
                Return "Количество,¶согласно инвентаризации"
            Case 160414
                Return "Количество согласно предыдущей инвентаризации"
            Case 136100
                Return "Открытые в данный момент инвентаризации"
            Case 136115
                Return "# пунктов"
            Case 136110
                Return "Открыта в"
            Case 1146
                Return "Выполняется"
            Case 134021
                Return "Инвентаризация начата в"
            Case 124164
                Return "Корректировать инвентаризацию"
            Case 158946
                Return "Назначить имеющееся в наличии количество как инвентаризированное количество"
            Case 136213
                Return "Добавить продукт в текущую инвентаризацию"
            Case 136214
                Return "Удалить продукт из инвентаризации"
            Case 136212
                Return "Показать список необходимых корректировок"
            Case 136215
                Return "Добавить новое место хранения для продукта"
            Case 136217
                Return "Удалить количество для выбранного Продукта - Места хранения товара"
            Case 155861
                Return "Обнулить количество для выбранных пунктов"
            Case 136216
                Return "Удалить выбранное место хранения для этого товара"
            Case 157336
                Return "Непригодный"
            Case 136030
                Return "Содержимое"
            Case 133147
                Return "Литры"
            Case 136432
                Return "Неправильный код"
            Case 143981
                Return "Неправильный код счета"
            Case 169310
                Return "Degustation/Development"
            Case 169318
                Return "Feedback"
            Case 110447
                Return "Order"
            Case 158216
                Return "Centralizing Recipe Management Anytime, Anywhere"
            Case 168373
                Return "Используемые онлайн"
            Case 168374
                Return "Reference No1"
            Case 168375
                Return "Reference No2"
            Case 157060
                Return "Исходный номер"
            Case 157659
                Return "Заблокировать"
            Case 157660
                Return "Разблокировать"
            Case 170155
                Return "Assign ingredient, recipes and menus to Categories, Keywords and Sources (could be a cookbook, Website, chef, etc.). This allows you to group and organize items in EGS CALCMENU Web. Searching for ingredient, recipes or menus can be made faster and easier since Categories, Keywords, and Sources are very useful in narrowing down search results."
            Case 160232
                Return "Экспорт в"
            Case 170770
                Return "Yield to Print"
            Case 133248
                Return "Ингредиент"
            Case 170779
                Return "Ingredient List"
            Case 170780
                Return "Ingredient Details"
            Case 170781
                Return "Ingredient Nutrient List"
            Case 170782
                Return " Ingredient Category"
            Case 170783
                Return "Ingredient Keyword"
            Case 170784
                Return "Ingredient Published On The Web"
            Case 170785
                Return "Ingredient Not Published On The Web"
            Case 170786
                Return "Ingredient Cost"
            Case 170849
                Return "Abbreviated Preparation Method"
            Case 171301
                Return "Preparation Method"
            Case 171302
                Return "Tips"
            Case 170850
                Return "Cook Mode only"
            Case 133115
                Return "Все рецепты"
            Case 170851
                Return "None Cook Mode only"
            Case 170852
                Return "Show Off"
            Case 170853
                Return "Quick & Easy"
            Case 170854
                Return "Chef Recommended"
            Case 170855
                Return "Moderate"
            Case 170856
                Return "Challenging"
            Case 170857
                Return "Gold"
            Case 170858
                Return "Unrated"
            Case 170859
                Return "Bronze"
            Case 170860
                Return "Move marked to new standard"
            Case 171219
                Return "LeadIn"
            Case 55011
                Return "Serving Size"
            Case 171220
                Return "Servings per Yield" ' "Number of Servings"
            Case 171221
                Return "Total Yield/Servings" ' "Total Yield"
            Case 151436
                Return "Attachment"
            Case 150009
                Return "Exportation Done. BrandSite Successfully Exported."
            Case 171597
                Return "Recipe has been checked in by another user and cannot be modified."
            Case 27220
                Return "Hour"

            Case 171650
                Return "Prep Time"
            Case 171651
                Return "Cook Time "
            Case 171652
                Return "Marinate Time "
            Case 171653
                Return "Stand Time "
            Case 171654
                Return "Chill Time "
            Case 171655
                Return "Brew Time "
            Case 171656
                Return "Freeze Time "
            Case 171657
                Return "ReadyIn"
            Case 171658
                Return "second"
            Case 171616
                Return "Placement"

        End Select
    End Function
 
 
'chinese
    Public Function FTBLow43USA(ByVal Code As Integer) As String
        Select Case Code
            Case 1080
                Return "商品成本"
            Case 1081
                Return "制品成本"
            Case 1090
                Return "售价"
            Case 1145
                Return "柜台"
            Case 1260
                Return "商品"
            Case 1280
                Return "备注"
            Case 1290
                Return "价格"
            Case 1300
                Return "损耗"
            Case 1310
                Return "数量"
            Case 1400
                Return "菜单"
            Case 1450
                Return "种类"
            Case 1480
                Return "统制价格"
            Case 1485
                Return "推荐价格"
            Case 1500
                Return "日期"
            Case 1530
                Return "单位丢失"
            Case 1600
                Return "修改菜单"
            Case 2430
                Return "从名单中选择"
            Case 2700
                Return "打印菜单列表"
            Case 2780
                Return "菜单的工程化管理"
            Case 3057
                Return "数据库"
            Case 3140
                Return "为了"
            Case 3150
                Return "百分比"
            Case 3161
                Return "因数"
            Case 3195
                Return "食谱#"
            Case 3200
                Return "厨师"
            Case 3204
                Return "姓"
            Case 3206
                Return "翻译"
            Case 3215
                Return "单价"
            Case 3230
                Return "图片"
            Case 3234
                Return "名单"
            Case 3300
                Return "菜单卡片"
            Case 3305
                Return "参考名称"
            Case 3306
                Return "代表"
            Case 3320
                Return "您是否要把数量调整到新的份数（一份食物[饮料]）？"
            Case 3460
                Return "&密码"
            Case 3680
                Return "备用"
            Case 3685
                Return "备用完整"
            Case 3721
                Return "来源"
            Case 3760
                Return "导入"
            Case 3800
                Return "导出"
            Case 4130
                Return "盘上的自由空间"
            Case 4185
                Return "产品编号"
            Case 4755
                Return "开始输入"
            Case 4832
                Return "食谱"
            Case 4834
                Return "菜谱成分"
            Case 4854
                Return "最小化"
            Case 4855
                Return "最大化"
            Case 4856
                Return "从"
            Case 4860
                Return "文件名"
            Case 4862
                Return "版本"
            Case 4865
                Return "用户"
            Case 4867
                Return "修改"
            Case 4870
                Return "修改用户"
            Case 4877
                Return "平均"
            Case 4890
                Return "文件类型"
            Case 4891
                Return "预览"
            Case 5100
                Return "单位"
            Case 5105
                Return "格式"
            Case 5270
                Return "商品清单"
            Case 5350
                Return "合计"
            Case 5390
                Return "服务"
            Case 5500
                Return "号"
            Case 5530
                Return "统制售价"
            Case 5590
                Return "配料"
            Case 5600
                Return "准备"
            Case 5610
                Return "页"
            Case 5720
                Return "数额"
            Case 5741
                Return "总"
            Case 5795
                Return "每份食物[饮料]"
            Case 5801
                Return "利润"
            Case 5900
                Return "商品类别"
            Case 6000
                Return "修改类别"
            Case 6002
                Return "种类的名字"
            Case 6055
                Return "增加文本"
            Case 6390
                Return "货币"
            Case 6416
                Return "因素"
            Case 6470
                Return "请等待"
            Case 7010
                Return "否"
            Case 7073
                Return "浏览"
            Case 7181
                Return "所有"
            Case 7183
                Return "明显"
            Case 7270
                Return "英语"
            Case 7296
                Return "欧洲"
            Case 7335
                Return "成功地去除了所有标记"
            Case 7570
                Return "星期天"
            Case 7571
                Return "星期一"
            Case 7572
                Return "星期二"
            Case 7573
                Return "星期三"
            Case 7574
                Return "星期四"
            Case 7575
                Return "星期五"
            Case 7576
                Return "星期六"
            Case 7720
                Return "包装"
            Case 7725
                Return "运输"
            Case 7755
                Return "系统"
            Case 8210
                Return "演算"
            Case 8220
                Return "程序"
            Case 8395
                Return "增加"
            Case 8397
                Return "删除"
            Case 8913
                Return "无"
            Case 8914
                Return "小数"
            Case 8994
                Return "工具"
            Case 9030
                Return "更新"
            Case 9070
                Return "没允许在演示版本"
            Case 9140
                Return "瑞士"
            Case 9920
                Return "描述"
            Case 10103
                Return "拷贝"
            Case 10104
                Return "文本"
            Case 10109
                Return "选项"
            Case 10116
                Return "备注"
            Case 10121
                Return "查寻"
            Case 10125
                Return "笔记"
            Case 10129
                Return "选择"
            Case 10130
                Return "在手边"
            Case 10131
                Return "输入"
            Case 10132
                Return "输出"
            Case 10135
                Return "样式"
            Case 10140
                Return "库存"
            Case 10363
                Return "税"
            Case 10369
                Return "供应商数字"
            Case 10370
                Return "按顺序"
            Case 10399
                Return "删除"
            Case 10417
                Return "已失效："
            Case 10430
                Return "地点"
            Case 10431
                Return "存货"
            Case 10468
                Return "状态"
            Case 10513
                Return "折扣"
            Case 10523
                Return "电话"
            Case 10524
                Return "电传"
            Case 10554
                Return "CCP描述"
            Case 10555
                Return "冷却时间"
            Case 10556
                Return "加热时间"
            Case 10557
                Return "加热程度或温度"
            Case 10558
                Return "加热方式"
            Case 10572
                Return "营养素"
            Case 10573
                Return "信息1"
            Case 10970
                Return "印刷品"
            Case 10990
                Return "供应商"
            Case 11040
                Return "完成的恢复"
            Case 11280
                Return "注册"
            Case 12515
                Return "条形码"
            Case 12525
                Return "无效日期"
            Case 13060
                Return "营养素"
            Case 13255
                Return "历史"
            Case 14070
                Return "字体"
            Case 14090
                Return "标题"
            Case 14816
                Return "替换用"
            Case 14819
                Return "替换"
            Case 14884
                Return "更新项目"
            Case 15360
                Return "明显菜单"
            Case 15504
                Return "管理员"
            Case 15510
                Return "密码"
            Case 15615
                Return "输入您的密码"
            Case 15620
                Return "确认"
            Case 16010
                Return "演算"
            Case 18460
                Return "保存进展中"
            Case 20122
                Return "公司"
            Case 20200
                Return "次级食谱"
            Case 20469
                Return "指定邮寄的方法"
            Case 20530
                Return "能量"
            Case 20703
                Return "主要"
            Case 20709
                Return "单位"
            Case 21570
                Return "打印一个电传形式"
            Case 21600
                Return "关于"
            Case 24002
                Return "最终命令"
            Case 24016
                Return "供应商"
            Case 24027
                Return "计算"
            Case 24028
                Return "取消"
            Case 24044
                Return "双方"
            Case 24050
                Return "新"
            Case 24085
                Return "分配新"
            Case 24105
                Return "显示"
            Case 24121
                Return "简称"
            Case 24129
                Return "调动"
            Case 24150
                Return "编辑"
            Case 24152
                Return "职务"
            Case 24153
                Return "城市"
            Case 24163
                Return "默认位置"
            Case 24260
                Return "这个供应商不可能被删除"
            Case 24270
                Return "返回"
            Case 24271
                Return "下一个"
            Case 24291
                Return "小计"
            Case 26000
                Return "继续"
            Case 26100
                Return "产品说明"
            Case 26101
                Return "烹调技巧或建议"
            Case 26102
                Return "提炼"
            Case 26103
                Return "存贮"
            Case 26104
                Return "生产量或生产力"
            Case 27000
                Return "参考名称"
            Case 27020
                Return "地址"
            Case 27050
                Return "电话号码"
            Case 27055
                Return "标题名"
            Case 27130
                Return "付款"
            Case 27135
                Return "到期日"
            Case 28000
                Return "错误运转中"
            Case 28008
                Return "无效目录"
            Case 28655
                Return "单位未被定义"
            Case 29170
                Return "不可利用"
            Case 29771
                Return "修改商品"
            Case 30210
                Return "操作失败"
            Case 30270
                Return "没发现"
            Case 31085
                Return "成功地更新"
            Case 31098
                Return "保存"
            Case 31370
                Return "食物成本"
            Case 31375
                Return "食物成本"
            Case 31380
                Return "主要"
            Case 31462
                Return "错误"
            Case 31492
                Return "我们的电传协助服务根据遇到的问题在一个到24个小时之内保证您一个回复， (除了周末)"
            Case 31755
                Return "结果"
            Case 31758
                Return "至"
            Case 31769
                Return "卖"
            Case 31800
                Return "天"
            Case 31860
                Return "期间"
            Case 51056
                Return "产品"
            Case 51086
                Return "语言"
            Case 51092
                Return "单位"
            Case 51097
                Return "EGS Enggist && Grandjean SA软件"
            Case 51098
                Return "路线de Soleure 12/PO箱子"
            Case 51099
                Return "2072年St布勒斯，瑞士"
            Case 51123
                Return "细节"
            Case 51129
                Return "被要的成份"
            Case 51130
                Return "不需要的成份"
            Case 51139
                Return "要"
            Case 51157
                Return "消息"
            Case 51178
                Return "请再试试。"
            Case 51198
                Return "连接到SMTP服务器"
            Case 51204
                Return "是"
            Case 51243
                Return "边际"
            Case 51244
                Return "上面"
            Case 51245
                Return "底部"
            Case 51246
                Return "左"
            Case 51247
                Return "右面"
            Case 51252
                Return "下载"
            Case 51257
                Return "电子邮件"
            Case 51259
                Return "SMTP服务器"
            Case 51261
                Return "用户名"
            Case 51294
                Return "产量"
            Case 51311
                Return "无效单位"
            Case 51336
                Return "不需要"
            Case 51353
                Return "版权协议"
            Case 51364
                Return "您是否接受版权协议上面并且想继续进行食谱的提议？"
            Case 51377
                Return "发送电子邮件"
            Case 51392
                Return "产生单位"
            Case 51402
                Return "是否确实要删除"
            Case 51500
                Return "购物单细节"
            Case 51502
                Return "购物单"
            Case 51532
                Return "打印购物单"
            Case 51907
                Return "&显示细节"
            Case 52012
                Return "浏览"
            Case 52110
                Return "将进口选择的文件"
            Case 52130
                Return "新的食谱"
            Case 52150
                Return "已完成"
            Case 52307
                Return "关闭"
            Case 52960
                Return "Simple"
            Case 52970
                Return "完全"
            Case 53250
                Return "出口选择"
            Case 54210
                Return "不要改变什么"
            Case 54220
                Return "所有大写"
            Case 54230
                Return "所有小写"
            Case 54240
                Return "大写每个词的第一个字母 "
            Case 54245
                Return "第一个字母大写了"
            Case 54710
                Return "选择的关键字"
            Case 54730
                Return "关键字"
            Case 55211
                Return "链接"
            Case 55220
                Return "数量"
            Case 56100
                Return "您的名字"
            Case 56130
                Return "国家"
            Case 56500
                Return "字典"
            Case 101600
                Return "修改菜单"
            Case 103150
                Return "百分比"
            Case 103215
                Return "单价"
            Case 103305
                Return "参考名称"
            Case 103306
                Return "代表"
            Case 104829
                Return "供应商名单"
            Case 104835
                Return "创造一个新产品"
            Case 104854
                Return "最小"
            Case 104855
                Return "最大"
            Case 104862
                Return "版本"
            Case 104869
                Return "新用户"
            Case 104870
                Return "修改用户"
            Case 105100
                Return "单位"
            Case 105110
                Return "日期"
            Case 105200
                Return "为了"
            Case 105360
                Return "按份食物[饮料]的售价"
            Case 106002
                Return "类别名称"
            Case 107183
                Return "标明的"
            Case 110101
                Return "修改"
            Case 110102
                Return "删除"
            Case 110112
                Return "打印"
            Case 110114
                Return "帮助"
            Case 110129
                Return "选择"
            Case 110417
                Return "失败："
            Case 110524
                Return "电传"
            Case 113275
                Return "税"
            Case 115610
                Return "新密码被接受了"
            Case 121600
                Return "的"
            Case 124016
                Return "供应商"
            Case 124024
                Return "批准人"
            Case 124042
                Return "类型"
            Case 124257
                Return "销路"
            Case 127010
                Return "公司"
            Case 127040
                Return "国家"
            Case 127050
                Return "电话号码"
            Case 127055
                Return "标题名"
            Case 128000
                Return "运转中的错误"
            Case 131462
                Return "错误"
            Case 131757
                Return "从"
            Case 132552
                Return "总税"
            Case 132554
                Return "修改食谱"
            Case 132555
                Return "添加食谱"
            Case 132557
                Return "创建一份新的菜单"
            Case 132559
                Return "创建一件新的商品"
            Case 132561
                Return "请输入序号、标题名和产品密匙。您可以在RecipeNet食谱网提供的文献中找到这个信息。"
            Case 132565
                Return "补充"
            Case 132567
                Return "商品类别"
            Case 132568
                Return "食谱类别"
            Case 132569
                Return "菜单类别"
            Case 132570
                Return "无法删除。"
            Case 132571
                Return "类别在使用中。"
            Case 132589
                Return "食谱的最大数量"
            Case 132590
                Return "食谱的当前数量"
            Case 132592
                Return "商品的最大数量"
            Case 132593
                Return "商品的当前数量"
            Case 132597
                Return "创建一份新食谱"
            Case 132598
                Return "菜单的最大数量"
            Case 132599
                Return "菜单的当前数量"
            Case 132600
                Return "分配关键字"
            Case 132601
                Return "把有标记的移动到新的类别"
            Case 132602
                Return "删除有标记的"
            Case 132605
                Return "购物单"
            Case 132607
                Return "标记作用"
            Case 132614
                Return "净数量"
            Case 132615
                Return "权利"
            Case 132616
                Return "所有者"
            Case 132621
                Return "修改来源"
            Case 132630
                Return "自动转换"
            Case 132638
                Return "用户信息"
            Case 132640
                Return "已经使用的用户名。"
            Case 132654
                Return "数据库管理"
            Case 132657
                Return "&回复"
            Case 132667
                Return "合并"
            Case 132668
                Return "清除"
            Case 132669
                Return "移上"
            Case 132670
                Return "移下"
            Case 132671
                Return "规范化"
            Case 132678
                Return "HACCP"
            Case 132683
                Return "以前的"
            Case 132706
                Return "营养价值是每100g或100ml"
            Case 132714
                Return "请从该名单中选择。"
            Case 132719
                Return "为相同单位的价格已经被定义。"
            Case 132723
                Return "损耗总量不可能是大于或等于100%。"
            Case 132736
                Return "总量"
            Case 132737
                Return "添加新供应商"
            Case 132738
                Return "修改供应商"
            Case 132739
                Return "供应商细节"
            Case 132740
                Return "省"
            Case 132741
                Return "网址"
            Case 132779
                Return "使用的关键字。"
            Case 132783
                Return "关键字"
            Case 132788
                Return "营养连接"
            Case 132789
                Return "&登录"
            Case 132813
                Return "&配置"
            Case 132828
                Return "重新计算营养素"
            Case 132841
                Return "添加商品"
            Case 132846
                Return "保存标记"
            Case 132847
                Return "加载标记"
            Case 132848
                Return "过滤器"
            Case 132855
                Return "添加菜单"
            Case 132860
                Return "添加配料"
            Case 132864
                Return "替换配料"
            Case 132865
                Return "添加分离器"
            Case 132877
                Return "添加项目"
            Case 132896
                Return "规范化类别"
            Case 132912
                Return "规范化文本"
            Case 132915
                Return "规范化单位"
            Case 132924
                Return "规范化出产量单位"
            Case 132930
                Return "缩略图"
            Case 132933
                Return "食谱名单"
            Case 132939
                Return "菜单名单"
            Case 132954
                Return "标记集"
            Case 132955
                Return "从名单中选择一个标记名称，或者键入一个新的标记名称来保存 "
            Case 132957
                Return "另存为"
            Case 132967
                Return "营养素"
            Case 132971
                Return "营养摘要"
            Case 132972
                Return "营养价值是以每份食物[饮料]为100%"
            Case 132974
                Return "损耗"
            Case 132987
                Return "摘要"
            Case 132989
                Return "显示"
            Case 132997
                Return "在或以前"
            Case 132998
                Return "在或以后"
            Case 132999
                Return "在之间"
            Case 133000
                Return "大于"
            Case 133001
                Return "少比"
            Case 133005
                Return "统制"
            Case 133023
                Return "显示选项"
            Case 133043
                Return "当地图片变革"
            Case 133045
                Return "最大图片文件大小"
            Case 133046
                Return "最大图片尺寸"
            Case 133047
                Return "优化"
            Case 133049
                Return "激活图片自动转换为在网站上使用"
            Case 133057
                Return "为网站上传徽标"
            Case 133060
                Return "网页颜色"
            Case 133075
                Return "新密码"
            Case 133076
                Return "验证新密码"
            Case 133080
                Return "最后"
            Case 133081
                Return "首先"
            Case 133085
                Return "文件输出"
            Case 133096
                Return "食谱准备"
            Case 133097
                Return "食谱的成本计算"
            Case 133099
                Return "变异"
            Case 133100
                Return "食谱细节"
            Case 133101
                Return "菜单细节"
            Case 133108
                Return "打印什么？"
            Case 133109
                Return "要打印的商品的选择"
            Case 133111
                Return "有些类别"
            Case 133112
                Return "标记的商品"
            Case 133116
                Return "标记的食谱"
            Case 133121
                Return "标记的菜单"
            Case 133123
                Return "菜单花费"
            Case 133124
                Return "菜单描述"
            Case 133126
                Return "EGS标准"
            Case 133127
                Return "EGS现代"
            Case 133128
                Return "EGS二专栏"
            Case 133133
                Return "无效的文件名。 请输入一个有效的文件名。"
            Case 133144
                Return "食谱 #"
            Case 133161
                Return "纸张大小"
            Case 133162
                Return "页边距的单位"
            Case 133163
                Return "左边距"
            Case 133164
                Return "右边距"
            Case 133165
                Return "上边距"
            Case 133166
                Return "下边距"
            Case 133168
                Return "字体大小"
            Case 133172
                Return "小图片/数量——名称"
            Case 133173
                Return "小图片/名称——数量"
            Case 133174
                Return "中等图片/数量——名称"
            Case 133175
                Return "中等图片/名称——数量"
            Case 133176
                Return "大图片/数量——名称"
            Case 133177
                Return "大图片/名称——数量"
            Case 133196
                Return "名单选项"
            Case 133201
                Return "以下商品正在使用中，没有被删除。"
            Case 133207
                Return "食谱可以被使用为次级食谱"
            Case 133208
                Return "重量"
            Case 133222
                Return "细节选项"
            Case 133230
                Return "以下食谱正在使用中，未被删除。"
            Case 133241
                Return "重新计算价格中。 请等待…"
            Case 133242
                Return "重新计算营养价值中。 请等待…"
            Case 133251
                Return "分离器"
            Case 133254
                Return "按*排序"
            Case 133260
                Return "使用中的来源。"
            Case 133266
                Return "规范化关键字"
            Case 133286
                Return "定义"
            Case 133289
                Return "使用中的单位。"
            Case 133290
                Return "您不能合并两个或多个系统单位。"
            Case 133295
                Return "该单位不能被删除。 ¶只有定义的用户单位可以被删除。"
            Case 133314
                Return "只有定义的用户生产量单位可以被删除。"
            Case 133315
                Return "您不能合并两个或多个系统生产量单位。"
            Case 133319
                Return "使用中的生产量单位。"
            Case 133325
                Return "您确认要清除所有未使用的类别吗？"
            Case 133326
                Return "没有来源"
            Case 133330
                Return "丢失的文件"
            Case 133349
                Return "菜单#"
            Case 133350
                Return "%y （净数量）的项目"
            Case 133351
                Return "" '"在%p% （净数量）中为%y的配料"
            Case 133352
                Return "一份食物[饮料]+税的统制售价"
            Case 133353
                Return "一份食物[饮料]的统制售价"
            Case 133359
                Return "按数量排序"
            Case 133360
                Return "按日期排序"
            Case 133361
                Return "按类别排序"
            Case 133365
                Return "售价+税"
            Case 133367
                Return "按供应商排序"
            Case 133405
                Return "上传图片"
            Case 133519
                Return "选择一种颜色："
            Case 133692
                Return "建议的价格"
            Case 134032
                Return "联系方式"
            Case 134055
                Return "购买中"
            Case 134056
                Return "销售"
            Case 134061
                Return "版本、模块&许可证"
            Case 134083
                Return "测试"
            Case 134111
                Return "无法删除标记的项目。"
            Case 134176
                Return "商品营养素名单"
            Case 134177
                Return "食谱营养素名单"
            Case 134178
                Return "菜单营养素名单"
            Case 134182
                Return "组"
            Case 134194
                Return "无效的数量"
            Case 134195
                Return "无效的价格"
            Case 134320
                Return "发帐单的地址"
            Case 134332
                Return "信息"
            Case 134333
                Return "重要"
            Case 134525
                Return "您确认要取消已做的变动吗？"
            Case 134571
                Return "无效的价值"
            Case 135056
                Return "营养规则"
            Case 135058
                Return "添加营养规则"
            Case 135059
                Return "修改营养规则"
            Case 135070
                Return "净"
            Case 135256
                Return "售出数量"
            Case 135608
                Return "端口"
            Case 135948
                Return "包括次级食谱"
            Case 135955
                Return "无效数值。"
            Case 135963
                Return "数据库"
            Case 135967
                Return "替换在食谱。"
            Case 135968
                Return "替换在菜单。"
            Case 135971
                Return "&连接"
            Case 135978
                Return "新"
            Case 135979
                Return "重命名"
            Case 135985
                Return "存在"
            Case 135986
                Return "丢失"
            Case 135989
                Return "项目"
            Case 135990
                Return "刷新"
            Case 136018
                Return "所有权"
            Case 136025
                Return "数据库转换"
            Case 136171
                Return "改变单位"
            Case 136265
                Return "次级食谱"
            Case 136601
                Return "重新设置"
            Case 136905
                Return "货币符号"
            Case 137019
                Return "变动"
            Case 137030
                Return "默认"
            Case 137070
                Return "一般设置"
            Case 138137
                Return "删除的"
            Case 138244
                Return "销售项目"
            Case 138402
                Return "所有传输已成功完成"
            Case 138412
                Return "<未定义>"
            Case 140056
                Return "文件"
            Case 140100
                Return "备份进展中"
            Case 140101
                Return "恢复进展中"
            Case 140129
                Return "错误，当恢复备份时"
            Case 140130
                Return "错误，当创造备份时"
            Case 140180
                Return "保存备用文件的道路"
            Case 143001
                Return "共享"
            Case 143002
                Return "不共享"
            Case 143008
                Return "损耗"
            Case 143013
                Return "修改"
            Case 143014
                Return "用户"
            Case 143508
                Return "食谱正被作为次级食谱来使用"
            Case 143509
                Return "线间距"
            Case 143987
                Return "项目类型"
            Case 143995
                Return "行动"
            Case 144591
                Return "时间"
            Case 144682
                Return "营养价值是以每100g或100 ml为100%"
            Case 144684
                Return "营养价值是以每1个生产量单位为100%"
            Case 144685
                Return "每个生产量单位为100%"
            Case 144686
                Return "每%Y为100%"
            Case 144687
                Return "每100g或100 ml为100%"
            Case 144688
                Return "无效"
            Case 144689
                Return "营养价值是以每1个生产量单位/100g或100 ml为100%"
            Case 144716
                Return "历史"
            Case 144734
                Return "销售项目名单"
            Case 144738
                Return "每%Y的重量"
            Case 145006
                Return "调动"
            Case 146056
                Return "边际收益"
            Case 146067
                Return "平衡"
            Case 146080
                Return "客户端"
            Case 146114
                Return "如果是不同的供应商，显示在新的一页"
            Case 146211
                Return "发行类型"
            Case 147070
                Return "行"
            Case 147075
                Return "无效日期"
            Case 147126
                Return "首先去除现有的标记"
            Case 147174
                Return "打开"
            Case 147441
                Return "这个销售项目已经链接了。"
            Case 147462
                Return "比率"
            Case 147520
                Return "主要"
            Case 147647
                Return "SQL服务器不存在，或者访问否认"
            Case 147652
                Return "删除"
            Case 147692
                Return "膳食信息"
            Case 147699
                Return "覆盖"
            Case 147700
                Return "共计价格"
            Case 147703
                Return "准备的份数"
            Case 147704
                Return "被留下的生产量"
            Case 147706
                Return "返回的生产量"
            Case 147707
                Return "丢失的生产量"
            Case 147708
                Return "售出的生产量"
            Case 147710
                Return "特别售出的生产量"
            Case 147713
                Return "EGS布局"
            Case 147727
                Return "费用"
            Case 147729
                Return "评分"
            Case 147733
                Return "选择一种语言"
            Case 147737
                Return "键入数量并且选择单位"
            Case 147743
                Return "上传"
            Case 147753
                Return "劳动力的成本控制"
            Case 147771
                Return "每小时速度"
            Case 147772
                Return "每分钟速度"
            Case 147773
                Return "人"
            Case 147774
                Return "时间（小时：分钟）"
            Case 149501
                Return "使用直接输入——输出"
            Case 149513
                Return "批准"
            Case 149531
                Return "制成品"
            Case 149645
                Return "链接到"
            Case 149706
                Return "去除链接"
            Case 149766
                Return "前缀"
            Case 149774
                Return "清除"
            Case 150333
                Return "成功地删除!"
            Case 150341
                Return "货币兑换"
            Case 150353
                Return "排序"
            Case 150634
                Return "电子邮件已被成功发送。"
            Case 150644
                Return "SMTP服务器是需要从您的计算机发送电子邮件。"
            Case 150688
                Return "这种应用的执照已经到期了。"
            Case 150707
                Return "帐户"
            Case 151011
                Return "瑞士-总部"
            Case 151019
                Return "商品关键字"
            Case 151020
                Return "食谱关键字"
            Case 151023
                Return "记数器"
            Case 151250
                Return "无任何改变"
            Case 151286
                Return "标准"
            Case 151299
                Return "请输入所需要的信息"
            Case 151322
                Return "包括在存货"
            Case 151336
                Return "装载一套标记"
            Case 151344
                Return "保存标记为商品"
            Case 151345
                Return "保存标记为盘"
            Case 151346
                Return "保存标记为菜单"
            Case 151364
                Return "选择两个或多个文本"
            Case 151389
                Return "清除文本"
            Case 151400
                Return "商品成本"
            Case 151404
                Return "增值税"
            Case 151424
                Return "转换成最佳的单位"
            Case 151427
                Return "按项目名称排序"
            Case 151435
                Return "主题"
            Case 151437
                Return "RecipeNet食谱网"
            Case 151438
                Return "CALCMENU"
            Case 151459
                Return "您的电子邮件"
            Case 151499
                Return "替换提案"
            Case 151854
                Return "Excel"
            Case 151906
                Return "没被发现的电子邮件"
            Case 151907
                Return "请登录您适当的用户名和密码。"
            Case 151910
                Return "登录"
            Case 151911
                Return "登出"
            Case 151912
                Return "忘记了您的密码？"
            Case 151915
                Return "请提供下面请求的信息。"
            Case 151916
                Return "与星号(*)需要领域。"
            Case 151918
                Return "请提供一封合法的电子邮件。"
            Case 151976
                Return "默认生产地点"
            Case 152004
                Return "树型视图"
            Case 152141
                Return "商品管理"
            Case 152146
                Return "邮编"
            Case 155024
                Return "图片管理"
            Case 155046
                Return "翻译"
            Case 155052
                Return "递交"
            Case 155118
                Return "寄发购物单到口袋"
            Case 155163
                Return "姓"
            Case 155170
                Return "受欢迎的%name!"
            Case 155205
                Return "主页"
            Case 155225
                Return "PDF"
            Case 155236
                Return "主要语言"
            Case 155245
                Return "关于我们"
            Case 155263
                Return "像素"
            Case 155264
                Return "翻译"
            Case 155374
                Return "会计编号"
            Case 155507
                Return "允许"
            Case 155575
                Return "默认自动输出地点"
            Case 155601
                Return "没有选择的项目。"
            Case 155642
                Return "食谱交换"
            Case 155713
                Return "%r存在。"
            Case 155731
                Return "CALCMENU赞成2009年"
            Case 155763
                Return "按数量比较"
            Case 155764
                Return "按名称比较"
            Case 155841
                Return "归档恢复"
            Case 155862
                Return "每"
            Case 155942
                Return "装载保存了的购物单"
            Case 155967
                Return "字段分隔符"
            Case 155994
                Return "不激活"
            Case 155996
                Return "电子邮件地址"
            Case 156000
                Return "移向一个新供应商"
            Case 156012
                Return "支持"
            Case 156015
                Return "联系我们"
            Case 156016
                Return "大会办公处"
            Case 156141
                Return "备份或恢复数据库"
            Case 156337
                Return "链接营养素"
            Case 156344
                Return "无效选择"
            Case 156355
                Return "档案"
            Case 156356
                Return "包括"
            Case 156405
                Return "请释放一些空间然后点击再试"
            Case 156413
                Return "附属食谱的定义 "
            Case 156485
                Return "导入以后删除文件"
            Case 156552
                Return "现在备份"
            Case 156590
                Return "从CSV文件（Excel）导入商品"
            Case 156669
                Return "网站"
            Case 156672
                Return "使用的在线（为网络内容）"
            Case 156683
                Return "正本"
            Case 156720
                Return "数字太长"
            Case 156721
                Return "名字太长"
            Case 156722
                Return "供应商太长"
            Case 156723
                Return "类别太长"
            Case 156725
                Return "描述太长"
            Case 156734
                Return "两个单位是相同的"
            Case 156742
                Return "到期日期"
            Case 156751
                Return "电话：  +41 848 000 357<br> (英国，法国，德国，操作时间： 8:30是6pm格林维志时间+01 ：00) <br><br>Tel ：  +41 32 544 00 17<br> (仅英语，操作时间： 3am-830am格林维志时间+01 ：00)"
            Case 156752
                Return "免费：  1-800-964-9357<br> (仅英语，操作时间： 9am-3am太平洋标准时)"
            Case 156753
                Return "电话： +63 2 687 3179<br> (仅英语，操作时间： 12am-6pm格林维志时间+08 ：00)"
            Case 156754
                Return "文件名"
            Case 156825
                Return "一千"
            Case 156870
                Return "您能确定吗?"
            Case 156925
                Return "下载成功!"
            Case 156938
                Return "激活"
            Case 156941
                Return "小型厨房"
            Case 156955
                Return "私有"
            Case 156957
                Return "旅馆"
            Case 156959
                Return "共享"
            Case 156960
                Return "递交"
            Case 156961
                Return "规定价格"
            Case 156962
                Return "没递交"
            Case 156963
                Return "价格"
            Case 156964
                Return "发现"
            Case 156965
                Return "出产量"
            Case 156966
                Return "受影响的纪录"
            Case 156967
                Return "请进入正确日期。"
            Case 156968
                Return "无效图像文件格式"
            Case 156969
                Return "请进入图像文件上装。 否则，任它空白。"
            Case 156970
                Return "进入类别信息"
            Case 156971
                Return "进入集合价格信息"
            Case 156972
                Return "进入主题词信息"
            Case 156973
                Return "进入单位信息"
            Case 156974
                Return "进入出产量信息"
            Case 156975
                Return "创造新的食谱并且递交给大会办公处为使用与其他旅馆。"
            Case 156976
                Return "商品是包括您的食谱和菜单的基本的元素或项目。"
            Case 156977
                Return "如果您有关于这软件的任何询问或技术问题。"
            Case 156978
                Return "父母主题词"
            Case 156979
                Return "主题词的名字"
            Case 156980
                Return "配置"
            Case 156981
                Return "税率"
            Case 156982
                Return "查寻结果"
            Case 156983
                Return "抱歉，结果未被发现。"
            Case 156984
                Return "无效用户名或密码。"
            Case 156986
                Return "项目已经存在。"
            Case 156987
                Return "成功地被保存了。"
            Case 156996
                Return "复制权© EGS Enggist & Grandjean SA，瑞士软件2004年。"
            Case 157002
                Return "价格为单位没有被定义。 请选择一个单位。"
            Case 157020
                Return "旧税"
            Case 157026
                Return "中等"
            Case 157033
                Return "系统将更新所有商品的价格。 请等待…"
            Case 157034
                Return "认证"
            Case 157038
                Return "月"
            Case 157039
                Return "年"
            Case 157040
                Return "没有主题词可利用。"
            Case 157041
                Return "被否认的通入"
            Case 157049
                Return "是否是保存？"
            Case 157055
                Return "学生版本"
            Case 157056
                Return "您是否想要取消？"
            Case 157057
                Return "明显项目现在分享。"
            Case 157076
                Return "帮助总结"
            Case 157079
                Return "以下明显项目没有递交并且不可能转移："
            Case 157084
                Return "使用以下明显项目和没有被删除："
            Case 157125
                Return "看法"
            Case 157130
                Return "您的信用卡信息已被成功发送。您的会员注册将在三天内被处理。谢谢！"
            Case 157132
                Return "个人(共享)"
            Case 157133
                Return "个人(不共享)"
            Case 157134
                Return "访客"
            Case 157136
                Return "积分"
            Case 157139
                Return "最坏!"
            Case 157140
                Return "好!"
            Case 157141
                Return "意想不到!"
            Case 157142
                Return "在导入之前删除未使用的商品单位"
            Case 157151
                Return "其他链接"
            Case 157152
                Return "用户回顾"
            Case 157153
                Return "The recipient will be prompted to accept these items."
            Case 157154
                Return "下列项目不能被得到，因为它们是由其他用户所拥有的。"
            Case 157155
                Return "有人想给您以下食谱："
            Case 157156
                Return "促销"
            Case 157157
                Return "用户意见"
            Case 157158
                Return "独创性"
            Case 157159
                Return "结果"
            Case 157160
                Return "困难"
            Case 157161
                Return "一天的食谱"
            Case 157164
                Return "Cardholder name"
            Case 157165
                Return "Credit card number"
            Case 157166
                Return "Record Limit"
            Case 157168
                Return "Bank"
            Case 157169
                Return "PayPal网"
            Case 157170
                Return "您不能在您的国家进行网上订购。"
            Case 157171
                Return "Become a member"
            Case 157172
                Return "Upgrade fee"
            Case 157173
                Return "Subscription fee"
            Case 157174
                Return "Upgrade packs"
            Case 157176
                Return "Total records used"
            Case 157177
                Return "We offer a variety of solutions to fit your needs"
            Case 157178
                Return "Trial user"
            Case 157179
                Return "Tell a Friend"
            Case 157180
                Return "Friend's e-mail address"
            Case 157182
                Return "常见问题解答"
            Case 157183
                Return "服务的期限和条件"
            Case 157214
                Return "只为作标记的食谱创建采购单"
            Case 157217
                Return "只为作标记的菜单创建采购单"
            Case 157226
                Return "明显食谱被送了为获得批准。"
            Case 157233
                Return "副产品不可能是大于或等于100%。"
            Case 157268
                Return "流通的货币。"
            Case 157269
                Return "使用套价格。"
            Case 157273
                Return "因为他们未递交也没拥有，不能分享以下项目。"
            Case 157274
                Return "交换率"
            Case 157275
                Return "被列出的所有项目将被合并入一个。 请选择一个项目由用户使用。 其他项目从数据库将被删除。"
            Case 157276
                Return "成功地合并。"
            Case 157277
                Return "总成本"
            Case 157297
                Return "请选择至少一个项目。"
            Case 157299
                Return "编辑档案并且确定外观。"
            Case 157300
                Return "请输入您的新口令。 密码不可能超出20个字符。 当您做时，点击""提交""。"
            Case 157301
                Return "请进入图像文件(jpeg或JPG、bmp等等)那您想要上装。 否则，任它空白。 (笔记： 不支持GIF文件。 所有图片被复制然后被转换成法线和指图jpeg格式。 )"
            Case 157302
                Return "名义上搜寻成份或部分的命名(用途[*]星号)。 要增加迅速，进入[净quanitity] _ [单位] _ [成份]象200 g Oel高油"
            Case 157303
                Return "要增加或编辑商品价格，进入新的价格并且定义测量单位。 分配那个单位比率到原始的单位。 例如，原价和单位是美国$11每公斤(公斤)。 如果您想要增加单位袋子，您必须定义那个袋子的价格，或者定义那里多少公斤在1个袋子(比率)。"
            Case 157304
                Return "名义上搜寻主题词或名字的部分。 为多个主题词使用逗号[，]。 例如，搜寻""牛肉，调味汁，婚礼""。"
            Case 157305
                Return "请选择一个项目"
            Case 157306
                Return "无效的文件类型。"
            Case 157310
                Return "商品细节"
            Case 157314
                Return "在增加商品价格时，使用主要的或大的单位"
            Case 157320
                Return "共享"
            Case 157322
                Return "用户协议"
            Case 157323
                Return "授予"
            Case 157329
                Return "终端"
            Case 157334
                Return "警告： 如果另一名用户修改了这个纪录，您也许丢失所有您的变动。 您是否想要刷新这页？"
            Case 157339
                Return "每页的消息"
            Case 157340
                Return "快速浏览"
            Case 157341
                Return "在每页"
            Case 157342
                Return "记录修改另一名用户。  点击OK进行。"
            Case 157343
                Return "这个纪录由另一名用户删除。"
            Case 157345
                Return "递交给Headoffice"
            Case 157346
                Return "不共享"
            Case 157378
                Return "会员"
            Case 157379
                Return "现在订阅"
            Case 157380
                Return "您的订阅在%n.将到期。"
            Case 157381
                Return "您的订阅到期了。"
            Case 157382
                Return "使用我剩余的点(信用)扩大我的会员资格"
            Case 157383
                Return "您到达了您的磁盘空间极限。 请删除你的一些食谱或商品。 谢谢。"
            Case 157384
                Return "无效交易"
            Case 157385
                Return "谢谢!"
            Case 157387
                Return "您将被改方向对Paypal完成您的订阅。 请使用的货币为了充电您正确数额的选择。 从名单请选择如下"
            Case 157388
                Return "邀请加入"
            Case 157404
                Return "等待处理。"
            Case 157405
                Return "为询问，请电子邮件我们在"
            Case 157408
                Return "仅成员和试验用户能访问这页。 您是否想要处理您自己的食谱在食谱Gallery.com ？  去捐款菜单并且订阅作为成员。"
            Case 157435
                Return "在输出之前自动调动到销路"
            Case 157437
                Return "原料"
            Case 157446
                Return "月"
            Case 157594
                Return "同意"
            Case 157595
                Return "拒绝"
            Case 157596
                Return "没有用户回顾"
            Case 157604
                Return "电子邮件支持"
            Case 157607
                Return "电话支持"
            Case 157608
                Return "网上支持"
            Case 157616
                Return "美国"
            Case 157617
                Return "亚洲和世界其他地方"
            Case 157629
                Return "批准"
            Case 157633
                Return "不批准"
            Case 157695
                Return "会计参考号"
            Case 157772
                Return "任意"
            Case 157802
                Return "证实密码"
            Case 157901
                Return "掩藏存在"
            Case 157926
                Return "签名"
            Case 158005
                Return "许可证"
            Case 158019
                Return "检查请求状态"
            Case 158169
                Return "请选择您的付款条件。¶ ¶预付款通过："
            Case 158170
                Return "请e - mail我们您的信用卡在<a href='mailto:info@calcmenu.com'>info@calcmenu.com</a>信用卡资料。信用卡的类型（VISA，万事达，美国运通），持卡人姓名，信用卡号码（请包括3位安全代码（CVC2/CVV2），你可以找到您的信用卡背面）和到期日。"
            Case 158171
                Return "银行或电汇"
            Case 158174
                Return "<b>注：</b>请告诉我们，一旦转移了进展。这将需要1-2周之后，我们收到银行的确认，我们就转移。"
            Case 158186
                Return "改变密码"
            Case 158220
                Return "创造新的商品名字与250个字符并且包括字母数字的参考数字、税率、四副产品百分比、类别、供应商和其他有用的信息例如产品说明，准备，烹调技巧、提炼方法和存贮。"
            Case 158229
                Return "图片"
            Case 158230
                Return "商品、食谱和菜单可以使用他们的名字或参考数字被搜寻。 您能使用类别和主题词也搜寻。 对于商品，当搜寻时，您能也使用供应商、日期编码的或最后更新、价格范围和营养价值。 为食谱和菜单，您能搜寻使用使用和没使用的项目。"
            Case 158232
                Return "行动标记是捷径在执行可能适用于一份明显商品、食谱或者菜单的一个相似的作用。 您能使用行动标记分配商品，食谱，或者菜单到类别和主题词，删除他们，通过电子邮件、印刷品、份额和unshare出口，送到其他用户，无需必须重覆他们为每个项目。 这在进行行动很多时间保存您和努力对明显项目。"
            Case 158234
                Return "营养素连接和演算"
            Case 158238
                Return "供应商管理"
            Case 158240
                Return "类别，关键字，来源管理"
            Case 158243
                Return "税率管理"
            Case 158246
                Return "单位管理"
            Case 158249
                Return "打印， PDF和电子表格输出"
            Case 158306
                Return "选择"
            Case 158346
                Return "更多"
            Case 158376
                Return "理论上的统制售价"
            Case 158511
                Return "如果您相信这不是实际情形，请送我们电子邮件<a href='mailto:%email'>%email</a>"
            Case 158577
                Return "站点语言"
            Case 158585
                Return "Headoffice"
            Case 158588
                Return "因为他们由另一名用户，拥有不能递交以下项目。"
            Case 158653
                Return "手机号"
            Case 158677
                Return "销售项目¶号码"
            Case 158694
                Return "改变信息"
            Case 158696
                Return "仅为菲律宾客户"
            Case 158730
                Return "排除"
            Case 158783
                Return "包括食谱/次级食谱"
            Case 158810
                Return "计算价格"
            Case 158835
                Return "按税排序"
            Case 158837
                Return "按价格排序"
            Case 158839
                Return "按物品成本排序"
            Case 158840
                Return "按常数排序"
            Case 158845
                Return "按售价排序"
            Case 158846
                Return "按统制价格排序"
            Case 158849
                Return "高的"
            Case 158850
                Return "低的"
            Case 158851
                Return "创建人："
            Case 158860
                Return "修改销售点系统设置"
            Case 158902
                Return "开始时间"
            Case 158912
                Return "申请"
            Case 158935
                Return "总收入"
            Case 158947
                Return "您将被改方向对Paypal完成您的指令。"
            Case 158952
                Return "批准的"
            Case 158953
                Return "未批准的"
            Case 158960
                Return "这个作用失去了能力。 如果您需要新的食谱，请与您的总店联系。"
            Case 158998
                Return "搜索条件"
            Case 158999
                Return "商品、食谱和菜单名单可以与他们的细节、价格和营养价值一起打印。 购物单或成份名单与用于各种各样的食谱的渐增数量一起可能也打印。 PDF和擅长文件可能为各种各样的报告也被创造。"
            Case 159000
                Return "套价格和多货币管理"
            Case 159009
                Return "边界"
            Case 159035
                Return "不完全"
            Case 159064
                Return "名字不能是空白的"
            Case 159082
                Return "根据最后修改过的日期来更新产品"
            Case 159089
                Return "取消要求认可的申请"
            Case 159112
                Return "为获得批准"
            Case 159113
                Return "可继承的"
            Case 159133
                Return "运输信息"
            Case 159139
                Return "结构"
            Case 159140
                Return "单位太长"
            Case 159141
                Return "单位%n不存在。"
            Case 159142
                Return "%n不可能是空白的。"
            Case 159144
                Return "文件导入中。 请等待…"
            Case 159145
                Return "项目保存中。 请等待…"
            Case 159162
                Return "&隐藏细节"
            Case 159168
                Return "按净数量排序"
            Case 159169
                Return "按总数量排序"
            Case 159171
                Return "进度表"
            Case 159181
                Return "按共计排序"
            Case 159264
                Return "导入商品CSV/供应商网络"
            Case 159273
                Return "总边际收益"
            Case 159275
                Return "由执照限制"
            Case 159298
                Return "菜单关键字"
            Case 159349
                Return "重新设置过滤器"
            Case 159360
                Return "特级厨师"
            Case 159361
                Return "行政厨师"
            Case 159362
                Return "使用的选择的项目。"
            Case 159363
                Return "进入品牌信息"
            Case 159364
                Return "品牌"
            Case 159365
                Return "角色"
            Case 159366
                Return "使用SMTP在服务器"
            Case 159367
                Return "使用SMTP在网络"
            Case 159368
                Return "商标"
            Case 159369
                Return "按*比较 "
            Case 159370
                Return "成功地导入"
            Case 159372
                Return "全球性"
            Case 159379
                Return "上升"
            Case 159380
                Return "下降"
            Case 159381
                Return "公开对所有用户"
            Case 159382
                Return "改变信仰者到系统食谱"
            Case 159383
                Return "不要公开"
            Case 159384
                Return "物产"
            Case 159385
                Return "递交词条"
            Case 159386
                Return "价格和营养素未被重估。"
            Case 159387
                Return "价格和营养素被重估了。"
            Case 159388
                Return "创造一张新的菜单卡片"
            Case 159389
                Return "修改菜单卡片"
            Case 159390
                Return "被送的电子邮件。"
            Case 159391
                Return "批准的价格"
            Case 159424
                Return "这个作用失去了能力。 如果您需要新的商品，请与您的总店联系。"
            Case 159426
                Return "名义上搜寻成份或分开名字。 要增加迅速，进入[净quanitity] _ [单位] _ [成份]。"
            Case 159430
                Return "注册信息成功地被保存了。"
            Case 159433
                Return "递交给系统"
            Case 159434
                Return "递交给系统"
            Case 159435
                Return "移向一个新的类别"
            Case 159436
                Return "电子邮件发令者为系统戒备通知"
            Case 159437
                Return "文件成功地被上装了。"
            Case 159444
                Return "强加图象尺寸"
            Case 159445
                Return "时区"
            Case 159446
                Return "图象处理"
            Case 159457
                Return "SQL服务器充分的文本查寻有能力执行复杂询问。 这些询问可能包括词或词组搜寻，接近度查寻、变形的比赛(驱动=驾驶了)和相关性等第(多么接近的是词)"
            Case 159458
                Return "充分的人口"
            Case 159459
                Return "充分的文本查寻"
            Case 159460
                Return "分钟"
            Case 159461
                Return "每"
            Case 159462
                Return "奔跑"
            Case 159463
                Return "增加人口"
            Case 159464
                Return "语言词破碎机"
            Case 159471
                Return "IP地址"
            Case 159472
                Return "封锁的IP名单"
            Case 159473
                Return "块IP，当注册企图到达"
            Case 159474
                Return "请进入至少¶字符"
            Case 159485
                Return "递交给食谱交换"
            Case 159486
                Return "递交给食谱交换"
            Case 159487
                Return "您批准了这份食谱。 它能由所有用户现在看见。"
            Case 159488
                Return "未知的语言"
            Case 159607
                Return "独立食谱管理软件"
            Case 159608
                Return "食谱管理软件为一致用户在网络"
            Case 159609
                Return "基于互联网的食谱管理软件"
            Case 159610
                Return "存货和后面办公室管理软件"
            Case 159611
                Return "食谱观察者为口袋个人计算机"
            Case 159612
                Return "命令采取和营养监视软件"
            Case 159613
                Return "E菜谱软件"
            Case 159699
                Return "更新现有的项目"
            Case 159707
                Return "法国"
            Case 159708
                Return "德国"
            Case 159751
                Return "站点"
            Case 159778
                Return "高级"
            Case 159779
                Return "基本"
            Case 159782
                Return "链接销售项目到产品"
            Case 159783
                Return "链接销售项目到食谱或菜单"
            Case 159795
                Return "销售点导——配置"
            Case 159918
                Return "您没有权利访问这个功能。"
            Case 159924
                Return "管理"
            Case 159925
                Return "无效转换"
            Case 159929
                Return "页面选择"
            Case 159934
                Return "营养信息"
            Case 159940
                Return "输出更新"
            Case 159941
                Return "输出所有"
            Case 159942
                Return "输出目录"
            Case 159943
                Return "质量"
            Case 159944
                Return "父母"
            Case 159946
                Return "CALCMENU网2007年"
            Case 159947
                Return "选择或上装文件"
            Case 159949
                Return "格式不应该超出10个字符。"
            Case 159950
                Return "营养名字不应该超出25个字符。"
            Case 159951
                Return "角色"
            Case 159962
                Return "进入税收消息"
            Case 159963
                Return "进入翻译"
            Case 159966
                Return "移动明显项目向新的品牌"
            Case 159967
                Return "输入缺省站点名字："
            Case 159968
                Return "进入缺省网站题材"
            Case 159969
                Return "由物产将处理的物产使能编组站点admin ："
            Case 159970
                Return "在可以使用它或被出版之前，要求用户首先递交信息给approver ："
            Case 159971
                Return "输入翻译为每种对应的语言或将使用缺省文本："
            Case 159973
                Return "选择应该属于这物产的站点"
            Case 159974
                Return "选择可利用的语言为翻译商品、食谱、菜单和其他信息使用"
            Case 159975
                Return "选择一个或更多价格小组为分配价格使用到您的商品、食谱和菜单"
            Case 159976
                Return "检查项目包括"
            Case 159977
                Return "所有者名单"
            Case 159978
                Return "选择格式如下"
            Case 159979
                Return "选择基本的名单清洗"
            Case 159981
                Return "下列是共有的站点为这个项目"
            Case 159982
                Return "移动明显向新的来源"
            Case 159987
                Return "请求类型"
            Case 159988
                Return "请求 "
            Case 159990
                Return "改变品牌"
            Case 159994
                Return "替换成份在菜单"
            Case 159997
                Return "全部共享"
            Case 160004
                Return "第一层"
            Case 160005
                Return "选择的成份应该有以下单位："
            Case 160008
                Return "步"
            Case 160009
                Return "更多行动"
            Case 160012
                Return "这份食谱或菜单在网被出版。"
            Case 160013
                Return "这份食谱或菜单在网没有被出版。"
            Case 160014
                Return "记住我"
            Case 160016
                Return "查看所有者"
            Case 160018
                Return "这件商品在网被出版。"
            Case 160019
                Return "这件商品在网没有被出版。"
            Case 160020
                Return "这件商品被暴露。"
            Case 160021
                Return "这件商品没有被暴露。"
            Case 160023
                Return "为打印"
            Case 160028
                Return "不被出版"
            Case 160030
                Return "增加到购物单"
            Case 160033
                Return "增加主题词"
            Case 160035
                Return "您试图登录%n时间"
            Case 160036
                Return "撤销了这个帐户"
            Case 160037
                Return "与您的系统管理员联系恢复活动这个帐户。"
            Case 160038
                Return "我的档案"
            Case 160039
                Return "最后登录"
            Case 160040
                Return "您没有签名。"
            Case 160041
                Return "页语言"
            Case 160042
                Return "主要翻译"
            Case 160043
                Return "扼要套价格"
            Case 160045
                Return "每页的行"
            Case 160046
                Return "缺省显示"
            Case 160047
                Return "成份数量"
            Case 160048
                Return "访问的为时"
            Case 160049
                Return "被接受的""%f"""
            Case 160050
                Return "长度"
            Case 160051
                Return "没接受""%f"""
            Case 160055
                Return "数量必须是大于0。"
            Case 160056
                Return "创造一份新的次级食谱"
            Case 160057
                Return "会议到期了。"
            Case 160058
                Return "您的注册有到期的由于不活泼%n分钟。"
            Case 160065
                Return "没有名字"
            Case 160066
                Return "是否是关闭？"
            Case 160067
                Return "您的词条要求认同"
            Case 160068
                Return "点击""%s""按钮请求认同。"
            Case 160070
                Return "将被处理的明显项目"
            Case 160071
                Return "这个词条递交了为获得批准。"
            Case 160072
                Return "已经有一个现有的要求这个词条。"
            Case 160074
                Return "选择单位"
            Case 160082
                Return "新的请求等候您的认同。"
            Case 160085
                Return "您的请求被回顾了。"
            Case 160086
                Return "印刷品营养素名单"
            Case 160087
                Return "印刷品名单"
            Case 160088
                Return "印刷品细节"
            Case 160089
                Return "激活"
            Case 160090
                Return "创建"
            Case 160091
                Return "从名单去除选择的项目。"
            Case 160093
                Return "递交给系统为全球性分享"
            Case 160094
                Return "使内容可利用在报亭浏览器"
            Case 160095
                Return "创造一个系统拷贝"
            Case 160096
                Return "替换用于食谱和菜单的成份"
            Case 160098
                Return "不要出版在网"
            Case 160100
                Return "建立将被购买的成份名单"
            Case 160101
                Return "您能使用文本作为不需要数量和价格定义的成份。"
            Case 160102
                Return "创造您自己的食谱数据库，与其他用户分享它，打印它和甚而建立一个购物单为它。"
            Case 160103
                Return "菜单是成份或食谱名单可利用在膳食。"
            Case 160105
                Return "组织基本的信息例如那些与用户、供应商等等有关。"
            Case 160106
                Return "欢迎"
            Case 160107
                Return "欢迎到%s"
            Case 160108
                Return "定做您的意图和其他设置。"
            Case 160109
                Return "网站外形"
            Case 160110
                Return "定做网站的名字、题材等等。"
            Case 160111
                Return "认同发送"
            Case 160112
                Return "商品、食谱和其他信息认同。"
            Case 160113
                Return "SMTP和机敏的通知设置"
            Case 160114
                Return "配置与您的邮件服务器的连接; 使能或使戒备失去能力。"
            Case 160115
                Return "设置最大注册企图，并且显示器阻拦了IP地址。"
            Case 160116
                Return "印刷品外形"
            Case 160117
                Return "定义打印格式的倍数作为外形。"
            Case 160118
                Return "定义翻译商品、食谱、菜单和其他信息的语言名单。"
            Case 160119
                Return "可利用的货币为货币兑换和套价格定义。"
            Case 160120
                Return "与商品、食谱和菜单一起使用与多套价格。"
            Case 160121
                Return "物产是小组站点。"
            Case 160122
                Return "站点组织一起研究特殊套的用户食谱。"
            Case 160123
                Return "处理工作在%s的用户"
            Case 160124
                Return "图象处理的特选"
            Case 160125
                Return "定义标准图象尺寸为商品、食谱和菜单。"
            Case 160130
                Return "辨认商品的商标或特别名字。"
            Case 160132
                Return "曾经由共同的属性编组商品、食谱或者菜单。"
            Case 160135
                Return "主题词提供描写细节给商品、食谱或者菜单。 用户能分配多个主题词每份商品、食谱或者菜单。"
            Case 160139
                Return "定义34营养素价值为营养素象能量、碳水化合物、蛋白质和油脂。"
            Case 160141
                Return "创造可以使用作为另外的过滤器为搜寻的规则。"
            Case 160151
                Return "用于定义商品价格的被预定义的(或系统)单位名单并且在内码食谱和菜单。"
            Case 160152
                Return "用户能补充说到这张名单。"
            Case 160153
                Return "使用在价格计算"
            Case 160154
                Return "来源提到一份特殊食谱的起源。 它可以是厨师、书、杂志、食品供应公司、组织或者网站。"
            Case 160155
                Return "从CALCMENU进口商品、食谱或者菜单赞成， CALCMENU企业和其他EGS产品。"
            Case 160156
                Return "交换率维护为不同的货币"
            Case 160157
                Return "删除未使用的文本。"
            Case 160158
                Return "格式化所有文本。"
            Case 160159
                Return "打印商品名单在HTML，电子表格， PDF和RTF格式。"
            Case 160160
                Return "打印商品细节在HTML，电子表格， PDF和RTF格式。"
            Case 160161
                Return "打印食谱细节在HTML，电子表格， PDF和RTF格式。"
            Case 160162
                Return "打印食谱名单在HTML，电子表格， PDF和RTF格式。"
            Case 160163
                Return "打印菜单细节在HTML，电子表格， PDF和RTF格式。"
            Case 160164
                Return "菜单工程学允许您评估现在和未来食谱定价和设计。"
            Case 160169
                Return "装载菜单拟订名单"
            Case 160170
                Return "修改或预览被保存的菜单卡片。"
            Case 160175
                Return "修改，预览或者打印被保存的购物单。"
            Case 160177
                Return "安全性"
            Case 160180
                Return "规范化项目的格式"
            Case 160181
                Return "清洗项目"
            Case 160182
                Return "角色权利"
            Case 160184
                Return "TCPOS输出"
            Case 160185
                Return "外销项目"
            Case 160187
                Return "创造可以使用作为成份为您的食谱的新的地方商品。"
            Case 160188
                Return "显示被保存的标记名单"
            Case 160189
                Return "显示将被购买的项目名单。"
            Case 160190
                Return "创造根据可利用的食谱的您自己的菜单在您的数据库。"
            Case 160191
                Return "创造用于食谱和菜单的文本。"
            Case 160200
                Return "按名称排序"
            Case 160202
                Return "从名单选择"
            Case 160209
                Return "请输入序号、标题名和产品密匙。您可以在%s提供的文献中找到这个信息。"
            Case 160210
                Return "被要的项目"
            Case 160211
                Return "不需要的项目"
            Case 160212
                Return "草稿"
            Case 160217
                Return "存档路径"
            Case 160218
                Return "CSV进口数据错误"
            Case 160219
                Return "等待需要被修理商品的名单"
            Case 160220
                Return "定义选择为商品进口"
            Case 160254
                Return "请重新开始窗口服务%n为您的对作为作用的变动。"
            Case 160258
                Return "货币不匹配选上的套价格。"
            Case 160259
                Return "名字或数字已经存在。"
            Case 160260
                Return "进口的日期"
            Case 160262
                Return "营养价值是每1个生产量单位"
            Case 160292
                Return "致敏物"
            Case 160293
                Return "食物过敏或敏感性名单联合经营。"
            Case 160295
                Return "这个帐户当前是在使用中。 请再试试以后。"
            Case 160353
                Return "购买套价格"
            Case 160354
                Return "卖套价格"
            Case 160423
                Return "独立食谱或菜单管理软件"
            Case 160433
                Return "*的消耗量"
            Case 160500
                Return "Text Management"
            Case 160687
                Return "交替的项目颜色"
            Case 160688
                Return "正常项目颜色"
            Case 160690
                Return "请注意:，当您恢复，它使用系统自动地将切除用户当前。"
            Case 160691
                Return "备份或恢复图片"
            Case 160716
                Return "默认情况下设置项目对全球性"
            Case 160774
                Return "撤销"
            Case 160775
                Return "去除落后的零"
            Case 160777
                Return "这里点击在网上学会更多关于CALCMENU。"
            Case 160788
                Return "激活了选择的项目。"
            Case 160789
                Return "撤销了选择的项目。"
            Case 160790
                Return "是否是去除选择的项目？"
            Case 160791
                Return "成功地去除了选择的项目。"
            Case 160801
                Return "您能只合并两个或多个相似的食谱。"
            Case 160802
                Return "是否是合并选择的项目？"
            Case 160803
                Return "是否是清洗项目？"
            Case 160804
                Return "请填好必需的领域。"
            Case 160805
                Return "选择两个或多个项目合并。"
            Case 160806
                Return "是否是撤销选择的项目？"
            Case 160863
                Return "商品价格表"
            Case 160940
                Return "有效性日期"
            Case 160941
                Return "连接的销售项目"
            Case 160953
                Return "卖套因素价格到购买套价格"
            Case 160958
                Return "与销售项目一起使用与多销售的套价格。"
            Case 160985
                Return "没连接的销售项目"
            Case 160987
                Return "创造销售项目并且与现有的食谱连接它。"
            Case 160988
                Return "销售项目用于卖，并且它与食谱通常连接。"
            Case 161028
                Return "是否是改变营养数据库？ 这次行动将改变您在您的商品已经设置了的营养定义。"
            Case 161029
                Return "必须选择出产量或成份复选框。"
            Case 161049
                Return "主题词和它的次级主题词的力量删除"
            Case 161050
                Return "被删除的主题词也从商品或食谱或者菜单项目将是未派职务。"
            Case 161051
                Return "选择的主题词和所有它的次级主题词成功地被删除。 被删除的主题词从商品、食谱和菜单项目也现在是未派职务。"
            Case 161078
                Return "确切"
            Case 161079
                Return "开始与"
            Case 161080
                Return "包含"
            Case 161082
                Return "其次"
            Case 161083
                Return "第三"
            Case 161084
                Return "第四"
            Case 161085
                Return "仅一次"
            Case 161086
                Return "每日"
            Case 161087
                Return "每周"
            Case 161088
                Return "月度"
            Case 161089
                Return "当文件改变"
            Case 161090
                Return "当计算机起动"
            Case 161091
                Return "进入%s信息"
            Case 161092
                Return "供应商小组"
            Case 161093
                Return "开单信息"
            Case 161094
                Return "起始日期"
            Case 161095
                Return "月"
            Case 161096
                Return "POS进口-不合格的数据"
            Case 161097
                Return "组织并且维护您的供应商的信息包括公司联络、地址、付款期限等等缓和命令的过程。"
            Case 161098
                Return "终端提到与您的CALCMENU网连接您的POS的驻地。 增加，修改或者删除终端在这个节目。"
            Case 161099
                Return "配置POS进口参量。 设置进口文件的日程表、地点等等。"
            Case 161100
                Return "产品和股票项目被保留并且被散布在不同的地点在不同的时期。 维护控制在建立可能的地点，您的产品可以在任何指定的时刻被发现。"
            Case 161101
                Return "客户是购买您的产品或制成品的公司。 处理您的客户名单在这个节目。"
            Case 161102
                Return "客户联络是您应付公司的人。 创造，修改，并且删除客户接触。"
            Case 161103
                Return "固定在系统没有成功地被进口的POS数据。"
            Case 161104
                Return "这提到发行交易的种类从供应。 这可能或不可能实际上被卖了对顾客例如雇员福利或泄漏。"
            Case 161105
                Return "销售历史迅速显示销售交易和销售项目lsit介入的"
            Case 161106
                Return "标记的项目"
            Case 161107
                Return "计算的产量"
            Case 161132
                Return "查看我的食谱"
            Case 159274
                Return "仅%number"
            Case 161147
                Return "食谱和菜单管理"
            Case 161162
                Return "TCPOS输出"
            Case 155761
                Return "导入商品"
            Case 161180
                Return "确定自动上传配置"
            Case 161181
                Return "主机名"
            Case 11060
                Return "目录"
            Case 24068
                Return "利润"
            Case 158734
                Return "该数据库的版本与这个程序的版本不兼容。"
            Case 161275
                Return "每日金额指南"
            Case 161276
                Return "每日金额指南（GDA）"
            Case 7250
                Return "法国"
            Case 7280
                Return "意大利"
            Case 7260
                Return "德国"
            Case 157515
                Return "荷兰语"
            Case 158868
                Return "中文"
            Case 161279
                Return "没有"
            Case 54295
                Return "同"
            Case 159468
                Return "用作配料"
            Case 159469
                Return "不被用作配料"
            Case 134159
                Return "全部"
            Case 144582
                Return "无分组"
            Case 161281
                Return "高级厨师"
            Case 161282
                Return "物业管理"
            Case 161283
                Return "系统管理"
            Case 161284
                Return "公司厨师"
            Case 161285
                Return "物业厨师"
            Case 161286
                Return "厨师"
            Case 161287
                Return "客人"
            Case 161288
                Return "网站厨师"
            Case 161289
                Return "网站管理"
            Case 161290
                Return "查看和打印"
            Case 161291
                Return "未定义"
            Case 161292
                Return "确定的"
            Case 161294
                Return "不需要的项目"
            Case 24269
                Return "选择所有"
            Case 24268
                Return "取消所有选定"
            Case 160880
                Return "重新计算"
            Case 160894
                Return "Silver"
            Case 14110
                Return "Footer"
            Case 161300
                Return "购买套价格"
            Case 160776
                Return "回到%s"
            Case 132617
                Return "所有类别"
            Case 155842
                Return "人"
            Case 155050
                Return "所有关键字"
            Case 135024
                Return "地区"
            Case 161333
                Return "标题"
            Case 161334
                Return "%z的食谱 %x-%y"
            Case 104836
                Return "修改一个产品"
            Case 51281
                Return "*的配料"
            Case 158349
                Return "指派的关键字"
            Case 158350
                Return "衍生的关键字"
            Case 119130
                Return "搜索"
            Case 155927
                Return "所有来源"
            Case 161484
                Return "温度"
            Case 161485
                Return "生产<br />日期"
            Case 161486
                Return "消费<br />日期"
            Case 31700
                Return "天"
            Case 7030
                Return "打印机"
            Case 161487
                Return "每日产品"
            Case 161488
                Return "消费前"
            Case 161489
                Return "Fresh enjoy freshly-prepared"
            Case 161490
                Return "过敏信息;包括："
            Case 161491
                Return "分配到所有被标记的"
            Case 4825
                Return "食谱"
            Case 21550
                Return "没有找到菜"
            Case 24011
                Return "的"
            Case 161494
                Return "在最高5℃"
            Case 161538
                Return "请提供下面请求的信息。"
            Case 161554
                Return "请提供下面请求的信息。"
            Case 161576
                Return "单价"
            Case 133328
                Return "食谱名称"
            Case 51128
                Return "食谱名称"
            Case 161577
                Return "时间"
            Case 161578
                Return "共有商品成本"
            Case 161579
                Return "计算"
            Case 161580
                Return "商品成本"
            Case 161581
                Return "税"
            Case 161582
                Return "毛利按法郎"
            Case 161583
                Return "毛利率百分比"
            Case 159733
                Return "文章编号"
            Case 161584
                Return "单位"
            Case 143003
                Return "净¶数量"
            Case 155811
                Return "总¶数量"
            Case 161585
                Return "价格/ ¶单位"
            Case 132708
                Return "没有供应商"
            Case 24075
                Return "货号"
            Case 27056
                Return "和"
            Case 161766
                Return "小份额"
            Case 161767
                Return "大份额"
            Case 156892
                Return "下载："
            Case 161777
                Return "取消指定关键字"
            Case 161778
                Return "分配/取消分配关键字"
            Case 161779
                Return "面包屑式"
            Case 161780
                Return "监视器面包屑式"
            Case 161781
                Return "Unwanted Keyword"
            Case 161782
                Return "打印标签"
            Case 161783
                Return "程序模板"
            Case 161784
                Return "学生"
            Case 161785
                Return "每%s的成分营养价值"
            Case 161786
                Return "每100克/毫升的成分营养价值"
            Case 155926
                Return "导出到Excel"
            Case 161787
                Return "应用模板"
            Case 135969
                Return "您确定要更换%o吗？"
            Case 132934
                Return "最后的食谱"
            Case 132937
                Return "最后的菜单"
            Case 161788
                Return "分配的/派生关键词"
            Case 161468
                Return "验证所有"
            Case 161823
                Return "添加行"
            Case 161824
                Return "从剪贴板粘贴"
            Case 161825
                Return "没有任何需要被链接的商品。"
            Case 161826
                Return "选择另一个"
            Case 8514
                Return "新价格"
            Case 161827
                Return "默认价格/单位："
            Case 161828
                Return "从现有的单位中选择"
            Case 161829
                Return "把这个增加为一个新单位"
            Case 161831
                Return "让我在添加之前编辑商品"
            Case 161832
                Return "把%s放在补位"
            Case 161834
                Return "请检查价格"
            Case 161835
                Return "切"
            Case 159594
                Return "＆添加到食谱"
            Case 161837
                Return "添加到食谱"
            Case 10447
                Return "命令"
            Case 161838
                Return "替换现有的成分"
            Case 161839
                Return "没有找到成分"
            Case 132672
                Return "您确定要删除%n 吗？"
            Case 161840
                Return ""
            Case 161841
                Return "链接到商品或子食谱"
            Case 161842
                Return "现在所有项目都被链接到了商品/子食谱"
            Case 161843
                Return "项目现正被链接到商品/分食谱"
            Case 161844
                Return "Storing Time"
            Case 161845
                Return "存储温度"
            Case 161851
                Return "可以被订购"
            Case 161852
                Return "食谱可能含有过敏原"
            Case 159088
                Return "发送申请核准"
            Case 161855
                Return "??"
            Case 161986
                Return "添加步骤"
            Case 161853
                Return "粘贴"
            Case 161987
                Return "%p的项目%n"
            Case 161988
                Return "链接的产品"
            Case 161989
                Return "没有链接的产品"
            Case 158851
                Return "创建人："
            Case 161830
                Return "认可的项目"
            Case 162198
                Return "该产量已被更改。点击计算按钮来调整原料数量。"
            Case 162199
                Return "该产量已被更改。您希望在没有计算原料数量的情况下继续保存吗？"
            Case 162203
                Return "信息"
            Case 162205
                Return "出价数"
            Case 162208
                Return "每周工作日"
            Case 151500
                Return "提案"
            Case 162211
                Return "选择语言"
            Case 162212
                Return "公司名称"
            Case 162213
                Return "商业号码"
            Case 162214
                Return "有效价格"
            Case 162215
                Return "链接到服务器的负载"
            Case 146043
                Return "一月"
            Case 146044
                Return "二月"
            Case 146045
                Return "三月"
            Case 146046
                Return "四月"
            Case 146047
                Return "五月"
            Case 146048
                Return "六月"
            Case 146049
                Return "七月"
            Case 146050
                Return "八月"
            Case 146051
                Return "九月"
            Case 146052
                Return "十月"
            Case 146053
                Return "十一月"
            Case 146054
                Return "十二月"
            Case 162216
                Return "偏好"
            Case 162219
                Return "后台办公室"
            Case 162221
                Return "常规配置"
            Case 162222
                Return "在这里插入"
            Case 8990
                Return "或者"
            Case 162230
                Return "输入文体资料"
            Case 162231
                Return "名称的文体"
            Case 162232
                Return "标题文体选项"
            Case 160237
                Return "黑体"
            Case 134826
                Return "关闭"
            Case 162235
                Return "您的意思是"
            Case 159700
                Return "＆导入食谱"
            Case 162276
                Return "导入食谱"
            Case 162282
                Return "笔记"
            Case 159681
                Return "食谱（％s）中有太多的成分。 （最高是％n）"
            Case 135257
                Return "毛利率"
            Case 31732
                Return "菜单计划"
            Case 162340
                Return "街"
            Case 162341
                Return "地方"
            Case 162357
                Return "例子"
            Case 162358
                Return "保持前缀的长度"
            Case 162361
                Return "标签"
            Case 162362
                Return "分割线"
            Case 162363
                Return "分号"
            Case 162364
                Return "空间"
            Case 133590
                Return "&粘贴"
            Case 155260
                Return "强加因数"
            Case 156060
                Return "强加的食物成本"
            Case 156061
                Return "强加的利润"
            Case 162383
                Return "认同"
            Case 162382
                Return "批准"
            Case 162386
                Return "开始"
            Case 162387
                Return "嗨，批准人您已经收到了一个要批准的食谱。[该项目的创作者姓名]已经提交了这个食谱：[...]请登录到该CALCMENU网站，审查和批准该食谱。致意，EGS 团队"
            Case 162388
                Return "嗨，您新创建的食谱已送交审批。该食谱将首先被审查和批准，然后才可以在网上使用。您所提交的这个食谱：[...]一旦获得批准，该食谱将可在网上被使用。致意，EGS 团队"
            Case 162389
                Return "嗨，批准人您已批准了这一食谱：[...]该食谱将被提供在网上。致意，EGS 团队"
            Case 162390
                Return "嗨，食谱[...]已得到了批准。现在，您可以在网上使用这个食谱。致意，EGS 团队"
            Case 162530
                Return "登录后移走面包屑式"
            Case 28483
                Return "该记录不存在"
            Case 162955
                Return "在%中的净利润"
            Case 132900
                Return "添加价格"
            Case 163032
                Return "复制价格表"
            Case 155995
                Return "检查中……"
            Case 156784
                Return "总计错误：  %n"
            Case 51174
                Return "导入完成"
            Case 133334
                Return "正在输入%r"
            Case 163046
                Return "对不起，关键词%k%n%u没有找到。 请按""浏览关键词"" ，选择可用的关键词。"
            Case 135283
                Return "最后价格"
            Case 156542
                Return "称重的平均价格"
            Case 147381
                Return "用于先前产品的存货价格"
            Case 157281
                Return "默认供应商的价格"
            Case 163057
                Return "%s的总成本"
            Case 163058
                Return "1个%s的成本"
            Case 132553
                Return "统制售价+税"
            Case 138031
                Return "所有产品的库存"
            Case 138032
                Return "从标记类别的产品"
            Case 138033
                Return "从标记储藏地的产品"
            Case 138034
                Return "从标记供应商的产品"
            Case 138035
                Return "从一个或多个先前库存清单中的产品"
            Case 138030
                Return "为这个库存清单，选择您想要的产品。"
            Case 163060
                Return "%s中的食物成本"
            Case 163061
                Return "在%s中强加食品成本"
            Case 167719
                Return "Budget"
            Case 158410
                Return "如果一些产品还没有定义的价格（价格= 0 ） ，使用默认供应商的价格来代替。"
            Case 136230
                Return "创建一个新的库存清单"
            Case 136231
                Return "修改库存清单信息"
            Case 3205
                Return "名字"
            Case 135235
                Return "存货价值"
            Case 135100
                Return "参考编号"
            Case 135110
                Return "库存清单¶数量"
            Case 160414
                Return "¶库存清单以前数量 "
            Case 136100
                Return "目前打开的库存清单"
            Case 136115
                Return "项目＃"
            Case 136110
                Return "在*下打开的"
            Case 1146
                Return "运行中"
            Case 134021
                Return "库存开始于"
            Case 124164
                Return "库存调整"
            Case 158946
                Return "设置现有数量为库存清单量"
            Case 136213
                Return "在现有库存清单中添加一个产品"
            Case 136214
                Return "从该库存清单中删除一个产品"
            Case 136212
                Return "查看需要调整的名单"
            Case 136215
                Return "为产品添加一个新的储藏地"
            Case 136217
                Return "删除选定的产品储藏地的数量"
            Case 155861
                Return "为所选项目重新把数量设置为零"
            Case 136216
                Return "删除该产品的选定储藏地"
            Case 157336
                Return "不适用"
            Case 136030
                Return "目录"
            Case 133147
                Return "升"
            Case 136432
                Return "无效编码"
            Case 143981
                Return "无效的帐户代码"
            Case 169310
                Return "品尝/发展"
            Case 169318
                Return "反馈"
            Case 110447
                Return "命令"
            Case 158216
                Return "随时随地的集中配方管理。"
            Case 168373
                Return "使用的在线"
            Case 168374
                Return "Reference No1"
            Case 168375
                Return "Reference No2"
            Case 157060
                Return "参考号码"
            Case 157659
                Return "锁定"
            Case 157660
                Return "解锁"
            Case 170155
                Return "Assign ingredient, recipes and menus to Categories, Keywords and Sources (could be a cookbook, Website, chef, etc.). This allows you to group and organize items in EGS CALCMENU Web. Searching for ingredient, recipes or menus can be made faster and easier since Categories, Keywords, and Sources are very useful in narrowing down search results."
            Case 160232
                Return "导出到"
            Case 170770
                Return "Yield to Print"
            Case 133248
                Return "配料"
            Case 170779
                Return "Ingredient List"
            Case 170780
                Return "Ingredient Details"
            Case 170781
                Return "Ingredient Nutrient List"
            Case 170782
                Return " Ingredient Category"
            Case 170783
                Return "Ingredient Keyword"
            Case 170784
                Return "Ingredient Published On The Web"
            Case 170785
                Return "Ingredient Not Published On The Web"
            Case 170786
                Return "Ingredient Cost"
            Case 170849
                Return "Abbreviated Preparation Method"
            Case 171301
                Return "Preparation Method"
            Case 171302
                Return "Tips"
            Case 170850
                Return "Cook Mode only"
            Case 133115
                Return "所有食谱"
            Case 170851
                Return "None Cook Mode only"
            Case 170852
                Return "Show Off"
            Case 170853
                Return "Quick & Easy"
            Case 170854
                Return "Chef Recommended"
            Case 170855
                Return "Moderate"
            Case 170856
                Return "Challenging"
            Case 170857
                Return "Gold"
            Case 170858
                Return "Unrated"
            Case 170859
                Return "Bronze"
            Case 170860
                Return "Move marked to new standard"
            Case 171219
                Return "LeadIn"
            Case 55011
                Return "Serving Size"
            Case 171220
                Return "Servings per Yield" '"Number of Servings"
            Case 171221
                Return "Total Yield/Servings" '"Total Yield"
            Case 151436
                Return "Attachment"
            Case 150009
                Return "Exportation Done. BrandSite Successfully Exported."
            Case 171597
                Return "Recipe has been checked in by another user and cannot be modified."
            Case 27220
                Return "Hour"

            Case 171650
                Return "Prep Time"
            Case 171651
                Return "Cook Time "
            Case 171652
                Return "Marinate Time "
            Case 171653
                Return "Stand Time "
            Case 171654
                Return "Chill Time "
            Case 171655
                Return "Brew Time "
            Case 171656
                Return "Freeze Time "
            Case 171657
                Return "ReadyIn"
            Case 171658
                Return "second"
            Case 171616
                Return "Placement"

        End Select
    End Function
 
'japanese
    Public Function FTBLow7USA(ByVal Code As Integer) As String
        Select Case Code
            Case 1080
                Return "商品化費用"
            Case 1081
                Return "商品原価"
            Case 1090
                Return "販売価格"
            Case 1145
                Return "カウンター"
            Case 1260
                Return "原材料製品"
            Case 1280
                Return "リマーク"
            Case 1290
                Return "価格"
            Case 1300
                Return "消耗"
            Case 1310
                Return "数量"
            Case 1400
                Return "メニュー"
            Case 1450
                Return "カテゴリー"
            Case 1480
                Return "公定価格"
            Case 1485
                Return "計算価格"
            Case 1500
                Return "日付"
            Case 1530
                Return "見当たらないユニット"
            Case 1600
                Return "メニュー修正"
            Case 2430
                Return "&リストから選択"
            Case 2700
                Return "メニューリストを印刷してください"
            Case 2780
                Return "購入リスト"
            Case 3057
                Return "データベース"
            Case 3140
                Return "のための"
            Case 3150
                Return "パーセンテージ"
            Case 3161
                Return "定数"
            Case 3195
                Return "レシピ#"
            Case 3200
                Return "シェフ"
            Case 3204
                Return "名"
            Case 3206
                Return "翻訳"
            Case 3215
                Return "ユニット価格"
            Case 3230
                Return "画像"
            Case 3234
                Return "リスト"
            Case 3300
                Return "メニューカード"
            Case 3305
                Return "参照名"
            Case 3306
                Return "代表"
            Case 3320
                Return "サービング数を変更しますか？"
            Case 3460
                Return "&パスワード"
            Case 3680
                Return "バックアップ"
            Case 3685
                Return "バックアップ完成"
            Case 3721
                Return "ソース"
            Case 3760
                Return "インポート"
            Case 3800
                Return "エクスポート"
            Case 4130
                Return "ディスク上のフリースペース"
            Case 4185
                Return "プロダクト-ID"
            Case 4755
                Return "スタートインポート"
            Case 4832
                Return "レシピ"
            Case 4834
                Return "レシピ材料"
            Case 4854
                Return "最少"
            Case 4855
                Return "最大"
            Case 4856
                Return "フォーム"
            Case 4860
                Return "外部ファイルからインポート"
            Case 4862
                Return "バージョン"
            Case 4865
                Return "ユーザー"
            Case 4867
                Return "修正"
            Case 4870
                Return "ユーザー修正"
            Case 4877
                Return "平均"
            Case 4890
                Return "ファイルタイプ"
            Case 4891
                Return "プレビュー"
            Case 5100
                Return "単位"
            Case 5105
                Return "フォーマット"
            Case 5270
                Return "商品リスト"
            Case 5350
                Return "合計"
            Case 5390
                Return "人前"
            Case 5500
                Return "ナンバー"
            Case 5530
                Return "課せられた販売価格"
            Case 5590
                Return "原材料"
            Case 5600
                Return "準備"
            Case 5610
                Return "ページ"
            Case 5720
                Return "金額"
            Case 5741
                Return "総額"
            Case 5795
                Return "サービングあたり"
            Case 5801
                Return "利益"
            Case 5900
                Return "商品カテゴリー"
            Case 6000
                Return "カテゴリー修正"
            Case 6002
                Return "カテゴリー名"
            Case 6055
                Return "テキスト追加"
            Case 6390
                Return "通貨"
            Case 6416
                Return "要因"
            Case 6470
                Return "お待ちください"
            Case 7010
                Return "いいえ"
            Case 7073
                Return "閲覧"
            Case 7181
                Return "すべて"
            Case 7183
                Return "マークされました"
            Case 7270
                Return "英語"
            Case 7296
                Return "ヨーロッパ"
            Case 7335
                Return "すべてのマークの削除が成功しました"
            Case 7570
                Return "日曜日"
            Case 7571
                Return "月曜日"
            Case 7572
                Return "火曜日"
            Case 7573
                Return "水曜日"
            Case 7574
                Return "木曜日"
            Case 7575
                Return "金曜日"
            Case 7576
                Return "土曜日"
            Case 7720
                Return "パッケージング"
            Case 7725
                Return "輸送"
            Case 7755
                Return "システム"
            Case 8210
                Return "計算"
            Case 8220
                Return "手順"
            Case 8395
                Return "追加"
            Case 8397
                Return "削除"
            Case 8913
                Return "なし"
            Case 8914
                Return "少数"
            Case 8994
                Return "ツール"
            Case 9030
                Return "アップデート"
            Case 9070
                Return "デモバージョンでは許可されません"
            Case 9140
                Return "スイス"
            Case 9920
                Return "説明"
            Case 10103
                Return "コピー"
            Case 10104
                Return "テキスト"
            Case 10109
                Return "オプション"
            Case 10116
                Return "ノート"
            Case 10121
                Return "サーチ"
            Case 10125
                Return "ノート"
            Case 10129
                Return "選択"
            Case 10130
                Return "手元に"
            Case 10131
                Return "入力"
            Case 10132
                Return "出力"
            Case 10135
                Return "スタイル"
            Case 10140
                Return "ストック"
            Case 10363
                Return "税"
            Case 10369
                Return "供給元番号"
            Case 10370
                Return "オーダーで"
            Case 10399
                Return "削除されました"
            Case 10417
                Return "失敗しました："
            Case 10430
                Return "位置"
            Case 10431
                Return "在庫"
            Case 10468
                Return "ステータス"
            Case 10513
                Return "割引"
            Case 10523
                Return "電話番号"
            Case 10524
                Return "ファックス"
            Case 10554
                Return "HACCP"
            Case 10555
                Return "冷却時間"
            Case 10556
                Return "加熱時間"
            Case 10557
                Return "加熱度/温度"
            Case 10558
                Return "加熱モード"
            Case 10572
                Return "栄養"
            Case 10573
                Return "情報1"
            Case 10970
                Return "印刷"
            Case 10990
                Return "供給元"
            Case 11040
                Return "復元が完成されました"
            Case 11280
                Return "登録"
            Case 12515
                Return "バーコード"
            Case 12525
                Return "無効な日付"
            Case 13060
                Return "栄養価"
            Case 13255
                Return "履歴"
            Case 14070
                Return "フォント"
            Case 14090
                Return "タイトル"
            Case 14816
                Return "と取り替え"
            Case 14819
                Return "取り替え"
            Case 14884
                Return "更新項目"
            Case 15360
                Return "マークメニュー"
            Case 15504
                Return "管理者"
            Case 15510
                Return "パスワード"
            Case 15615
                Return "パスワードを入力"
            Case 15620
                Return "確認"
            Case 16010
                Return "計算"
            Case 18460
                Return "進行過程中での保存"
            Case 20122
                Return "会社"
            Case 20200
                Return "サブレシピ"
            Case 20469
                Return "郵送方法を指定してください"
            Case 20530
                Return "エネルギー"
            Case 20703
                Return "メイン"
            Case 20709
                Return "ユニット"
            Case 21570
                Return "FAXフォームを印刷してください"
            Case 21600
                Return "の"
            Case 24002
                Return "ラストオーダー"
            Case 24016
                Return "仕入先"
            Case 24027
                Return "計算"
            Case 24028
                Return "キャンセル"
            Case 24044
                Return "両方"
            Case 24050
                Return "新規"
            Case 24085
                Return "新しい指定"
            Case 24105
                Return "ディスプレイ"
            Case 24121
                Return "省略形"
            Case 24129
                Return "転送"
            Case 24150
                Return "編集"
            Case 24152
                Return "ポジション"
            Case 24153
                Return "都市"
            Case 24163
                Return "デフォルト位置"
            Case 24260
                Return "この供給元を削除できません"
            Case 24270
                Return "戻る"
            Case 24271
                Return "進む"
            Case 24291
                Return "小計"
            Case 26000
                Return "続行"
            Case 26100
                Return "製品説明"
            Case 26101
                Return "料理の助言/アドバイス"
            Case 26102
                Return "洗練"
            Case 26103
                Return "保管"
            Case 26104
                Return "出来高/生産性"
            Case 27000
                Return "参照名"
            Case 27020
                Return "アドレス"
            Case 27050
                Return "電話番号"
            Case 27055
                Return "ヘッダー名"
            Case 27130
                Return "支払"
            Case 27135
                Return "有効期限"
            Case 28000
                Return "オペレーションエラー"
            Case 28008
                Return "無効なディレクトリー"
            Case 28655
                Return "ユニットが定義されていません"
            Case 29170
                Return "利用不可"
            Case 29771
                Return "商品を修正"
            Case 30210
                Return "オペレーションは失敗されました"
            Case 30270
                Return "見つかりません"
            Case 31085
                Return "更新は成功されました"
            Case 31098
                Return "保存"
            Case 31370
                Return "食費"
            Case 31375
                Return "FC"
            Case 31380
                Return "メイン"
            Case 31462
                Return "エラー"
            Case 31492
                Return "ファックスアシスタントサービスは24時間以内に、あなたの問題に返答いたします（週末は除く）"
            Case 31755
                Return "結果"
            Case 31758
                Return "に"
            Case 31769
                Return "販売済み"
            Case 31800
                Return "日"
            Case 31860
                Return "期間"
            Case 51056
                Return "プロダクト"
            Case 51086
                Return "言語"
            Case 51092
                Return "単位"
            Case 51097
                Return "EGS Enggist && Grandjean Software SA"
            Case 51098
                Return "Route de Soleure 12 / PO BOX"
            Case 51099
                Return "2072 St-Blaise, Switzerland"
            Case 51123
                Return "詳細"
            Case 51129
                Return "必要な材料"
            Case 51130
                Return "不要な材料"
            Case 51139
                Return "必要な"
            Case 51157
                Return "メッセージ"
            Case 51178
                Return "もう一度試してください。"
            Case 51198
                Return "SMTPサーバーに接続"
            Case 51204
                Return "はい"
            Case 51243
                Return "余白"
            Case 51244
                Return "上"
            Case 51245
                Return "下"
            Case 51246
                Return "左"
            Case 51247
                Return "右"
            Case 51252
                Return "ダウンロード"
            Case 51257
                Return "電子メール"
            Case 51259
                Return "ＳＭＰＴサーバ-"
            Case 51261
                Return "ユーザー名"
            Case 51294
                Return "出来高"
            Case 51311
                Return "無効なユニット"
            Case 51336
                Return "不要項目"
            Case 51353
                Return "著作権契約"
            Case 51364
                Return "上記の著作権契約に合意し、レシピの提示を続けますか？"
            Case 51377
                Return "電子メールを送信する"
            Case 51392
                Return "出来高ユニット"
            Case 51402
                Return "本当に削除しますか？"
            Case 51500
                Return "ショッピングリスト詳細"
            Case 51502
                Return "ショッピングリスト"
            Case 51532
                Return "ショッピングリストをプリント"
            Case 51907
                Return "詳細を表示する"
            Case 52012
                Return "閲覧"
            Case 52110
                Return "選択されたファイルがインポートされます"
            Case 52130
                Return "新しいレシピ"
            Case 52150
                Return "完了"
            Case 52307
                Return "終了"
            Case 52960
                Return "Simple"
            Case 52970
                Return "完成"
            Case 53250
                Return "エクスポート選択"
            Case 54210
                Return "何も変更しない"
            Case 54220
                Return "全て大文字"
            Case 54230
                Return "全て小文字"
            Case 54240
                Return "各単語の頭文字を大文字にしてください"
            Case 54245
                Return "頭文字の大文字化"
            Case 54710
                Return "選択されたキーワード"
            Case 54730
                Return "キーワード"
            Case 55211
                Return "リンク"
            Case 55220
                Return "量"
            Case 56100
                Return "あなたの氏名"
            Case 56130
                Return "国"
            Case 56500
                Return "辞書"
            Case 101600
                Return "修正メニュー"
            Case 103150
                Return "パーセンテージ"
            Case 103215
                Return "ユニット価格"
            Case 103305
                Return "参照名"
            Case 103306
                Return "代表"
            Case 104829
                Return "供給元リスト"
            Case 104835
                Return "新しいプロダクトを作成"
            Case 104854
                Return "最少"
            Case 104855
                Return "最大"
            Case 104862
                Return "バージョン"
            Case 104869
                Return "新しいユーザー"
            Case 104870
                Return "ユーザーを修正"
            Case 105100
                Return "単位"
            Case 105110
                Return "日付"
            Case 105200
                Return "仕上数"
            Case 105360
                Return "サービングによる販売価格"
            Case 106002
                Return "カテゴリー名"
            Case 107183
                Return "マークされました"
            Case 110101
                Return "修正"
            Case 110102
                Return "削除"
            Case 110112
                Return "印刷"
            Case 110114
                Return "ヘルプ"
            Case 110129
                Return "選択"
            Case 110417
                Return "失敗しました:"
            Case 110524
                Return "ファックス"
            Case 113275
                Return "税"
            Case 115610
                Return "新しいパスワードが受理されました"
            Case 121600
                Return "の"
            Case 124016
                Return "仕入先"
            Case 124024
                Return "に認可された"
            Case 124042
                Return "タイプ"
            Case 124257
                Return "アウトレット"
            Case 127010
                Return "会社"
            Case 127040
                Return "国"
            Case 127050
                Return "電話番号"
            Case 127055
                Return "ヘッダー名"
            Case 128000
                Return "オペレーションエラー"
            Case 131462
                Return "エラー"
            Case 131757
                Return "から"
            Case 132552
                Return "税合計"
            Case 132554
                Return "レシピを変更する"
            Case 132555
                Return "レシピを追加"
            Case 132557
                Return "新しいメニューを作成する"
            Case 132559
                Return "新しい商品を作成する"
            Case 132561
                Return "シリアルナンバー、ヘッダー名、プロダクトキーを入力してください。RecipeNet が提供した文書で、この情報を見つけることができます。"
            Case 132565
                Return "補足"
            Case 132567
                Return "商品カテゴリー"
            Case 132568
                Return "レシピカテゴリー"
            Case 132569
                Return "メニューカテゴリー"
            Case 132570
                Return "削除できません。"
            Case 132571
                Return "カテゴリーは現在使用されています。"
            Case 132589
                Return "レシピの最大数"
            Case 132590
                Return "レシピの現在の数"
            Case 132592
                Return "商品の最大数"
            Case 132593
                Return "商品の現在数"
            Case 132597
                Return "新しいレシピを作成"
            Case 132598
                Return "メニューの最大数"
            Case 132599
                Return "メニューの現在の数"
            Case 132600
                Return "キーワードを指定する"
            Case 132601
                Return "マークされたものを新しいカテゴリーに移動"
            Case 132602
                Return "マークされたものを削除する"
            Case 132605
                Return "ショッピングリスト"
            Case 132607
                Return "アクションマーク"
            Case 132614
                Return "正味数量"
            Case 132615
                Return "権利"
            Case 132616
                Return "オーナー"
            Case 132621
                Return "ソースを変更する"
            Case 132630
                Return "自動変換"
            Case 132638
                Return "ユーザー情報"
            Case 132640
                Return "このユーザー名はすでに使用されています。"
            Case 132654
                Return "データベース管理"
            Case 132657
                Return "復元"
            Case 132667
                Return "マージする"
            Case 132668
                Return "パージ"
            Case 132669
                Return "上に移動"
            Case 132670
                Return "下に移動"
            Case 132671
                Return "規格化する"
            Case 132678
                Return "HACCP"
            Case 132683
                Return "戻る"
            Case 132706
                Return "栄養価は100gあるいは100ml当たりです"
            Case 132714
                Return "リストから選択してください。"
            Case 132719
                Return "既に確定された同じユニットの価格。"
            Case 132723
                Return "損失合計が100%以上になることはありません。"
            Case 132736
                Return "総量"
            Case 132737
                Return "新しい供給元を追加する"
            Case 132738
                Return "供給元を変更する"
            Case 132739
                Return "供給元の詳細"
            Case 132740
                Return "状態"
            Case 132741
                Return "URL"
            Case 132779
                Return "キーワードは使用されています。"
            Case 132783
                Return "キーワード"
            Case 132788
                Return "栄養価のリンク"
            Case 132789
                Return "ログイン"
            Case 132813
                Return "環境設定"
            Case 132828
                Return "栄養価を再計算"
            Case 132841
                Return "商品を追加"
            Case 132846
                Return "マークを保存"
            Case 132847
                Return "マークをロード"
            Case 132848
                Return "除外"
            Case 132855
                Return "メニューを追加"
            Case 132860
                Return "材料を追加"
            Case 132864
                Return "材料を変更"
            Case 132865
                Return "セパレーターを追加"
            Case 132877
                Return "アイテムを追加"
            Case 132896
                Return "カテゴリーを規格化"
            Case 132912
                Return "テキストを規格化"
            Case 132915
                Return "ユニット規格化"
            Case 132924
                Return "出来高ユニットを規格化"
            Case 132930
                Return "サムネール"
            Case 132933
                Return "レシピリスト"
            Case 132939
                Return "レシピリスト"
            Case 132954
                Return "マークのセット"
            Case 132955
                Return "リストからマーク名を選択するか、保存する新しいマーク名を入力してください。"
            Case 132957
                Return "マークしたものを保存"
            Case 132967
                Return "栄養"
            Case 132971
                Return "栄養概要"
            Case 132972
                Return "栄養価はサービング数当たり100％です。"
            Case 132974
                Return "廃棄率"
            Case 132987
                Return "概要"
            Case 132989
                Return "表示"
            Case 132997
                Return "当日もしくは以前"
            Case 132998
                Return "当日もしくは以降"
            Case 132999
                Return "の間"
            Case 133000
                Return "以上"
            Case 133001
                Return "未満"
            Case 133005
                Return "課するべき価格"
            Case 133023
                Return "ディスプレイオプション"
            Case 133043
                Return "ローカル画像変換"
            Case 133045
                Return "最大の画像ファイルサイズ"
            Case 133046
                Return "最大の画像サイズ"
            Case 133047
                Return "最適化"
            Case 133049
                Return "ウェブサイトで使用するための画像の自動変換を起動させる"
            Case 133057
                Return "ウェブサイトのためにロゴをアップロードする"
            Case 133060
                Return "ウェブカラー"
            Case 133075
                Return "新しいパスワード"
            Case 133076
                Return "新しいパスワードを確認"
            Case 133080
                Return "最後"
            Case 133081
                Return "最初"
            Case 133085
                Return "ドキュメント出力"
            Case 133096
                Return "レシピ準備"
            Case 133097
                Return "レシピ原価計算"
            Case 133099
                Return "変動"
            Case 133100
                Return "レシピ詳細"
            Case 133101
                Return "メニュー詳細"
            Case 133108
                Return "何を印刷しますか？"
            Case 133109
                Return "印刷する商品の選択"
            Case 133111
                Return "いくつかのカテゴリー"
            Case 133112
                Return "マークした商品"
            Case 133116
                Return "マークしたレシピ"
            Case 133121
                Return "マークしたメニュー"
            Case 133123
                Return "メニュー原価計算"
            Case 133124
                Return "メニュー説明"
            Case 133126
                Return "EGS標準"
            Case 133127
                Return "EGS最新"
            Case 133128
                Return "EGS2コラム"
            Case 133133
                Return "このファイル名は無効です。有効なファイル名を入力してください。"
            Case 133144
                Return "レシピ＃"
            Case 133161
                Return "用紙サイズ"
            Case 133162
                Return "余白のユニット"
            Case 133163
                Return "左余白"
            Case 133164
                Return "右余白"
            Case 133165
                Return "上余白"
            Case 133166
                Return "下余白"
            Case 133168
                Return "フォントサイズ"
            Case 133172
                Return "小画像/ 量 - 名前"
            Case 133173
                Return "小画像/ 名前 - 量"
            Case 133174
                Return "中画像/ 量 - 名前"
            Case 133175
                Return "中画像 / 名前 - 量"
            Case 133176
                Return "大画像/ 量 - 名前"
            Case 133177
                Return "大画像/ 名前 - 量"
            Case 133196
                Return "リストオプション"
            Case 133201
                Return "以下の商品は、使用中で、削除することができません。"
            Case 133207
                Return "レシピはサブレシピとして使用できます。"
            Case 133208
                Return "重量"
            Case 133222
                Return "詳細オプション"
            Case 133230
                Return "以下のレシピは、使用されていて削除されませんでした。"
            Case 133241
                Return "価格を再計算します。お待ちください..."
            Case 133242
                Return "栄養価を再計算します。お待ちください..."
            Case 133251
                Return "セパレーター"
            Case 133254
                Return "ソート"
            Case 133260
                Return "ソースは使用されています。"
            Case 133266
                Return "キーワードを規格化する"
            Case 133286
                Return "定義"
            Case 133289
                Return "ユニットは使用されています。"
            Case 133290
                Return "２つ以上のシステムユニットをマージできません。"
            Case 133295
                Return "このユニットを削除することができません。 ¶ユーザ定義されたユニットしか削除できません。"
            Case 133314
                Return "ユーザ定義された出来高ユニットのみ削除することができます。"
            Case 133315
                Return "二つ以上のシステム出来高ユニットをマージすることができません。"
            Case 133319
                Return "出来高ユニットは現在使用されています。"
            Case 133325
                Return "本当に使用されていないカテゴリーを全てパージしますか？"
            Case 133326
                Return "ソースがありません"
            Case 133330
                Return "見つからないファイル。"
            Case 133349
                Return "メニュー#"
            Case 133350
                Return "%y(正味数量)のためのアイテム"
            Case 133351
                Return "" '"%p%(正味数量)で%yのための材料"
            Case 133352
                Return "サービング＋税による課販売価格"
            Case 133353
                Return "サービングによる課販売価格"
            Case 133359
                Return "番号で分類されました"
            Case 133360
                Return "日付で分類されました"
            Case 133361
                Return "カテゴリーで分類されました"
            Case 133365
                Return "販売価格+税"
            Case 133367
                Return "供給元で分類されました"
            Case 133405
                Return "画像をアップロードする"
            Case 133519
                Return "カラーを選択してください:"
            Case 133692
                Return "提案価格"
            Case 134032
                Return "コンタクト"
            Case 134055
                Return "購買"
            Case 134056
                Return "販売"
            Case 134061
                Return "バージョン、モジュールライセンス"
            Case 134083
                Return "テスト"
            Case 134111
                Return "マークしたアイテムを削除できません。"
            Case 134176
                Return "商品／栄養価リスト"
            Case 134177
                Return "レシピ／栄養価リスト"
            Case 134178
                Return "メニュー／栄養価リスト"
            Case 134182
                Return "グループ"
            Case 134194
                Return "無効な数量"
            Case 134195
                Return "無効な価格"
            Case 134320
                Return "請求先住所"
            Case 134332
                Return "情報"
            Case 134333
                Return "重要な"
            Case 134525
                Return "変更を本当にキャンセルしますか？"
            Case 134571
                Return "無効な価値"
            Case 135056
                Return "栄養基準"
            Case 135058
                Return "栄養基準を追加する"
            Case 135059
                Return "栄養基準を変更する"
            Case 135070
                Return "正味"
            Case 135256
                Return "販売した量"
            Case 135608
                Return "ポート"
            Case 135948
                Return "サブレシピを含む"
            Case 135955
                Return "無効な数値"
            Case 135963
                Return "データベース"
            Case 135967
                Return "レシピの取替"
            Case 135968
                Return "メニューの取替"
            Case 135971
                Return "接続"
            Case 135978
                Return "新規"
            Case 135979
                Return "名前を変更する"
            Case 135985
                Return "既存のファイルからインポート"
            Case 135986
                Return "見つからない"
            Case 135989
                Return "アイテム"
            Case 135990
                Return "更新"
            Case 136018
                Return "所有権"
            Case 136025
                Return "データベース転換"
            Case 136171
                Return "ユニットを変更する"
            Case 136265
                Return "サブレシピ"
            Case 136601
                Return "リセットする"
            Case 136905
                Return "通貨記号"
            Case 137019
                Return "変更する"
            Case 137030
                Return "初期設定"
            Case 137070
                Return "一般設定"
            Case 138137
                Return "削除されました"
            Case 138244
                Return "販売アイテム"
            Case 138402
                Return "全て転送されました"
            Case 138412
                Return "<定義されません>"
            Case 140056
                Return "ファイル"
            Case 140100
                Return "進行過程中でのバックアップ"
            Case 140101
                Return "進行過程中での復元"
            Case 140129
                Return "バックアップを回復する間のエラー"
            Case 140130
                Return "バックアップを作成する間のエラー"
            Case 140180
                Return "バックアップファイルを保存する経路"
            Case 143001
                Return "共有する"
            Case 143002
                Return "共有しない"
            Case 143008
                Return "廃棄率"
            Case 143013
                Return "変更"
            Case 143014
                Return "ユーザー"
            Case 143508
                Return "レシピはサブレシピとして使用されています"
            Case 143509
                Return "行間隔"
            Case 143987
                Return "アイテムの種類"
            Case 143995
                Return "アクション"
            Case 144591
                Return "時間"
            Case 144682
                Return "栄養価は100%で、100gあるいは100mｌ当たりです"
            Case 144684
                Return "栄養価は100%で、1つの出来高ユニット当たりです"
            Case 144685
                Return "100%で出来高ユニット当たり"
            Case 144686
                Return "100%で%Y当たり"
            Case 144687
                Return "100%で100gあるいは100ml当たり"
            Case 144688
                Return "N/A"
            Case 144689
                Return "栄養価は100%で1つの出来高ユニット/100gあるいは100ml当たりです"
            Case 144716
                Return "履歴"
            Case 144734
                Return "販売項目リスト"
            Case 144738
                Return "%Y当たりの重量"
            Case 145006
                Return "転送"
            Case 146056
                Return "コントリビューションマージン"
            Case 146067
                Return "収支"
            Case 146080
                Return "クライアント"
            Case 146114
                Return "異なる供給元の場合は新しいページに表示します"
            Case 146211
                Return "発行タイプ"
            Case 147070
                Return "Oｋ"
            Case 147075
                Return "無効な日付"
            Case 147126
                Return "まず既存のマークを削除する"
            Case 147174
                Return "開く"
            Case 147441
                Return "この販売アイテムはリンクされています。"
            Case 147462
                Return "比率"
            Case 147520
                Return "主要"
            Case 147647
                Return "SQLサーバーが存在していないか、アクセスが拒否されました"
            Case 147652
                Return "削除"
            Case 147692
                Return "メニュー情報"
            Case 147699
                Return "上書き"
            Case 147700
                Return "総計"
            Case 147703
                Return "準備されたポーション数"
            Case 147704
                Return "残りの出来高"
            Case 147706
                Return "返却された出来高"
            Case 147707
                Return "失われた出来高"
            Case 147708
                Return "販売された出来高"
            Case 147710
                Return "特売された出来高"
            Case 147713
                Return "EGSレイアウト"
            Case 147727
                Return "コスト"
            Case 147729
                Return "評価"
            Case 147733
                Return "言語を選択してください"
            Case 147737
                Return "数量を入力し、ユニットを選択してください"
            Case 147743
                Return "アップロード"
            Case 147753
                Return "労働コスト"
            Case 147771
                Return "レート/時間"
            Case 147772
                Return "レート/分"
            Case 147773
                Return "人"
            Case 147774
                Return "時間(時間: 分)"
            Case 149501
                Return "直接入力-出力を使用する"
            Case 149513
                Return "承認"
            Case 149531
                Return "完成した商品"
            Case 149645
                Return "リンク"
            Case 149706
                Return "リンクを削除"
            Case 149766
                Return "敬称"
            Case 149774
                Return "クリア"
            Case 150333
                Return "削除に成功しました。"
            Case 150341
                Return "通貨換算"
            Case 150353
                Return "ソート"
            Case 150634
                Return "Eメールは送信されました"
            Case 150644
                Return "あなたのコンピュータからEメールを送るためにSMTPサーバーが必要です。"
            Case 150688
                Return "このアプリケーションのためのライセンスは制限切れになりました。"
            Case 150707
                Return "アカウント"
            Case 151011
                Return "スイス - 本部"
            Case 151019
                Return "商品キーワード"
            Case 151020
                Return "レシピキーワード"
            Case 151023
                Return "登録"
            Case 151250
                Return "何も変更されませんでした。"
            Case 151286
                Return "基準"
            Case 151299
                Return "必要な情報を入力してください"
            Case 151322
                Return "棚卸に含む"
            Case 151336
                Return "1セットのマークをロード"
            Case 151344
                Return "商品のためのマークを保存"
            Case 151345
                Return "料理のためのマークを保存"
            Case 151346
                Return "メニューのためのマークを保存"
            Case 151364
                Return "2つ以上のテキストを選択"
            Case 151389
                Return "テキストを取り除く"
            Case 151400
                Return "商品コスト"
            Case 151404
                Return "付加価値税"
            Case 151424
                Return "一番適切なユニットに変換する"
            Case 151427
                Return "アイテム名で分類した"
            Case 151435
                Return "主題"
            Case 151437
                Return "RecipeNet"
            Case 151438
                Return "CALCMENU"
            Case 151459
                Return "あなたのEメール"
            Case 151499
                Return "提案を取り替える"
            Case 151854
                Return "エクセル"
            Case 151906
                Return "Eメールアドレスは見つかりませんでした"
            Case 151907
                Return "正しいユーザー名とパスワードをログインしてください。"
            Case 151910
                Return "サインイン"
            Case 151911
                Return "サインアウト"
            Case 151912
                Return "パスワードをお忘れですか？"
            Case 151915
                Return "以下で要求された情報を提供してください。"
            Case 151916
                Return "アスタリスク（*）を備えたフィールドは必要です。"
            Case 151918
                Return "有効なEメールアドレスを提供してください。"
            Case 151976
                Return "デフォルトでの製造場所"
            Case 152004
                Return "ツリービュー"
            Case 152141
                Return "商品管理"
            Case 152146
                Return "Zip"
            Case 155024
                Return "画像管理"
            Case 155046
                Return "翻訳"
            Case 155052
                Return "送信する"
            Case 155118
                Return "ポケットに購入品目リストを送る"
            Case 155163
                Return "姓"
            Case 155170
                Return "ようこそ %name!"
            Case 155205
                Return "ホーム"
            Case 155225
                Return "PDF"
            Case 155236
                Return "主な言語"
            Case 155245
                Return "私たちについて"
            Case 155263
                Return "ピクセル"
            Case 155264
                Return "翻訳"
            Case 155374
                Return "会計ID"
            Case 155507
                Return "可能にする"
            Case 155575
                Return "デフォルトでの自動出力する場所設定"
            Case 155601
                Return "選択されたアイテムが存在しません"
            Case 155642
                Return "レシピ交換"
            Case 155713
                Return "%rが存在しています"
            Case 155731
                Return "CALCMENU Pro"
            Case 155763
                Return "数で比較する"
            Case 155764
                Return "名前で比較する"
            Case 155841
                Return "復元するためのファイル"
            Case 155862
                Return "当たり"
            Case 155942
                Return "保存したショッピングリストをロードする"
            Case 155967
                Return "フィールドセパレーター"
            Case 155994
                Return "有効ではない"
            Case 155996
                Return "Eメールアドレス"
            Case 156000
                Return "新しい供給元に進む"
            Case 156012
                Return "サポート"
            Case 156015
                Return "お問い合わせ"
            Case 156016
                Return "メインオフィス"
            Case 156141
                Return "データベースをバックアップ/復元"
            Case 156337
                Return "栄養をリンクする"
            Case 156344
                Return "無効な選択"
            Case 156355
                Return "アーカイブ"
            Case 156356
                Return "追加"
            Case 156405
                Return "いくらかの間をおいてから、再試行をクリックしてください"
            Case 156413
                Return "サブレシピ定義"
            Case 156485
                Return "インポートの後ファイルを削除する"
            Case 156552
                Return "今すぐバックアップ"
            Case 156590
                Return "CSVファイル（エクセル）から商品をインポートする"
            Case 156669
                Return "ウェブサイト"
            Case 156672
                Return "ウェッブに公開"
            Case 156683
                Return "原物の"
            Case 156720
                Return "数が長すぎます"
            Case 156721
                Return "名前が長すぎます"
            Case 156722
                Return "供給元が長すぎます"
            Case 156723
                Return "カテゴリーが長すぎます"
            Case 156725
                Return "説明が長すぎます"
            Case 156734
                Return "二つのユニットは同一です"
            Case 156742
                Return "の後無効になります"
            Case 156751
                Return "Tel:  +41 848 000 357<br>（英語、フランス語、ドイツ語、営業時間：8:30am-6pm GMT +01：00）<br><br>Tel：+41 32 544 00 17<br>（英語のみ、営業時間：3am-830am GMT +01：00）"
            Case 156752
                Return "フリーダイヤル: 1-800-964-9357<br>(英語のみ、営業時間: 9am-3am 太平洋標準時)"
            Case 156753
                Return "Tel: +63 2 687 3179<br>（英語のみ、営業時間: 12am-6pm GMT +08:00)   "
            Case 156754
                Return "外部ファイルからインポート"
            Case 156825
                Return "千"
            Case 156870
                Return "本当に実行しますか？"
            Case 156925
                Return "ダウンロードできました！"
            Case 156938
                Return "有効"
            Case 156941
                Return "ポケット キッチン"
            Case 156955
                Return "プライベート"
            Case 156957
                Return "ホテル"
            Case 156959
                Return "共有されました"
            Case 156960
                Return "提出されました"
            Case 156961
                Return "価格設定"
            Case 156962
                Return "提出されてません"
            Case 156963
                Return "価格"
            Case 156964
                Return "中で検索"
            Case 156965
                Return "出来高"
            Case 156966
                Return "影響を受ける記録"
            Case 156967
                Return "正しい日付を入力してください。"
            Case 156968
                Return "無効なイメージファイルフォーマット"
            Case 156969
                Return "アップロードにイメージ・ファイルを入力してください。 さもなければ、それを空白の状態でおいてください。"
            Case 156970
                Return "カテゴリー情報を入力してください"
            Case 156971
                Return "設定価格情報を入力してください"
            Case 156972
                Return "キーワード情報を入力してください"
            Case 156973
                Return "ユニット情報を入力してください"
            Case 156974
                Return "出来高情報を入力してください"
            Case 156975
                Return "新規にレシピを作成し、他のホテルで使用するために本部オフィスに送る"
            Case 156976
                Return "商品はレシピとメニューを構成する要素を意味します"
            Case 156977
                Return "このソフトウェアに関する問い合せ及び技術的なご質問は、遠慮なく申しつけください。"
            Case 156978
                Return "基本キーワード"
            Case 156979
                Return "キーワード名"
            Case 156980
                Return "構成"
            Case 156981
                Return "税率"
            Case 156982
                Return "検索結果"
            Case 156983
                Return "申し訳ございませんが、結果は見つかりませんでした。"
            Case 156984
                Return "無効なユーザー名、または、パスワード。"
            Case 156986
                Return "項目は既に存在しています。"
            Case 156987
                Return "保存に成功しました。"
            Case 156996
                Return "Copyright © 2004 of EGS Enggist & Grandjean Software SA, Switzerland."
            Case 157002
                Return "ユニットの価格が確定しません。 ユニットを選択してください。"
            Case 157020
                Return "使われる税"
            Case 157026
                Return "中判"
            Case 157033
                Return "システムは、すべての商品の価格を更新します。お待ちください…"
            Case 157034
                Return "認証"
            Case 157038
                Return "月"
            Case 157039
                Return "年"
            Case 157040
                Return "利用可能なキーワードはありません。"
            Case 157041
                Return "アクセス拒否"
            Case 157049
                Return "あなたは本当に保存を削除したいですか"
            Case 157055
                Return "スチューデントバージョン"
            Case 157056
                Return "あなたは、キャンセルしたいですか？"
            Case 157057
                Return "マークされたアイテムは、現在、共有されています。"
            Case 157076
                Return "ヘルプ概要"
            Case 157079
                Return "以下のマークされた項目は提出されておらず、転送することができません："
            Case 157084
                Return "以下のマークされた項目が使われていて、削除されません："
            Case 157125
                Return "一覧"
            Case 157130
                Return "あなたのクレジットカード情報は、送られました。あなたの申し込みは、3日以内に処理されます。ありがとうございます。"
            Case 157132
                Return "パーソナル（共有する）"
            Case 157133
                Return "パーソナル（共有しない）"
            Case 157134
                Return "ビジター"
            Case 157136
                Return "クレジット"
            Case 157139
                Return "最低 !"
            Case 157140
                Return "良い !"
            Case 157141
                Return "素晴らしい !"
            Case 157142
                Return "インポートする前に使用されていない商品ユニットを削除する"
            Case 157151
                Return "他のリンク"
            Case 157152
                Return "ユーザレビュー"
            Case 157153
                Return "受取人は、これらの項目を受け入れるように促されるでしょう。"
            Case 157154
                Return "以下のアイテムは他のユーザーに所有されているため使えません。"
            Case 157155
                Return "誰かが以下のレシピを提供することを希望しています。"
            Case 157156
                Return "プロモ"
            Case 157157
                Return "ユーザーオプション"
            Case 157158
                Return "オリジナリティー"
            Case 157159
                Return "結果"
            Case 157160
                Return "困難"
            Case 157161
                Return "その日のレシピ"
            Case 157164
                Return "クレジットカード名義人氏名"
            Case 157165
                Return "クレジットカード番号"
            Case 157166
                Return "レコード制限"
            Case 157168
                Return "銀行"
            Case 157169
                Return "PayPal"
            Case 157170
                Return "オンライン注文はあなたの国では利用できません。"
            Case 157171
                Return "メンバー登録"
            Case 157172
                Return "アップグレード料金"
            Case 157173
                Return "会員料金"
            Case 157174
                Return "アップグレードパック"
            Case 157176
                Return "トータルレコードは使用されました。"
            Case 157177
                Return "私たちは、あなたの必要性に合うように、さまざまな解決策を提供します"
            Case 157178
                Return "トライアルユーザー"
            Case 157179
                Return "友達に教える"
            Case 157180
                Return "友達のＥメールアドレス"
            Case 157182
                Return "回答"
            Case 157183
                Return "サービスの期間と条件"
            Case 157214
                Return "マークしたレシピのみのショッピングリストを作成する"
            Case 157217
                Return "マークしたメニューのみのショッピングリストを作成する"
            Case 157226
                Return "マークされたレシピは、承認のために送られました。"
            Case 157233
                Return "廃棄率が100%以上になることはありません。"
            Case 157268
                Return "通貨は使用されました。"
            Case 157269
                Return "価格セットは使用されています。"
            Case 157273
                Return "これらは、提出も所有されなかったので、以下の項目を共有することができません。"
            Case 157274
                Return "為替相場"
            Case 157275
                Return "リストされたすべての項目は、1つに合併されます。使用したい項目を選んでください。他の項目は、データベースから削除されます。"
            Case 157276
                Return "マージに成功しました。"
            Case 157277
                Return "総費用"
            Case 157297
                Return "少なくとも1つの項目を選択してください。"
            Case 157299
                Return "プロフィールを編集しカスタマイズする"
            Case 157300
                Return "新しいパスワードを入力してください。パスワードは、20文字を越えることができません。 完了したら、'送信' をクリックしてください。"
            Case 157301
                Return "あなたがアップロードしたいイメージファイル(jpeg/jpg、bmpなど)を入力してください。 もしくは、それを空白の状態にしておいてください。 (注： GIFファイルはサポートされません。 よって、すべての画像が、正常で小さいjpeg形式に変換され、コピーされます。)"
            Case 157302
                Return "名称あるいは名称([*]アスタリスク付き)の一部によって材料を検索してください。素早く追加するためには、例えば、200 g Oel 高オレインのように、[正味数量] [ユニット] [材料] を入力してください。"
            Case 157303
                Return "商品価格を加えるか編集するためには、新しい価格を入力して、計量ユニットを定義してください。オリジナルのユニットに、そのユニットの比率を割り当ててください。例えば、オリジナルの価格およびユニットは1キログラム(kg)当たり11USドルです。ユニット・バッグを加えたければ、そのバッグの価格を定義するか、あるいは1つのバッグに、どれだけのキログラムがあるか限定しなければなりません（比率）。"
            Case 157304
                Return "名称、または名称の一部で、キーワードを検索してください。複数のキーワードには、コンマ[ , ] を使ってください。例えば、""牛肉,ソース,結婚式''のように検索してください。"
            Case 157305
                Return "項目を選択してください"
            Case 157306
                Return "無効なファイルタイプ"
            Case 157310
                Return "商品の詳細"
            Case 157314
                Return "商品価格を追加するときに主要な/大きいユニットを使用してください"
            Case 157320
                Return "共有"
            Case 157322
                Return "ユーザー契約"
            Case 157323
                Return "与える"
            Case 157329
                Return "端末"
            Case 157334
                Return "警告: 別のユーザがこの記録を修正したなら、あなたは変更をすべて失うかもしれません。 あなたはこのページをリフレッシュしたいですか?"
            Case 157339
                Return "ページ当たりのメッセージ"
            Case 157340
                Return "クイックブラウザー"
            Case 157341
                Return "各ページで"
            Case 157342
                Return "記録は、他のユーザーによって修正されました。進行するために、「OK」をクリックしてください。"
            Case 157343
                Return "この記録は、他のユーザーによって削除されました。"
            Case 157345
                Return "本社に送信します"
            Case 157346
                Return "共有されません"
            Case 157378
                Return "メンバー"
            Case 157379
                Return "今、申し込む"
            Case 157380
                Return "あなたの定期会員は%nで期限が切れます。"
            Case 157381
                Return "あなたの定期会員は無効になりました。"
            Case 157382
                Return "私の残りのポイントを、現在使用している私のメンバーシップに繰り越してください(残高)"
            Case 157383
                Return "あなたのデータはディスク領域制限に達しました。あなたのレシピまたは商品のいくつかを削除してください。ご利用ありがとうございます"
            Case 157384
                Return "無効なトランスアクション"
            Case 157385
                Return "ありがとうございます"
            Case 157387
                Return "お申し込みを完了するために、PayPalにリダイレクトされます。お手数ですが、どの通貨を使用したらよいかを下記のリストから選んでください。"
            Case 157388
                Return "参加するための招待"
            Case 157404
                Return "未解決のトランスアクション"
            Case 157405
                Return "お問い合わせには、E-MAILを　　　まで送ってください"
            Case 157408
                Return "メンバーとトライアルユーザだけが、このページにアクセスできます。 あなたはRecipe Gallery.comで、あなたのレシピを管理をしてみてはいかがでしょうか?　申込メニューへ行き、メンバーとして、ご予約をお願いいたします。"
            Case 157435
                Return "出力前にアウトレットへ自動転送"
            Case 157437
                Return "原料"
            Case 157446
                Return "月"
            Case 157594
                Return "承諾する"
            Case 157595
                Return "否認する"
            Case 157596
                Return "ユーザ再検討なし"
            Case 157604
                Return "Ｅメールサポート"
            Case 157607
                Return "電話でのサポート"
            Case 157608
                Return "オンラインサポート"
            Case 157616
                Return "米国"
            Case 157617
                Return "アジアとその他の国々"
            Case 157629
                Return "承認する"
            Case 157633
                Return "承認しない"
            Case 157695
                Return "会計参照"
            Case 157772
                Return "任意の"
            Case 157802
                Return "パスワード確認"
            Case 157901
                Return "存在しているものを隠す"
            Case 157926
                Return "サインアップ"
            Case 158005
                Return "ライセンス"
            Case 158019
                Return "チェック要求ステータス"
            Case 158169
                Return "支払期限を選んでください。¶¶前払い金によって:"
            Case 158170
                Return "あなたのクレジットカードの詳細について、当社に、Eメールをお送りください。 <a href='mailto:info@calcmenu.com'>info@calcmenu.com</a>.クレジットカードタイプ（ビザ、マスターカード、アメリカン エキスプレス）、クレジットカード名義人氏名、カード有効期限、クレジットカード番号（カードの後ろにある、3桁のセキュリティコード(CVC2/CVV2)を含む）をお願いします。"
            Case 158171
                Return "銀行/電子送金"
            Case 158174
                Return "<b>注:</b>当社が銀行送金の確認を受けるのは1～2週間ですので、それまでに送金後、当社に通知してください。"
            Case 158186
                Return "パスワード変更"
            Case 158220
                Return "最高250文字で新しい商品名をつくって、英数字の参照番号、税率、4つの消費割合、カテゴリー、供給元、および、その他の役に立つ情報、例えば、製品の説明、準備方法、料理方法のアドバイス、改良方法と保存方法をその中に含めてください。"
            Case 158229
                Return "画像"
            Case 158230
                Return "商品、レシピおよびメニューは、それらの名称または参照番号を使用して、検索することができます。さらに、カテゴリーやキーワードでの検索も可能です。また、商品を検索するとき、供給元、コード化された日付、または、最終修正された価格帯、および栄養価を使用できます。あなたは、レシピとメニューで、使われていない項目であれ、使われている項目であれ、検索し、活用することができます。"
            Case 158232
                Return "アクションマークは、マークされた商品やレシピやメニューでの類似している機能を行う際の近道です。 各項目で、これらを繰り返す必要はなく、商品、レシピ、メニューで、カテゴリやキーワードに割り当てるのに、アクションマークを使用できますので、削除したり、インポートしてください。そして、メール、印刷、共有、非共有を通して、他のユーザに発信してください。 このことは、マークされた項目を動作実行する際に、多くの時間と労力を節約させることができます。"
            Case 158234
                Return "栄養価データベースのリンクと計算"
            Case 158238
                Return "供給元の管理"
            Case 158240
                Return "カテゴリー、キーワード、管理ソース"
            Case 158243
                Return "管理税率"
            Case 158246
                Return "管理ユニット"
            Case 158249
                Return "ユニットの管理"
            Case 158306
                Return "選択"
            Case 158346
                Return "さらに"
            Case 158376
                Return "理論的な課価格"
            Case 158511
                Return "もし、この事例に心当たりがない場合、当社にEメールを送ってください <a href='mailto:%email'>%email</a>"
            Case 158577
                Return "サイト言語"
            Case 158585
                Return "本社"
            Case 158588
                Return "別のユーザによって所有されているので、以下の項目を提出できません。"
            Case 158653
                Return "モバイル"
            Case 158677
                Return "販売アイテム¶番号"
            Case 158694
                Return "情報の変更"
            Case 158696
                Return "フィリピンの顧客のみ"
            Case 158730
                Return "除外する"
            Case 158783
                Return "レシピ/サブレシピを含む"
            Case 158810
                Return "計算価格"
            Case 158835
                Return "税で分類"
            Case 158837
                Return "価格で分類"
            Case 158839
                Return "品物コストで分類"
            Case 158840
                Return "定数で分類"
            Case 158845
                Return "販売価格で分類"
            Case 158846
                Return "課価格で分類"
            Case 158849
                Return "高い"
            Case 158850
                Return "低い"
            Case 158851
                Return "作成者"
            Case 158860
                Return "POS設定を変更する"
            Case 158902
                Return "始業時間"
            Case 158912
                Return "申請"
            Case 158935
                Return "総財源"
            Case 158947
                Return "あなたの注文を終えるためにPaypalにリダイレクトされます。"
            Case 158952
                Return "承認した"
            Case 158953
                Return "承認していない"
            Case 158960
                Return "この機能は無効になりました。もし新しいレシピが必要なら本社に連絡してください。"
            Case 158998
                Return "検索の特徴"
            Case 158999
                Return "商品、レシピおよびメニューリストは、その詳細、値段および栄養価と共に印刷することができます。ショッピングリストや様々なレシピで使用されている材料のリストも累積量とともに印刷することができます。　　　　　PDＦとExcelファイルもまた様々なレポートのために作成できます。"
            Case 159000
                Return "価格とマルチ通貨の設定"
            Case 159009
                Return "境界"
            Case 159035
                Return "不完全"
            Case 159064
                Return "名前は空欄にできません"
            Case 159082
                Return "最新日付変更によりプロダクトを更新する"
            Case 159089
                Return "承認のための申請をキャンセルする"
            Case 159112
                Return "承認のため"
            Case 159113
                Return "継続"
            Case 159133
                Return "シッピングインフォメーション"
            Case 159139
                Return "構成"
            Case 159140
                Return "ユニットが長すぎます"
            Case 159141
                Return "ユニット%nは存在していません。"
            Case 159142
                Return "%nは空欄にできません。"
            Case 159144
                Return "ファイルをインポートしています。お待ちください…"
            Case 159145
                Return "アイテムを保存しています。お待ちください…"
            Case 159162
                Return "詳細を隠す"
            Case 159168
                Return "正味量で分類した"
            Case 159169
                Return "総量で分類した"
            Case 159171
                Return "予定"
            Case 159181
                Return "総計で分類した"
            Case 159264
                Return "商品CSV/供給元ネットワークをインポートする"
            Case 159273
                Return "合計コントリビューションマージン"
            Case 159275
                Return "ライセンスにより制限されています"
            Case 159298
                Return "メニューキーワード"
            Case 159349
                Return "フィルターをリセットする"
            Case 159360
                Return "プロパティシェフ"
            Case 159361
                Return "エグゼクティブシェフ"
            Case 159362
                Return "選択されたアイテムは現在"
            Case 159363
                Return "銘柄情報を入力する"
            Case 159364
                Return "銘柄"
            Case 159365
                Return "役割"
            Case 159366
                Return "サーバーでSMTPを使う"
            Case 159367
                Return "ネットワークでSTMPを使う"
            Case 159368
                Return "ロゴ"
            Case 159369
                Return "により比較する"
            Case 159370
                Return "インポートされた"
            Case 159372
                Return "グローバル"
            Case 159379
                Return "上昇する"
            Case 159380
                Return "下降する"
            Case 159381
                Return "すべてのユーザーに"
            Case 159382
                Return "システムレシピに変更する"
            Case 159383
                Return "閲覧させない"
            Case 159384
                Return "プロパティー"
            Case 159385
                Return "エントリーを送信する"
            Case 159386
                Return "価格と栄養は再計算されていません"
            Case 159387
                Return "価格と栄養は再計算されました"
            Case 159388
                Return "メニューカードの新規作成"
            Case 159389
                Return "メニューカードの編集"
            Case 159390
                Return "Eメールは送信されました"
            Case 159391
                Return "認可された価格"
            Case 159424
                Return "この機能は無効になりました。もし新しいレシピが必要なら本社に連絡してください。"
            Case 159426
                Return "材料を名前か名前の一部で検索する。素早く検索するには[正味量]_[ユニット]_[材料]を入力する"
            Case 159430
                Return "登録情報は保存されました"
            Case 159433
                Return "システムに送信する"
            Case 159434
                Return "システムに送信された"
            Case 159435
                Return "新しいカテゴリーに移動する"
            Case 159436
                Return "システム警告通知のためのEメール送信元"
            Case 159437
                Return "ファイルのアップロードに成功しました"
            Case 159444
                Return "写真のサイズをインポーズする"
            Case 159445
                Return "タイムゾーン"
            Case 159446
                Return "イメージ処理"
            Case 159457
                Return "SQLサーバー　全文検索では文字データに関係なく複雑な問い合わせに対応できます。　全文検索によって似たような文も検索することができます。例えば''tomato''と検索したら""tomatoes""も検索結果に表示されます。SQL2009では名前、ノート（もしくは手順）、クエリーの材料のもとに検索結果ランキングを表すことができます。"
            Case 159458
                Return "フルポピュレーション"
            Case 159459
                Return "全文テキスト検索"
            Case 159460
                Return "分"
            Case 159461
                Return "あらゆる"
            Case 159462
                Return "起動"
            Case 159463
                Return "付加ポピュレーション"
            Case 159464
                Return "言語ワードブレーカー"
            Case 159471
                Return "IPアドレス"
            Case 159472
                Return "ブロックされたIPアドレス"
            Case 159473
                Return "ログインするときIPをブロックする"
            Case 159474
                Return "少なくとも ¶文字以上で入力してください"
            Case 159485
                Return "レシピ交換に送る"
            Case 159486
                Return "レシピ交換に送信されました。"
            Case 159487
                Return "このレシピを承認しました。このレシピはほかのユーザーに閲覧されることになっています。"
            Case 159488
                Return "未知の言語"
            Case 159607
                Return "スタンダローンレシピ管理ソフトウェア"
            Case 159608
                Return "ネットワークにおける兼任ユーザーのためのレシピ管理ソフトウェア"
            Case 159609
                Return "ウェブベースレシピ管理ソフトウェア"
            Case 159610
                Return "目録とバックオフィス管理ソフトウェア"
            Case 159611
                Return "ポケットPCレシピビューアー"
            Case 159612
                Return "注文の仕方と栄養のモニタリングソフトウェア"
            Case 159613
                Return "E-Cookbook ソフトウェア"
            Case 159699
                Return "存在しているアイテムの更新"
            Case 159707
                Return "フランス"
            Case 159708
                Return "ドイツ"
            Case 159751
                Return "サイト"
            Case 159778
                Return "詳細"
            Case 159779
                Return "基本"
            Case 159782
                Return "販売アイテムをプロダクトにリンク"
            Case 159783
                Return "販売アイテムをレシピ/メニューにリンク"
            Case 159795
                Return "POSインポートー環境設定"
            Case 159918
                Return "この機能にアクセスする権利がありません"
            Case 159924
                Return "管理"
            Case 159925
                Return "無効変換"
            Case 159929
                Return "ページオプション"
            Case 159934
                Return "栄養価"
            Case 159940
                Return "エクスポートの最新情報"
            Case 159941
                Return "すべてをエクスポート"
            Case 159942
                Return "出力ディレクトリ"
            Case 159943
                Return "質"
            Case 159944
                Return "基本"
            Case 159946
                Return "CALCMENU Web 2007"
            Case 159947
                Return "ファイルを選択するまたはアップロードしてください"
            Case 159949
                Return "フォーマットは１０文字以下に限られています"
            Case 159950
                Return "栄養名は２５文字以下に限られています"
            Case 159951
                Return "役割"
            Case 159962
                Return "税金情報を記入してください"
            Case 159963
                Return "翻訳を記入してください"
            Case 159966
                Return "マークした商品を新しい銘柄品に移動してください"
            Case 159967
                Return "デフォルトサイト名を記入してください"
            Case 159968
                Return "デフォルトウェブサイトのテーマを記入してください"
            Case 159969
                Return "プロパティグループサイトをプロパティの管理者によって管理されることを可能にする"
            Case 159970
                Return "ユーザーは、使用できるまたは公表される前に、認可する人に必要な情報を提出することがある。"
            Case 159971
                Return "一致する言語の翻訳を記入するまたはデフォルトテキストは使用されます。"
            Case 159973
                Return "このプロパティにふさわしいサイトを選択してください"
            Case 159974
                Return "商品、レシピ、メニュー、他の情報を翻訳するための利用できる言語を選択する。"
            Case 159975
                Return "商品、レシピ、メニューに価格を割り当てるための一つかそれ以上の価格のグループを選択してください"
            Case 159976
                Return "含める商品をチェックしてください"
            Case 159977
                Return "所有者リスト"
            Case 159978
                Return "下からフォーマットを選択してください"
            Case 159979
                Return "削除する基本リストを選択してください"
            Case 159981
                Return "下記のものはこの商品の共用サイトです。"
            Case 159982
                Return "マークしたものを新しいソースに移動してください"
            Case 159987
                Return "注文のタイプ"
            Case 159988
                Return "注文した人"
            Case 159990
                Return "銘柄を変更する"
            Case 159994
                Return "メニューの材料を取り替える"
            Case 159997
                Return "グローバルシェアリング"
            Case 160004
                Return "第一のレベル"
            Case 160005
                Return "選択した材料は次のユニットを持っている必要があります"
            Case 160008
                Return "段階"
            Case 160009
                Return "アクション"
            Case 160012
                Return "このレシピ/メニューはウェブで発表されています。"
            Case 160013
                Return "このレシピ/メニューはウェブに発表されていません。"
            Case 160014
                Return "ヒント"
            Case 160016
                Return "所有者を見る"
            Case 160018
                Return "この商品はウェブで発表されています"
            Case 160019
                Return "この商品はウェブに発表されています"
            Case 160020
                Return "この商品は陳列されています"
            Case 160021
                Return "この商品は陳列されていません"
            Case 160023
                Return "プリントアウト"
            Case 160028
                Return "公開されないもの"
            Case 160030
                Return "ショッピングリストに追加"
            Case 160033
                Return "キーワード追加"
            Case 160035
                Return "ログインを試みた回数は％回です。"
            Case 160036
                Return "このアカウントは無効になりました"
            Case 160037
                Return "このアカウントを有効にするためにはシステム管理者に連絡してください"
            Case 160038
                Return "私のプロフィール"
            Case 160039
                Return "最新のログイン"
            Case 160040
                Return "登録されていません。"
            Case 160041
                Return "言語ページ"
            Case 160042
                Return "主な翻訳"
            Case 160043
                Return "主な価格のセット"
            Case 160045
                Return "1ページ当たりの行数"
            Case 160046
                Return "デフォルトディスプレイ"
            Case 160047
                Return "材料の量"
            Case 160048
                Return "最新のアクセス"
            Case 160049
                Return "受取'%f'"
            Case 160050
                Return "長さ"
            Case 160051
                Return "%f'　を受け取れません"
            Case 160055
                Return "量は1以上でなければなりません"
            Case 160056
                Return "新しいサブレシピを作る"
            Case 160057
                Return "セッションは無効になりました"
            Case 160058
                Return "あなたのログインは％分の不活動状態のため無効になりました"
            Case 160065
                Return "No name"
            Case 160066
                Return "終了します。よろしいですか。"
            Case 160067
                Return "あなたの入力するためには許可が必要です。"
            Case 160068
                Return "許可を要求するためには'%s'をクリックしてください"
            Case 160070
                Return "マークした商品は処理中です"
            Case 160071
                Return "この入力は許可されるために提出されました。"
            Case 160072
                Return "この入力には存在している要求があります。"
            Case 160074
                Return "ユニットを選択してください"
            Case 160082
                Return "あなたからの許可を必要とする新しい要求があります。"
            Case 160085
                Return "あなたの要求は審査されました"
            Case 160086
                Return "栄養リストをプリントする"
            Case 160087
                Return "リストをプリントする"
            Case 160088
                Return "詳細をプリントする"
            Case 160089
                Return "有効にする"
            Case 160090
                Return "新規"
            Case 160091
                Return "選択したものはリストから削除する"
            Case 160093
                Return "グローバルシェアリングのためにシステムに送る"
            Case 160094
                Return "内容をキオスクブラウザーで利用できるようにする"
            Case 160095
                Return "システムコピーを作る"
            Case 160096
                Return "レシピとメニューで利用された材料を取替える"
            Case 160098
                Return "ウェブに発表しないでください"
            Case 160100
                Return "購入する材料のリストを作る"
            Case 160101
                Return "量や価格の詳細が必要のない材料としてテキストを利用できます"
            Case 160102
                Return "レシピデータベースの作成、共有とプリントアウト及びショッピングリストの作成を行います"
            Case 160103
                Return "作成したレシピと原材料をもとに作成されます。"
            Case 160105
                Return "ユーザー情報や原材料の供給元等の情報を管理します。"
            Case 160106
                Return "ようこそ"
            Case 160107
                Return "%ｓへようこそ"
            Case 160108
                Return "背景とほかの設定をカスタマイズする"
            Case 160109
                Return "ウェブサイトプロフィール"
            Case 160110
                Return "ウェブサイト名やテーマなどをカスタマイズする"
            Case 160111
                Return "ルーチンの認可"
            Case 160112
                Return "商品アイテムやレシピやほかの情報の認可"
            Case 160113
                Return "SMTPと警告通知の設定"
            Case 160114
                Return "メールサーバーへの接続の環境設定をする；有効にする、または、無効にする"
            Case 160115
                Return "最大ログインの試みを設定する；封鎖されたIPアドレスをモニターする"
            Case 160116
                Return "プロフィールを印刷する"
            Case 160117
                Return "倍数の印刷フォーマットをプロフィールとして同定する"
            Case 160118
                Return "商品、レシピ、メニュー、その他の情報の翻訳言語を設定する。"
            Case 160119
                Return "使用可能な通貨と価格の設定"
            Case 160120
                Return "商品、レシピ、メニューの価格をマルチに設定する。"
            Case 160121
                Return "プロパティーはサイトのグループです。"
            Case 160122
                Return "サイトはレシピを一緒に取り組んでいる特定のユーザを組織化します。"
            Case 160123
                Return "%sのユーザを管理します。"
            Case 160124
                Return "好みのイメージを構成する。"
            Case 160125
                Return "商品、レシピ、メニューの写真を標準化する。"
            Case 160130
                Return "商品を特定する商標もしくは固有の名前。"
            Case 160132
                Return "商品、レシピ、メニューは一般的な属性でグループ化されます。"
            Case 160135
                Return "キーワードは、商品、レシピ、メニューの詳細をイメージするのに役立ちます。ユーザーは自由にそしてマルチにキーワードが設定できます。"
            Case 160139
                Return "最大34種の栄養価を設定できます"
            Case 160141
                Return "探索用に使用できる追加フィルタを作成する。"
            Case 160151
                Return "事前に定義された(または、システム)ユニットのリストは商品の価格に適用されレシピとメニューに反映されます。"
            Case 160152
                Return "ユーザーはこのリストを追加できます。"
            Case 160153
                Return "使用された価格での計算"
            Case 160154
                Return "ソースは取り込むレシピの元となる事項／シェフ、本、雑誌、フード・サービス会社、組織、ウェブサイト等を意味します。"
            Case 160155
                Return "CALCMENU Pro, CALCMENU Enterprise, またはその他の EGS productsからの商品、レシピ、メニューのインポート"
            Case 160156
                Return "異なる通貨レートのメンテナンス"
            Case 160157
                Return "使用されていないテキストの削除"
            Case 160158
                Return "すべてのテキストフォーマット"
            Case 160159
                Return "HTML, Excel, PDF, RTFフォーマットの商品リストをプリントアウト"
            Case 160160
                Return "HTML, Excel, PDF, RTFフォーマットの商品詳細をプリントアウト"
            Case 160161
                Return "HTML, Excel, PDF, RTFフォーマットのレシピ詳細をプリントアウト"
            Case 160162
                Return "HTML, Excel, PDF, RTFフォーマットのレシピリストをプリントアウト"
            Case 160163
                Return "HTML, Excel, PDF, RTFフォーマットのメニュー詳細をプリントアウト"
            Case 160164
                Return "メニューエンジニアリングは現在から今後のレシピの価格設定とデザインを評価します。 メニューと個々のメニュー項目を分析して、最適利益を達成してください。 メニューエンジニアリングを使用すれば、メニューからどのメニュー商品を保有すべきかまたは削除すべきかを特定出来ます。"
            Case 160169
                Return "メニューカードをロード"
            Case 160170
                Return "メニューカードを編集、保存"
            Case 160175
                Return "ショッピングリストをを編集、保存"
            Case 160177
                Return "セキュリティー"
            Case 160180
                Return "アイテムのフォーマットを標準化"
            Case 160181
                Return "パージアイテム"
            Case 160182
                Return "役割の権利"
            Case 160184
                Return "TCPOSエキスポート"
            Case 160185
                Return "販売アイテムのエキスポート"
            Case 160187
                Return "レシピに使用する新しいローカルの商品を作成"
            Case 160188
                Return "保存されたマークのリストを表示する"
            Case 160189
                Return "購入するアイテムのリストを表示する"
            Case 160190
                Return "データーベースの存在するレシピで自分のメニューを作る"
            Case 160191
                Return "レシピとメニューのためのテキストを作る"
            Case 160200
                Return "名前で分類"
            Case 160202
                Return "リストから選択"
            Case 160209
                Return "シリアル番号、ヘッダー名、プロダクトキーを入力してください。%sにより供給された文書でこの情報を見つけます。"
            Case 160210
                Return "必要なアイテム"
            Case 160211
                Return "不要なアイテム"
            Case 160212
                Return "ドラフト"
            Case 160217
                Return "アーカイブパス"
            Case 160218
                Return "エラーのあるインポート商品のデータ"
            Case 160219
                Return "修複する商品の未定のリスト"
            Case 160220
                Return "インポート商品のオプションを同定する"
            Case 160254
                Return "変更が有効するためにウィンドウサービス％ｎを再起動してください"
            Case 160258
                Return "通貨は選択した価格のセットと合いません"
            Case 160259
                Return "名前または数字は存在しています"
            Case 160260
                Return "インポートの日付"
            Case 160262
                Return "栄養価は１出来高ユニット当たりです"
            Case 160292
                Return "アレルゲン"
            Case 160293
                Return "商品に関連のある食物のアレルギーのリスト"
            Case 160295
                Return "このアカウントは只今使用されています。後ほどまたログインしてください。"
            Case 160353
                Return "購入の価格セット"
            Case 160354
                Return "販売の価格セット"
            Case 160423
                Return "スタンダローンレシピ・メニュー管理ソフトウェア"
            Case 160433
                Return "内の消費"
            Case 160500
                Return "テキストマネージメント"
            Case 160687
                Return "代わりのアイテムカラー"
            Case 160688
                Return "標準のアイテムカラー"
            Case 160690
                Return "復元するとき、自動的に使っているシステムがカットオフされることに注意してください。"
            Case 160691
                Return "バックアップ/復元された写真"
            Case 160716
                Return "アイテムをデフォルトによってグローバルに設定する"
            Case 160774
                Return "復旧する"
            Case 160775
                Return "zeroesまでのあとを削除する。"
            Case 160777
                Return "ここをクリックしてCALCMENU Onlineについて更に知る"
            Case 160788
                Return "選択されたアイテムは起動されました。"
            Case 160789
                Return "選択されたアイテムは起動されませんでした。"
            Case 160790
                Return "本当に選択されたアイテムを削除しますか？"
            Case 160791
                Return "選択されたアイテムはうまく削除されました。"
            Case 160801
                Return "2つもしくは更なるレシピをマージすることができます。"
            Case 160802
                Return "選択したアイテムをマージしますか。"
            Case 160803
                Return "アイテムをクリアしますか。"
            Case 160804
                Return "必要事項を記入してください"
            Case 160805
                Return "マージするアイテムは二つ以上選択してください"
            Case 160806
                Return "選択したアイテムを無効にします。よろしいですか。"
            Case 160863
                Return "商品の値段リスト"
            Case 160940
                Return "有効期限"
            Case 160941
                Return "リンクされた販売アイテム"
            Case 160953
                Return "問屋の売り値から買値"
            Case 160958
                Return "セールスアイテムの販売価格をマルチに作動させる"
            Case 160985
                Return "リンクされていない販売アイテム"
            Case 160987
                Return "セールスアイテムを作成し、すでにあるレシピとリンクさせる"
            Case 160988
                Return "セールスアイテムは販売に使われ、通例レシピにリンクされます。"
            Case 161028
                Return "栄養データベースの変更を本当に保存したいですか？この変更によりあなたがすでに商品に設定した栄養の設定は変更されます。"
            Case 161029
                Return "収益もしくは原材料のチェックボックスを選択してください。"
            Case 161049
                Return "キーワード、サブキーワードの削除を強制する"
            Case 161050
                Return "消去されたキーワードは商品、レシピ、メニュー項目からも割り当てられることはありません。"
            Case 161051
                Return "選択されたキーワードとすべてのサブキーワードの削除に成功しました。消去されたキーワードは商品、レシピ、メニュー項目からも割り当てられることはありません。"
            Case 161078
                Return "全文検索"
            Case 161079
                Return "文頭検索"
            Case 161080
                Return "キー検索"
            Case 161082
                Return "二回目"
            Case 161083
                Return "三回目"
            Case 161084
                Return "四回目"
            Case 161085
                Return "一回のみ"
            Case 161086
                Return "毎日"
            Case 161087
                Return "毎週"
            Case 161088
                Return "毎月"
            Case 161089
                Return "ファイルが変わった際"
            Case 161090
                Return "コンピュータを起動した際"
            Case 161091
                Return "％ｓ情報を入力する"
            Case 161092
                Return "供給グループ"
            Case 161093
                Return "勘定情報"
            Case 161094
                Return "開始する日"
            Case 161095
                Return "今月の"
            Case 161096
                Return "POS　インポート-　成功しなかったデータ"
            Case 161097
                Return "供給元の情報、例えば会社の連絡先、住所、支払期限などをオーダーを容易にするために編集、維持する"
            Case 161098
                Return "ターミナルとはCALCMENU Web とリンクされたPOSのステーションを意味します。このプログラムではターミナルを追加、編集、削除します。"
            Case 161099
                Return "POSインポートパラメータを配列する。スケジュールやインポートファイルの場所などを設定する。"
            Case 161100
                Return "製品と在庫商品は違う時期に違う場所に保存、流通されます。製品がいつでも見つけられるように場所を設定し管理します。"
            Case 161101
                Return "クライアントは商品を購入する会社を意味します。このプログラムを使ってクライアントリストを管理します。"
            Case 161102
                Return "クライアントコンタクトは会社で取り引きをする担当者を意味いします。クライアントコンタクトの作成、編集、削除"
            Case 161103
                Return "システムにインポートされていないPOSデータを修正する。"
            Case 161104
                Return "これは供給取引状況を管理します。 実際に従業員利益や景品などの顧客にこれを販売したかもしれません。"
            Case 161105
                Return "販売ヒストリーに取引状況と販売アイテムが提示されます。"
            Case 161106
                Return "マークしたアイテム"
            Case 161107
                Return "計算した出来高"
            Case 161132
                Return "マイレシピを見る"
            Case 159274
                Return "％ナンバーのみ"
            Case 161147
                Return "レシピとメニュー管理（メニュープラニング無し）"
            Case 161162
                Return "TCPOS"
            Case 155761
                Return "商品にインポート"
            Case 161180
                Return "自動更新の環境設定を同定する"
            Case 161181
                Return "ホスト名"
            Case 11060
                Return "ディレクトリー"
            Case 24068
                Return "余白"
            Case 158734
                Return "データベースバージョンはこのプログラムのバージョンとは適合していません。"
            Case 161275
                Return "日用品の総計のガイドライン"
            Case 161276
                Return "GDA"
            Case 7250
                Return "フランス語"
            Case 7280
                Return "イタリア語"
            Case 7260
                Return "ドイツ語"
            Case 157515
                Return "オランダ語"
            Case 158868
                Return "中国語"
            Case 161279
                Return "なし"
            Case 54295
                Return "と"
            Case 159468
                Return "材料として使用されている"
            Case 159469
                Return "材料として使用されていない"
            Case 134159
                Return "全て"
            Case 144582
                Return "グループがありません"
            Case 161281
                Return "パワーコック"
            Case 161282
                Return "プロパーティ管理者"
            Case 161283
                Return "システム管理者"
            Case 161284
                Return "コック長"
            Case 161285
                Return "プロパティシェフ"
            Case 161286
                Return "コック"
            Case 161287
                Return "ゲスト"
            Case 161288
                Return "サイトのシェフ"
            Case 161289
                Return "サイトの管理者"
            Case 161290
                Return "表示と印刷"
            Case 161291
                Return "定義されません"
            Case 161292
                Return "定義された"
            Case 161294
                Return "不要な%s"
            Case 24269
                Return "すべてを選択"
            Case 24268
                Return "すべてを外す"
            Case 160880
                Return "再計算"
            Case 160894
                Return "Silver"
            Case 14110
                Return "フッター"
            Case 161300
                Return "主要な購入の価格のセット"
            Case 160776
                Return "％ｓに戻る"
            Case 132617
                Return "全てのカテゴリー"
            Case 155842
                Return "人"
            Case 155050
                Return "全てのキーワード"
            Case 135024
                Return "位置"
            Case 161333
                Return "見出し"
            Case 161334
                Return "レシピ %x-%y of %z"
            Case 104836
                Return "プロダクトを修正"
            Case 51281
                Return "使用原材料"
            Case 158349
                Return "指定されたキーワード"
            Case 158350
                Return "引き出したキーワード"
            Case 119130
                Return "検索"
            Case 155927
                Return "全てのソース"
            Case 161484
                Return "温度"
            Case 161485
                Return "製造<br />日付"
            Case 161486
                Return "消費<br />日付"
            Case 31700
                Return "日中に"
            Case 7030
                Return "プリンター"
            Case 161487
                Return "日用品"
            Case 161488
                Return "賞味有効期限"
            Case 161489
                Return "新鮮に用意された"
            Case 161490
                Return "アレルギー情報；入っているものは："
            Case 161491
                Return "全てをマークしたものに指定しました"
            Case 4825
                Return "レシピ"
            Case 21550
                Return "料理が見つかりません"
            Case 24011
                Return "の"
            Case 161494
                Return "最高5°C"
            Case 161538
                Return "EGS社製品をご検討いただきありがとうございます。"
            Case 161554
                Return "<a href=''%url''>Product Resourcesのページで</a>PDF形式で製品の紹介をご覧いただけます。"
            Case 161576
                Return "ユニット価格"
            Case 133328
                Return "レシピ名"
            Case 51128
                Return "レシピ名"
            Case 161577
                Return "時間"
            Case 161578
                Return "原材料コスト総計"
            Case 161579
                Return "計算"
            Case 161580
                Return "原材料コスト総計"
            Case 161581
                Return "税"
            Case 161582
                Return "粗利益／Ｆｒ"
            Case 161583
                Return "粗利益／％"
            Case 159733
                Return "品番"
            Case 161584
                Return "単位"
            Case 143003
                Return "正味¶数量"
            Case 155811
                Return "総¶量"
            Case 161585
                Return "価格／単位"
            Case 132708
                Return "供給元がありません"
            Case 24075
                Return "品番"
            Case 27056
                Return "と"
            Case 161766
                Return "ポーション（小）"
            Case 161767
                Return "ポーション（大）"
            Case 156892
                Return "ダウンロード:"
            Case 161777
                Return "割り当てのないキーワード"
            Case 161778
                Return "キーワードの設定"
            Case 161779
                Return "順路のマーキング"
            Case 161780
                Return "順路のマーキングをモニター"
            Case 161781
                Return "不要なキーワード"
            Case 161782
                Return "ラベルを印刷する"
            Case 161783
                Return "手順のテンプレート"
            Case 161784
                Return "学生"
            Case 161785
                Return "原材料の栄養価／％ｓ"
            Case 161786
                Return "原材料の栄養価／100g/ml"
            Case 155926
                Return "エクセルにエクスポートする"
            Case 161787
                Return "テンプレートを適用"
            Case 135969
                Return "本当に%oを取り替えますか？"
            Case 132934
                Return "最新レシピ"
            Case 132937
                Return "最新メニュー"
            Case 161788
                Return "割り当てられた／生成されたキーワード"
            Case 161468
                Return "全て有効にする"
            Case 161823
                Return "行を追加"
            Case 161824
                Return "クリップボードから追加"
            Case 161825
                Return "リンクされる必要がある商品はありません。"
            Case 161826
                Return "他を選択"
            Case 8514
                Return "新しい価格"
            Case 161827
                Return "デフォルト 価格／単位"
            Case 161828
                Return "既存の単位から、選んでください。"
            Case 161829
                Return "新規単位として追加する。"
            Case 161831
                Return "加える前に、商品を編集する。"
            Case 161832
                Return "補足する場所%s"
            Case 161834
                Return "価格を確認してください。"
            Case 161835
                Return "カット"
            Case 159594
                Return "レシピに追加する"
            Case 161837
                Return "レシピに追加する"
            Case 10447
                Return "注文"
            Case 161838
                Return "既存の原材料を入れ替える"
            Case 161839
                Return "原材料が見つかりません。"
            Case 132672
                Return "%nを本当に削除しますか?"
            Case 161840
                Return ""
            Case 161841
                Return "商品もしくはサブレシピにリンク"
            Case 161842
                Return "すべてのアイテムが商品もしくはサブレシピにリンクされました。"
            Case 161843
                Return "アイテムが商品もしくはサブレシピにリンクされました。"
            Case 161844
                Return "保蔵時間"
            Case 161845
                Return "保蔵温度"
            Case 161851
                Return "オーダー可"
            Case 161852
                Return "レシピにアレルゲンが含まれています"
            Case 159088
                Return "承認のための申請を送信する"
            Case 161855
                Return "ドラフト"
            Case 161986
                Return "工程を追加して下さい。"
            Case 161853
                Return "ペースト"
            Case 161987
                Return "%pのアイテム%n"
            Case 161988
                Return "リンクされた製品"
            Case 161989
                Return "リンクされていない製品"
            Case 158851
                Return "作成者"
            Case 161830
                Return "有効な項目"
            Case 162198
                Return "利回りを変えました。 計算ボタンをクリックして、原材料の量をリサイズしてください。"
            Case 162199
                Return "利回りを変えました。 原材料の量を計算しないで保存を続けますか?"
            Case 162203
                Return "インフォメーション"
            Case 162205
                Return "入札数"
            Case 162208
                Return "毎週の営業日"
            Case 151500
                Return "提案"
            Case 162211
                Return "言語を選択"
            Case 162212
                Return "ビジネス名"
            Case 162213
                Return "ビジネス数"
            Case 162214
                Return "使用可能な価格"
            Case 162215
                Return "ロゴをサーバーにロードする"
            Case 146043
                Return "1月"
            Case 146044
                Return "2月"
            Case 146045
                Return "3月"
            Case 146046
                Return "4月"
            Case 146047
                Return "5月"
            Case 146048
                Return "6月"
            Case 146049
                Return "7月"
            Case 146050
                Return "8月"
            Case 146051
                Return "9月"
            Case 146052
                Return "10月"
            Case 146053
                Return "11月"
            Case 146054
                Return "12月"
            Case 162216
                Return "好み"
            Case 162219
                Return "バックオフィス"
            Case 162221
                Return "基本設定"
            Case 162222
                Return "ここにインサートする"
            Case 8990
                Return "または"
            Case 162230
                Return "スタイル情報を入力"
            Case 162231
                Return "スタイル名"
            Case 162232
                Return "ヘッダースタイルオプション"
            Case 160237
                Return "太字"
            Case 134826
                Return "終了された"
            Case 162235
                Return "もしかして？"
            Case 159700
                Return "レシピのインポート"
            Case 162276
                Return "レシピをインポート"
            Case 162282
                Return "注釈"
            Case 159681
                Return "レシピ (%s)の材料数が多すぎます(最大 %n)"
            Case 135257
                Return "総計マージン"
            Case 31732
                Return "メニュープラン"
            Case 162340
                Return "通り"
            Case 162341
                Return "場所"
            Case 162357
                Return "例"
            Case 162358
                Return "接頭語の長さを保ってください"
            Case 162359
                Return ""
            Case 162361
                Return "タブ"
            Case 162362
                Return "パイプ"
            Case 162363
                Return "セミコロン"
            Case 162364
                Return "スペース"
            Case 133590
                Return "&Paste"
            Case 155260
                Return "課要素"
            Case 156060
                Return "課されたフードコスト"
            Case 156061
                Return "課された利益"
            Case 162383
                Return "承認"
            Case 162382
                Return "承認する"
            Case 162386
                Return "進む"
            Case 162387
                Return "こんにちは、承認者あなたは承認されたレシピを受けました。この名前の製作者がこのレシピを提出しました: [...]CALCMENUウェブサイトにログインし、レシピを承認してください。敬具EGSチーム"
            Case 162388
                Return "こんにちは承認のためにあなたの新たに作成されたレシピを送りました。 最初に、オンラインで使用する前に、承認してください。 あなたはこのレシピを提出しました: [...]いったん承認されると、レシピはオンラインで利用可能になるでしょう。敬具EGSチーム"
            Case 162389
                Return "こんにちは、承認者様あなたはこのレシピを承認しました: [...]レシピはオンラインで利用可能になります。敬具EGSチーム"
            Case 162390
                Return "こんにちはレシピ、…は承認されました。 あなたは、現在、オンラインでこのレシピを使用できます。敬具EGSチーム"
            Case 162530
                Return "ログインの際に順路のマーキングを削除してください。"
            Case 28483
                Return "記録は存在していません"
            Case 162955
                Return "ネットマージン％"
            Case 132900
                Return "価格を追加"
            Case 163032
                Return "プライスリストをコピー"
            Case 155995
                Return "確認しています…"
            Case 156784
                Return "エラーの合計: %n"
            Case 51174
                Return "インポートを完成しました"
            Case 133334
                Return "%rをインポートしています。"
            Case 163046
                Return "%k%n%u のキーワードは見つかりません。 `キーワード閲覧' を押し、有効なキーワードを選択して下さい。"
            Case 135283
                Return "最新の価格"
            Case 156542
                Return "加重平均価格"
            Case 147381
                Return "以前の製品のため使用された目録価格"
            Case 157281
                Return "初期設定の供給元の価格"
            Case 163057
                Return "計％のコスト"
            Case 163058
                Return "１％のコスト"
            Case 132553
                Return "課販売価格+税"
            Case 138031
                Return "棚卸のためのすべての製品"
            Case 138032
                Return "マークしたカテゴリーからの製品"
            Case 138033
                Return "マークした保管場所からの製品"
            Case 138034
                Return "マークした供給元からの製品"
            Case 138035
                Return "以前の棚卸からの製品"
            Case 138030
                Return "この棚卸のために必要な製品を選択してください。"
            Case 163060
                Return "フードコスト％"
            Case 163061
                Return "課されたフードコスト％"
            Case 167719
                Return "Budget"
            Case 158410
                Return "もしいくつかのプロダクトが確定した価格がなければ（価格＝０）、代わりに初期設定の供給元の価格を使用する。"
            Case 136230
                Return "新しい棚卸を作成する"
            Case 136231
                Return "棚卸情報を変更する"
            Case 3205
                Return "名前"
            Case 135235
                Return "在庫有高"
            Case 135100
                Return "参照番号"
            Case 135110
                Return "数量¶棚卸"
            Case 160414
                Return "プレビュー数量¶棚卸"
            Case 136100
                Return "最近開いた目録"
            Case 136115
                Return "アイテムの＃"
            Case 136110
                Return "に開いた"
            Case 1146
                Return "読み込み中"
            Case 134021
                Return "棚卸開始日"
            Case 124164
                Return "目録の調整"
            Case 158946
                Return "手持ち量を数量目録に設定する"
            Case 136213
                Return "最新の棚卸に商品を追加する"
            Case 136214
                Return "棚卸から商品を削除する"
            Case 136212
                Return "必要な調整のリストを表示する"
            Case 136215
                Return "新しい商品の保管場所を追加する"
            Case 136217
                Return "選択した保管場所の商品量を削除する"
            Case 155861
                Return "選択されたアイテムの量をゼロにリセットする"
            Case 136216
                Return "選択した商品の保管場所を削除する"
            Case 157336
                Return "適用できません"
            Case 136030
                Return "内容"
            Case 133147
                Return "リットル"
            Case 136432
                Return "無効なコード"
            Case 143981
                Return "無効なアカウントコード"
            Case 169310
                Return "開発"
            Case 169318
                Return "フィードバック"
            Case 110447
                Return "発注"
            Case 158216
                Return "いつでも、どこでもレシピの集中管理。"
            Case 168373
                Return "ウエッブ公開"
            Case 168374
                Return "参照 No1"
            Case 168375
                Return "参照 No2"
            Case 157060
                Return "参照番号"
            Case 157659
                Return "ロックする"
            Case 157660
                Return "ロックを外す"
            Case 170155
                Return "Assign ingredient, recipes and menus to Categories, Keywords and Sources (could be a cookbook, Website, chef, etc.). This allows you to group and organize items in EGS CALCMENU Web. Searching for ingredient, recipes or menus can be made faster and easier since Categories, Keywords, and Sources are very useful in narrowing down search results."
            Case 160232
                Return "にエクスポートする"
            Case 170770
                Return "Yield to Print"
            Case 133248
                Return "材料"
            Case 170779
                Return "Ingredient List"
            Case 170780
                Return "Ingredient Details"
            Case 170781
                Return "Ingredient Nutrient List"
            Case 170782
                Return " Ingredient Category"
            Case 170783
                Return "Ingredient Keyword"
            Case 170784
                Return "Ingredient Published On The Web"
            Case 170785
                Return "Ingredient Not Published On The Web"
            Case 170786
                Return "Ingredient Cost"
            Case 170849
                Return "Abbreviated Preparation Method"
            Case 171301
                Return "Preparation Method"
            Case 171302
                Return "Tips"
            Case 170850
                Return "Cook Mode only"
            Case 133115
                Return "全てのレシピ"
            Case 170851
                Return "None Cook Mode only"
            Case 170852
                Return "Show Off"
            Case 170853
                Return "Quick & Easy"
            Case 170854
                Return "Chef Recommended"
            Case 170855
                Return "Moderate"
            Case 170856
                Return "Challenging"
            Case 170857
                Return "Gold"
            Case 170858
                Return "Unrated"
            Case 170859
                Return "Bronze"
            Case 170860
                Return "Move marked to new standard"
            Case 171219
                Return "LeadIn"
            Case 55011
                Return "Serving Size"
            Case 171220
                Return "Servings per Yield" ' "Number of Servings"
            Case 171221
                Return "Total Yield/Servings" '"Total Yield"
            Case 151436
                Return "Attachment"
            Case 150009
                Return "Exportation Done. BrandSite Successfully Exported."
            Case 171597
                Return "Recipe has been checked in by another user and cannot be modified."
            Case 27220
                Return "Hour"

            Case 171650
                Return "Prep Time"
            Case 171651
                Return "Cook Time "
            Case 171652
                Return "Marinate Time "
            Case 171653
                Return "Stand Time "
            Case 171654
                Return "Chill Time "
            Case 171655
                Return "Brew Time "
            Case 171656
                Return "Freeze Time "
            Case 171657
                Return "ReadyIn"
            Case 171658
                Return "second"
            Case 171616
                Return "Placement"
            Case 176055
                Return ""

        End Select
    End Function


End Module
