Public Module modCustomFTB
    Private ftbMap As New Dictionary(Of String, Integer)()

    Dim thisLock As New Object

    Private Sub LoadMap()
        ' RBAJ-2013.02.12 Makes sure that this will run only once
        SyncLock thisLock
            If ftbMap.Count = 0 Then
                ' For USA
                ftbMap.Add("28.171700", 171970) 'ProjectName
                ftbMap.Add("28.171755", 170386) 'Project
                ftbMap.Add("28.1260", 133248) 'Ingredients
                'AGL 2012.10.16 - CWM-1605
                ftbMap.Add("28.171986", 171985) 'Enter Project Information 
                'AGL 2012.10.17 - CWM-1592
                'ftbMap.Add("28.159364", 171670) 'Primary Brand 'removed
                ftbMap.Add("28.171988", 171989) 'Unwanted Primary Brand
                ' RBAJ-2012.10.17 [CWM-1589]
                ftbMap.Add("28.3230", 171618) 'Digital Asset
                ftbMap.Add("28.171796", 171990) 'Assign Project
                ftbMap.Add("28.171797", 171991) 'Unassign Project
                ftbMap.Add("28.171798", 171993) 'The recipe will be assigned to the selected project.
                ftbMap.Add("28.171799", 171994) 'The recipe will be unassigned from the project.
                ftbMap.Add("28.171806", 171995) 'Set Project(s)
                ftbMap.Add("28.171996", 171808) 'Set Recipe Placement
                ftbMap.Add("28.171617", 171616) 'Placement
                ftbMap.Add("28.171997", 171998) 'Manage BrandSite 
                'JTOC 18.10.2012
                ftbMap.Add("28.172001", 170857) 'Excellent/Gold
                ftbMap.Add("28.172002", 160894) 'Great/Silver
                ftbMap.Add("28.4877", 170859)   'Average/Bronze

                'AGL 2012.10.18 - CWM-1593
                ftbMap.Add("28.171973", 170860) 'Move marked items to new standard
                'AGL 2012.10.19 - CWM-1360
                ftbMap.Add("28.171840", 151286) 'Move marked items to new standard

                'JTOC 19.10.2012
                ftbMap.Add("28.171999", 172000) 'Picture & Time/Digital Asset & Time

                'JTOC 22.10.2012
                ftbMap.Add("28.172006", 171901) 'Assign Publication/Assign Promotion
                ftbMap.Add("28.3205", 14090)    'Name/Title

                'AGL 2012.10.23 - CWM-1808 - from "Ingredient List" to "Merchandise List"
                ftbMap.Add("28.5270", 170779)

                'AGL 2012.10.23 - CWM-1310 
                ftbMap.Add("28.160187", 171730) 'Create new merchandise description
                ftbMap.Add("28.160159", 171728) 'Print Merchandise List description
                ftbMap.Add("28.172030", 160103) 'Menu Description
                ftbMap.Add("28.172031", 160101) 'Text Description
                ftbMap.Add("28.132559", 171673) 'Create New Merchandise

                ftbMap.Add("28.157303", 171696) 'Guide_MerchandisePricing

                'JTOC 29.10.2012
                ftbMap.Add("28.172032", 171758) 'Please Enter Recipe Name/Please Enter Recipe Title
                ftbMap.Add("28.172034", 172033) 'Please Enter Recipe SubName/Please Enter Recipe SubTitle
                :
                'AGL 2012.10.30 - CWM-1937
                ftbMap.Add("28.29771", 132861)

                'AGL 2012.10.30 - CWM-1963
                ftbMap.Add("28.172039", 172040) 'Select a Cookbook
                ftbMap.Add("28.172046", 51129) 'Wanted Merchandise
                ftbMap.Add("28.172047", 51130) 'Unwanted Merchandise
                ftbMap.Add("28.172048", 159468) 'Used as Merchandise
                ftbMap.Add("28.172049", 159649) 'Not used as Merchandise


                'JTOC 31.10.2012
                ftbMap.Add("28.172042", 171900) 'Move mark to new Recipe Status/Move mark to new Recipe and Web Status
                ftbMap.Add("28.172043", 171807) 'Set Recipe Status/Set Recipe and Web Status
                ftbMap.Add("28.172045", 172044) 'Replace Recipe Status/Replace Recipe and Web Status

                'JTOC 13.12.2012
                ftbMap.Add("28.158849", 171832) 'High/Expensive
                ftbMap.Add("28.158850", 171833) 'Low/Cheap


                'AGL 2014.10.24
                ftbMap.Add("28.171834", 170386) 'Project/Cookbook
                ftbMap.Add("28.172007", 167385) 'SubTitle/SubName
                ftbMap.Add("28.171481", 171619) 'Brand Site/Kiosk
                ftbMap.Add("28.167149", 133248) 'Ingredient/Merchandise
                ftbMap.Add("28.155761", 171743) 'Import Ingredient/Import Merchandise


            End If
        End SyncLock
    End Sub

    Public Function GetCustomText(ByVal codeTrans As Integer, ByVal codeClient As Integer) As Integer
        If ftbMap.Count = 0 Then
            LoadMap()
        End If
        Dim val As Integer = codeTrans
        If Not ftbMap.TryGetValue(String.Concat(codeClient, ".", codeTrans), val) Then
            val = codeTrans
        End If
        Return val
    End Function

End Module
