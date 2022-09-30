namespace PKHeX.Core;

public enum RanchPkOwnershipType : uint
{
    None              = 0x00000000U,
    Trainer           = 0x01000000U,
    Trainer_Conflict1 = 0x01000001U,
    Trainer_Conflict2 = 0x01000002U,
    Trainer_Conflict3 = 0x01000003U,
    Trainer_Conflict4 = 0x01000004U, 
    Trainer_Conflict5 = 0x01000005U,
    Trainer_Conflict6 = 0x01000006U,
    Trainer_Conflict7 = 0x01000007U,
    Hayley            = 0x04000000U,
    Hayley_Tradable   = 0x04000002U,
    Hayley_Traded     = 0x05000000U
}
