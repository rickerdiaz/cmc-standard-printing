using System.Collections.Generic;

namespace CmcStandardPrinting.Domain.NetworkSuppliers;

public sealed class NetworkSupplierUnitManage
{
    public int TransactionType { get; set; }
    public string Unit1 { get; set; } = string.Empty;
    public string Unit2 { get; set; } = string.Empty;
    public string Unit3 { get; set; } = string.Empty;
    public string Unit4 { get; set; } = string.Empty;
    public int Unit1Code { get; set; }
    public int Unit2Code { get; set; }
    public int Unit3Code { get; set; }
    public int Unit4Code { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseMessage { get; set; } = string.Empty;
}

public sealed class SupplierNetworkMerchandise
{
    public int CodeSetPrice { get; set; }
    public int CodeUser { get; set; }
    public int CodeTrans { get; set; }
    public int CodeSite { get; set; }
    public string EgsRef { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Declaration { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Preparation { get; set; } = string.Empty;
    public string CookingTip { get; set; } = string.Empty;
    public string Refinement { get; set; } = string.Empty;
    public string Storage { get; set; } = string.Empty;
    public string Productivity { get; set; } = string.Empty;
    public string Allergen { get; set; } = string.Empty;
    public string CountryOrigin { get; set; } = string.Empty;
    public string Attachment { get; set; } = string.Empty;
    public string SpecificDetermination { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public double Price1 { get; set; }
    public double Price2 { get; set; }
    public double Price3 { get; set; }
    public double Price4 { get; set; }
    public double Ratio1 { get; set; }
    public double Ratio2 { get; set; }
    public double Ratio3 { get; set; }
    public string Unit1 { get; set; } = string.Empty;
    public string Unit2 { get; set; } = string.Empty;
    public string Unit3 { get; set; } = string.Empty;
    public string Unit4 { get; set; } = string.Empty;
    public string Tax { get; set; } = string.Empty;
    public double N1 { get; set; }
    public double N2 { get; set; }
    public double N3 { get; set; }
    public double N4 { get; set; }
    public double N5 { get; set; }
    public double N6 { get; set; }
    public double N7 { get; set; }
    public double N8 { get; set; }
    public double N9 { get; set; }
    public double N10 { get; set; }
    public double N11 { get; set; }
    public double N12 { get; set; }
    public double N13 { get; set; }
    public double N14 { get; set; }
    public double N15 { get; set; }
    public double N16 { get; set; }
    public double N17 { get; set; }
    public double N18 { get; set; }
    public double N19 { get; set; }
    public double N20 { get; set; }
    public double N21 { get; set; }
    public double N22 { get; set; }
    public double N23 { get; set; }
    public double N24 { get; set; }
    public double N25 { get; set; }
    public double N26 { get; set; }
    public double N27 { get; set; }
    public double N28 { get; set; }
    public double N29 { get; set; }
    public double N30 { get; set; }
    public double N31 { get; set; }
    public double N32 { get; set; }
    public double N33 { get; set; }
    public double N34 { get; set; }
    public double N35 { get; set; }
    public double N36 { get; set; }
    public double N37 { get; set; }
    public double N38 { get; set; }
    public double N39 { get; set; }
    public double N40 { get; set; }
    public double N41 { get; set; }
    public double N42 { get; set; }
    public string N1Name { get; set; } = string.Empty;
    public string N2Name { get; set; } = string.Empty;
    public string N3Name { get; set; } = string.Empty;
    public string N4Name { get; set; } = string.Empty;
    public string N5Name { get; set; } = string.Empty;
    public string N6Name { get; set; } = string.Empty;
    public string N7Name { get; set; } = string.Empty;
    public string N8Name { get; set; } = string.Empty;
    public string N9Name { get; set; } = string.Empty;
    public string N10Name { get; set; } = string.Empty;
    public string N11Name { get; set; } = string.Empty;
    public string N12Name { get; set; } = string.Empty;
    public string N13Name { get; set; } = string.Empty;
    public string N14Name { get; set; } = string.Empty;
    public string N15Name { get; set; } = string.Empty;
    public string N16Name { get; set; } = string.Empty;
    public string N17Name { get; set; } = string.Empty;
    public string N18Name { get; set; } = string.Empty;
    public string N19Name { get; set; } = string.Empty;
    public string N20Name { get; set; } = string.Empty;
    public string N21Name { get; set; } = string.Empty;
    public string N22Name { get; set; } = string.Empty;
    public string N23Name { get; set; } = string.Empty;
    public string N24Name { get; set; } = string.Empty;
    public string N25Name { get; set; } = string.Empty;
    public string N26Name { get; set; } = string.Empty;
    public string N27Name { get; set; } = string.Empty;
    public string N28Name { get; set; } = string.Empty;
    public string N29Name { get; set; } = string.Empty;
    public string N30Name { get; set; } = string.Empty;
    public string N31Name { get; set; } = string.Empty;
    public string N32Name { get; set; } = string.Empty;
    public string N33Name { get; set; } = string.Empty;
    public string N34Name { get; set; } = string.Empty;
    public string N35Name { get; set; } = string.Empty;
    public string N36Name { get; set; } = string.Empty;
    public string N37Name { get; set; } = string.Empty;
    public string N38Name { get; set; } = string.Empty;
    public string N39Name { get; set; } = string.Empty;
    public string N40Name { get; set; } = string.Empty;
    public string N41Name { get; set; } = string.Empty;
    public string N42Name { get; set; } = string.Empty;
    public List<SupplierProductTranslation> ProductTranslation { get; set; } = new();
}

public sealed class SupplierProductTranslation
{
    public string CodeTrans { get; set; } = string.Empty;
    public string EgsRef { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Declaration { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Preparation { get; set; } = string.Empty;
    public string CookingTip { get; set; } = string.Empty;
    public string Refinement { get; set; } = string.Empty;
    public string Storage { get; set; } = string.Empty;
    public string Productivity { get; set; } = string.Empty;
    public string SpecificDetermination { get; set; } = string.Empty;
}
