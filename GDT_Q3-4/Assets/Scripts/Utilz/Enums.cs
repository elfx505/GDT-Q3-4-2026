public enum ItemType
{
    Note,
    Key,
    RatTrap,
    Marker,
    Screwdriver,
    GasCanister,
    Wrench,
    DefectivePrinterPaper,
    ResignationPaper,
    ElevatorHint,

}

public enum PuzzleColor
{
    Cyan,
    Yellow,
    Red,
    Green,
    Blue,
    Magenta
}

public enum GameState
{
    Default,
    EnteredOffice,
    KenGreeted,
    ComputerIntroDone,
    DocGiven1,
    DocGiven2,
    DocGiven3,
    PhoneRead,
    IntroSequenceDone,
    ReadNote,
    EnteredBathroom,
    SinkBroken,
    SinkRepaired,
    ReturnedToDesk,
    RatTrapPlaced,
    FingerSnapped,
    NumberDialed,
    PipePuzzleComplete1,
    PipePuzzleComplete2,
    PipePuzzleComplete3,
    AllPipePuzzlesCompleted,
    PrinterFueled,
    InsideJanitorOffice,
    ResignationPapersPrinted,
    JanitorDoorUnlocked,
    ReachedUpperElevator,
    JanitorSpokenTo,
    ElevatorEntered,
    ElevatorPuzzleDone,
    ElevatorButtonSequencePressed,
    EnteredReception,
    MinesweeperDone,
    ShuttersOpened
}