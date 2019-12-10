using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Core
{
    public static class Res
    {
        public static Audio Audio { get; private set; }
        public static Tiles Tiles { get; private set; }
        public static SpriteFont Font { get; private set; }
        public static SpriteFont Font2 { get; private set; }
        public static ContentManager Content;
        public static bool ShownTutorial = false;

        public static string SfxSlide = "slide";
        public static string IttyBitty8 = "Itty_Bitty_8_Bit";
        public static string SfxDuckQuack = "duck-quack";
        public static string SfxMine = "mine";
        public static string SfxWhack = "whack";
        public static string SfxJump = "jump";
        public static string SfxChestOpen = "chestopen";
        public static string SfxGrassMove = "grassmove";
        public static string SfxBattleStart = "battlestart";
        public static string SfxClimb = "climb";
        public static string SfxLand = "land";
        public static string SfxChop = "chop";
        public static string SfxGrassCut = "grasscut";
        public static string SfxCoinGet = "coinget";
        public static string SfxSizzle = "sizzle";
        public static string SfxEnterWater = "enterwater";
        public static string SfxExitWater = "exitwater";
        public static string SfxZombHit0 = "zomb_hit_0";
        public static string SfxZombHit1 = "zomb_hit_1";
        public static string SfxZombGrowl0 = "zomb_growl_0";
        public static string SfxZombGrowl1 = "zomb_growl_1";
        public static string SfxDieSquishy = "die_squishy";
        public static string SfxLavaBall = "lavaball";
        public static string SfxSwordSwing = "axswing";
        public static string SfxExitStairs = "exit_stairs";
        public static string SfxBootsJump = "boots_jump";
        public static string SfxGetKeyItem = "getkeyitem";
        public static string SfxShowNewItem = "shownewitem";
        public static string SfxShowMenu = "showmenu";
        public static string SfxHideMenu = "hidemenu";
        public static string SfxTextBlip = "textblip";
        public static string SfxBombexplode = "bombexplode";
        public static string SfxTakeOutItem = "bombstart";
        public static string SfxBombsizzle = "bombsizzle";
        public static string SfxThrow = "throw";
        public static string SfxPickupItem = "pickupitem";
        public static string SfxPop1 = "pop1";
        public static string SfxPop2 = "pop2";
        public static string SfxPop3 = "pop3";
        public static string SfxMarbleDrop = "marbledrop";
        public static string SfxDoorOpen = "door_open";
        public static string SfxDoorLocked = "door_locked";
        public static string SfxDoorUnlock = "door_unlock";
        public static string SfxDoorClose = "door_close";
        public static string SfxGetPowerupLong = "get_powerup_long";
        public static string SfxGetPowerup = "get_powerup";
        public static string SfxPowerSwordChargeBase = "powerswordcharge";
        public static string SfxPowerSwordChargeFinal = "powerswordchargefinal";
        public static string SfxPowerSwordChargeRise = "powerswordchargerise";
        public static string SfxPowerSwordShoot = "powerswordshoot";
        public static string SfxTorchout = "torchout";
        public static string SfxFallthrough = "fallthrough";
        public static string SfxShieldOut = "shieldout";
        public static string SfxPlantBombGuyDamage = "plantbombguy_damage";
        public static string SfxPlantBombGuyHide = "plantbombguy_hide";
        public static string SfxPlantBombGuyUnHide = "plantbombguy_unhide";
        public static string SfxPlantBombExplode = "plantbomb_explode";
        public static string SfxShieldDeflect = "shield_deflect";
        public static string SfxSavePoint1 = "save_point_1";
        public static string SfxSavePoint2 = "save_point_2";
        public static string SfxSwitchButtonPress = "switch_button_press";
        public static string SfxPuzzleSolved = "good_thing";
        public static string SfxSwordLightFire = "sword_light_fire";
        public static string SfxElectronicNogo = "electronic_nogo";
        public static string SfxGrubDie = "grub_die";
        public static string SfxGrubHit = "grub_hit";
        public static string SfxChangeSubweapon = "change_subweapon";
        public static string SfxDrawBow = "draw_bow";
        public static string SfxArrowShoot = "arrow_shoot";
        public static string SfxNoArrowShoot = "bow_noarrow_shoot";
        public static string SfxDead = "dead";
        public static string SfxContinue = "continue";
        public static string SprGuyWalk = "SprGuyWalk";
        public static string SprGuyItemHeldWalk = "SprGuyItemHeldWalk";
        public static string SprGuyJump = "SprGuyJump";
        public static string SprGuyHang = "SprGuyHang";
        public static string SprGuyClimb = "SprGuyClimb";
        public static string SprGuyMount = "SprGuyMount";
        public static string SprGuyCurl = "SprGuyCurl";
        public static string SprGuyWalkAttack = "SprGuyWalkAttack";
        public static string SprGuyCrouchAttack = "SprGuyCrouchAttack";
        public static string SprGuyFall = "SprGuyFall";
        public static string SprTileDirt = "SprTileDirt";
        public static string SprTileCoal = "SprTileCoal";
        public static string SprTileSilver = "SprTileSilver";
        public static string SprTileCopper = "SprTileCopper";
        public static string SprTileGold = "SprTileGold";
        public static string SprNoGo = "SprNoGo";
        public static string SprCrack = "SprCrack";
        public static string SprParticleSmall = "SprParticleBrown";
        public static string SprSword = "SprSword";
        public static string SprSwordItem = "SprSwordItem";
        public static string SprPowerSword = "SprPowerSword";
        public static string SprPowerSwordItem = "SprPowerSwordItem";
        public static string SprPowerSwordProjectile = "SprPowerSwordProjectile";
        public static string SprShield = "SprShield";
        public static string SprPlinthEmpty = "SprPlinthEmpty";
        public static string SprPlinthSword = "SprPlinthSword";
        public static string SprPlinthShield = "SprPlinthShield";
        public static string SprBow = "SprBow";
        public static string SprArrow = "SprArrow";
        public static string SprPlinthBow = "SprPlinthBow";
        public static string SprTorch = "SprTorch";
        public static string SprTorchOut = "SprTorchOut";
        public static string SprTorchWall = "SprTorchWall";
        public static string SprTorchOutWall = "SprTorchOutWall";
        public static string SprLantern = "SprLantern";
        public static string SprHorizon = "SprHorizon";
        public static string SprGrassBlockTiles = "SprGrassTiles";
        public static string SprRockMonsterTiles = "SprRockMonsterTiles";
        public static string SprTreeTiles = "SprTreeTiles";
        public static string SprLadderTiles = "SprLadderTiles";
        public static string SprBigChest = "SprBigChest";
        public static string SprMountainBackdrop = "SprMountainBackdrop";
        public static string SprCaveBackgroundBlockTiles = "SprCaveBackgroundBlockTiles";
        public static string SprWoodBackgroundBlockTiles = "SprWoodBackgroundBlockTiles";
        public static string SprSandRockBack = "SprSandRockBack";
        public static string SprBlock_Rock = "SprBlock_Rock";
        public static string SprBlock_Copper = "SprBlock_Copper";
        public static string SprBlock_Gold = "SprBlock_Gold";
        public static string SprBlock_Sandrock = "SprBlock_Sandrock";
        public static string SprSmallChest = "SprSmallChest";
        public static string SprSilverSmallChest = "SprSilverSmallChest";
        public static string SprGoldSmallChest = "SprGoldSmallChest";
        public static string SprGoldChest = "SprGoldChest";
        public static string SprCoin = "SprCoin";
        public static string SprCoinSm = "SprCoinSm";
        public static string SprZombieWalk = "SprZombieWalk";
        public static string SprSkeleWalk = "SprSkeleWalk";
        public static string SprBlock_Obsidian = "SprBlock_Obsidian";
        public static string SprCursorArrow = "SprCursorArrow";
        public static string SprCursorCrosshair = "SprCursorCrosshair";
        public static string SprCursorQuestion = "SprCursorQuestion";
        public static string SprCursorSword = "SprCursorSword";
        public static string SprCursorTalk = "SprCursorTalk";
        public static string SprGrassDirt2Tiles = "SprGrassDirt2Tiles";
        public static string SprRockMonsterWalk = "SprRockMonsterWalk";
        public static string SprBlock_GrassDirt = "SprBlock_GrassDirt";
        public static string SprBlock_GreenDot = "SprBlock_GreenDot";
        public static string SprBlock_WaterGrass = "SprBlock_WaterGrass";
        public static string SprBlock_Crag = "SprBlock_Crag";
        public static string SprBlock_DirtBackground = "SprBlock_DirtBackground";
        public static string SprBlock_WaterGrassBack = "SprBlock_WaterGrassBack";
        public static string SprBomb = "SprMine";
        public static string SprBombUI = "SprMineUI";
        public static string SprBowUI = "SprBowUI";
        public static string SprSelectedItemUI = "SprSelectedItemUI";
        public static string SprCheckboxUI = "SprCheckboxUI";
        public static string SprSmoke_Yellow = "SprSmoke_Yellow";
        public static string SprMineshaftExit = "SprMineshaftExit";
        public static string SprMenuUIInventory = "SprMenuUIInventory";
        public static string SprMenuUIMap = "SprMenuUIMap";
        public static string SprMenuUIOption = "SprMenuUIOption";
        public static string SprGlowfish_Swim = "SprGlowfish_Swim";
        public static string SprSeaweed = "SprSeaweed";
        public static string SprBoots = "SprBoots";
        public static string SprGuyOpenChest = "SprGuyOpenChest";
        public static string SprTextBk = "SprTextBk";
        public static string SprSign = "SprSign";
        public static string SprMoreTextCursor = "SprMoreTextCursor";
        public static string SprBackground_Trees = "SprBackground_Trees";
        public static string SprCaveVineTiles = "SprCaveVineTiles";
        public static string SprGlowFlower0 = "SprGlowFlower0";
        public static string SprGlowFlower1 = "SprGlowFlower1";
        public static string SprRock0 = "SprRock0";
        public static string SprRock1 = "SprRock1";
        public static string SprRock2 = "SprRock2";
        public static string SprShroom0 = "SprShroom0";
        public static string SprShroom1 = "SprShroom1";
        public static string SprShroom2 = "SprShroom2";
        public static string SprFlower0 = "SprFlower0";
        public static string SprFlower1 = "SprFlower1";
        public static string SprFlower2 = "SprFlower2";
        public static string SprFlower3 = "SprFlower3";
        public static string SprGrass0 = "SprGrass0";
        public static string SprGrass1 = "SprGrass1";
        public static string SprGrass2 = "SprGrass2";
        public static string SprGrass3 = "SprGrass3";
        public static string SprLever = "SprLever";
        public static string SprDuck = "SprDuck";
        public static string SprParticleCloud = "SprParticleCloud";
        public static string SprParticleRock = "SprParticleRock";
        public static string SprItemPotion = "SprItemBomb";
        public static string SprItemBomb = "SprItemPotion";
        public static string SprHeartUI = "SprHeartUI";
        public static string SprDoor_LR = "SprDoor_LR";
        public static string SprDoor_TB = "SprDoor_TB";
        public static string SprSmallKey = "SprSmallKey";
        public static string SprSmallKeyUI = "SprSmallKeyUI";
        public static string SprMarble = "SprMarble";
        public static string SprMarbleUI = "SprMarbleUI";
        public static string SprBlackNogo = "SprBlackNogo";
        public static string SprGrub = "SprGrub";
        public static string SprGrubLava = "SprGrubLava";
        public static string SprGrubWater = "SprGrubWater";
        public static string SprGrubRock = "SprGrubRock";
        public static string SprBlock_Hedge = "SprBlock_Hedge";
        public static string SprFallThroughDissolve = "SprFallThroughDissolve";
        public static string SprPlantBombDudeAttack = "SprPlantBombDudeAttack";
        public static string SprPlantBombDudeSleep = "SprPlantBombDudeSleep";
        public static string SprPlantBombDudeCover = "SprPlantBombDudeCover";
        public static string SprPlantBombDudeIdle = "SprPlantBombDudeIdle";
        public static string SprPlantBomb = "SprPlantBomb";
        public static string SprGuyHand = "SprGuyHand";
        public static string SprSavePoint = "SprSavePoint";
        public static string SprButtonSwitch = "SprButtonSwitch";
        public static string SprBrazier = "SprBrazier";
        public static string SprSparkle = "SprSparkle";
        public static string SprApple = "SprApple";
        public static string SprCapeGuyWalk = "SprCapeGuyWalk";
        public static string SprCapeGuyTalk = "SprCapeGuyTalk";
        public static string SprGuyDead = "SprGuyDead";
        public const string SprParticleBig = "SprParticleBig";
        public static string SprTree_Doodad = "Tree_Doodad";
        public static string SprGrass_Doodad1 = "Grass_Doodad1";
        public static string SprGrass_Doodad2 = "Grass_Doodad2";
        public static string SprGuyWave = "SprGuyWave";

        

        public static int NoGoTileId = 9999;//**NOT IN THE SPRITE KEY - this is the default "black" background tile so player can't see shit.
        public static int BorderTileId { get; private set; } = 1;// World
        public static int BlockTileId_Rock { get; private set; } = 2;// Wo
        public static int BlockTileId_Copper { get; private set; } = 3;// 
        public static int LadderTileId_R { get; private set; } = 4;// Worl
        public static int BlockTileId_SandRock { get; private set; } = 5;
        public static int BlockTileID_CaveBack { get; private set; } = 6;
        public static int SwordItemTileId { get; private set; } = 7;
        public static int Sun_20Percent { get; private set; } = 8;
        public static int Sun_5Percent { get; private set; } = 9;
        public static int SilverSmallChestTileId { get; private set; } = 10;
        public static int GoldSmallChestTileId { get; private set; } = 11;
        public static int SandRockBackTileId { get; private set; } = 12;
        public static int BigChestTileId { get; private set; } = 13;
        public static int LadderTileId_L { get; private set; } = 14;// Worl
        public static int TreeTileId { get; private set; } = 15;// World.Re
        public static int TorchTileId { get; private set; } = 16;// World.R
        public static int MonsterGrassTileId { get; private set; } = 17;// 
        public static int GuyTileId { get; private set; } = 19;
        public static int SmallChestTileId { get; private set; } = 20;
        public static int PlantBombGuyTileId { get; private set; } = 21;
        public static int ShieldItemTileId { get; private set; } = 22;
        public static int BrazierTileId { get; private set; } = 23;
        public static int GoldChestTileId { get; private set; } = 24;
        public static int FlowerTileId { get; private set; } = 25; //50% water
        public static int ZombieTileId { get; private set; } = 26;
        public static int BootsTileId { get; private set; } = 27;
        public static int Water100TileId { get; private set; } = 28; //100% water
        public static int Water50TileId { get; private set; } = 29; //50% water
        public static int Lava100TileId { get; private set; } = 30; //100% water
        public static int Lava50TileId { get; private set; } = 31; //50% water
        public static int BlockTileId_Obsidian { get; private set; } = 32; //50% water
        public static int TriggerTileId0 { get; private set; } = 33;
        public static int TriggerTileId1 { get; private set; } = 34;
        public static int TriggerTileId2 { get; private set; } = 35;
        public static int TriggerTileId3 { get; private set; } = 36;
        public static int TriggerTileId4 { get; private set; } = 37;
        public static int TriggerTileId5 { get; private set; } = 38;
        public static int TriggerTileId6 { get; private set; } = 39;
        public static int TriggerTileId7 { get; private set; } = 40;
        public static int TriggerTileId8 { get; private set; } = 41;
        public static int TriggerTileId9 { get; private set; } = 42;
        public static int RockMonsterTileId { get; private set; } = 43; //50% water
        public static int RockTileId { get; private set; } = 44; //50% water
        public static int MushroomTileId { get; private set; } = 45; //50% water
        public static int BlockTileId_GrassDirt { get; private set; } = 46;
        public static int LanternTileId { get; private set; } = 47;// World.Res.
        public static int BlockTileId_GreenDot { get; private set; } = 48;
        public static int Key_MonsterTileId { get; private set; } = 49; //50% water
        public static int SkeleTileId { get; private set; } = 50;
        public static int BlockTileId_Hedge { get; private set; } = 51;
        public static int Door_Nolock_TileId { get; private set; } = 52;
        public static int FallThrough_Tile_TileId { get; private set; } = 53;
        public static int Bombable_Tile_TileId { get; private set; } = 54;
        public static int SmallKey_TileId { get; private set; } = 55; //50% water
        public static int Door_Lock_TileId { get; private set; } = 56; //50% water
        public static int Door_Lock_Monster_TileId { get; private set; } = 57; //50% water
        public static int Door_Lock_RightOnly_TileId { get; private set; } = 58; //50% water
        public static int Door_Lock_LeftOnly_TileId { get; private set; } = 59; //50% water
        public static int Doorless_Portal_TileId { get; private set; } = 60; //50% water
        public static int BlockTileId_WaterGrass { get; private set; } = 61;
        public static int BlockTileId_WaterGrassBack { get; private set; } = 62;
        public static int SeaweedTileId { get; private set; } = 63;
        public static int GlowfishTileId { get; private set; } = 64;
        public static int SignTileId_0 { get; private set; } = 66;
        public static int SignTileId_1 { get; private set; } = 67;
        public static int SignTileId_2 { get; private set; } = 68;
        public static int SignTileId_3 { get; private set; } = 69;
        public static int SignTileId_4 { get; private set; } = 70;
        public static int SignTileId_5 { get; private set; } = 71;
        public static int SignTileId_6 { get; private set; } = 72;
        public static int SignTileId_7 { get; private set; } = 73;
        public static int SignTileId_8 { get; private set; } = 74;
        public static int SignTileId_9 { get; private set; } = 75;
        public static int SignTileId_10 { get; private set; } = 76;
        public static int BombTileId { get; private set; } = 77;
        public static int BombPowerupTileId { get; private set; } = 78;
        public static int CaveVineTileId { get; private set; } = 79;
        public static int GlowFlowerTileId { get; private set; } = 80;
        public static int LeverTileId_0 { get; private set; } = 81;//Levers + Gates
        public static int LeverTileId_1 { get; private set; } = 82;
        public static int LeverTileId_2 { get; private set; } = 83;
        public static int LeverTileId_3 { get; private set; } = 84;
        public static int LeverTileId_4 { get; private set; } = 85;
        public static int LeverTileId_5 { get; private set; } = 86;
        public static int LeverTileId_6 { get; private set; } = 87;
        public static int LeverTileId_7 { get; private set; } = 88;
        public static int GateTileId_0 { get; private set; } = 89;
        public static int GateTileId_1 { get; private set; } = 90;
        public static int GateTileId_2 { get; private set; } = 91;
        public static int GateTileId_3 { get; private set; } = 92;
        public static int GateTileId_4 { get; private set; } = 93;
        public static int GateTileId_5 { get; private set; } = 94;
        public static int GateTileId_6 { get; private set; } = 95;
        public static int GateTileId_7 { get; private set; } = 96;
        public static int SlopeTile_BR { get; private set; } = 97;
        public static int SlopeTile_BL { get; private set; } = 98;
        public static int SlopeTile_TR { get; private set; } = 99;
        public static int SlopeTile_TL { get; private set; } = 100;
        public static int EnemSharkTileId { get; private set; } = 101;
        public static int EnemGrubGrassTileId { get; private set; } = 102;
        public static int PowerSwordTileId { get; private set; } = 103;
        public static int CrystalRockTileId { get; private set; } = 104;
        public static int Elevator_A_TileId { get; private set; } = 105;
        public static int Elevator_B_TileId { get; private set; } = 106;
        public static int BugTileId { get; private set; } = 107;
        public static int EnemGrubWaterTileId { get; private set; } = 108;
        public static int SavePointTileId { get; private set; } = 109;
        public static int SwitchButtonTileId { get; private set; } = 110;
        public static int SwitchDoorTileId { get; private set; } = 111;
        public static int SwitchConduitTileId { get; private set; } = 112;
        public static int Tar80TileId { get; private set; } = 114;
        public static int BlockTileId_Crag { get; private set; } = 115;
        public static int EnemGrubLavaTileId { get; private set; } = 116;
        public static int EnemGrubRockTileId { get; private set; } = 117;
        public static int BlockTileID_DirtBackground { get; private set; } = 118;
        public static int BowItemTileId { get; private set; } = 119;
        public static int Spike1_LTileId { get; private set; } = 120;
        public static int Spike1_RTileId { get; private set; } = 121;
        public static int Spike1_TTileId { get; private set; } = 122;
        public static int Spike1_BTileId { get; private set; } = 123;
        public static int Spike2_LTileId { get; private set; } = 124;
        public static int Spike2_RTileId { get; private set; } = 125;
        public static int Spike2_TTileId { get; private set; } = 126;
        public static int Spike2_BTileId { get; private set; } = 127;
        public static int CapeGuyTileId = 128;
        public static int AppleTileId = 129;
        public static int BlockTileID_WoodBackground = 130;

        public static string Spr_Sky_Level0 = "Spr_Sky_Level0";
        public static string Spr_Sky_Level1 = "Spr_Sky_Level1";
        public static string Spr_Sky_Level2 = "Spr_Sky_Level2";
        public static string Spr_Sky_Level3 = "Spr_Sky_Level3";
        public static string Spr_Sky_Level4 = "Spr_Sky_Level4";

        public static int Sky_Level0 = 131;
        public static int Sky_Level1 = 132;
        public static int Sky_Level2 = 133;
        public static int Sky_Level3 = 134;
        public static int Sky_Level4 = 135;
        public static int Tree_Doodad = 136;
        public static int Grass_Doodad1 = 137;
        public static int Grass_Doodad2 = 138;
        public static int Star_0 = 139;

        public static void Load(ContentManager c, GraphicsDevice d)
        {
            Res.Content = c;
            Res.Audio = new Audio();
            Res.Tiles = new Tiles();
            Font = Content.Load<SpriteFont>("Font");
            Font2 = Content.Load<SpriteFont>("Font2");
            Tiles.Texture = Content.Load<Texture2D>("sprites");

            //**NOGO is alwasy a seamless tile
            //Mesh the grass with the various rocks
            List<int> SeamlessRock = new List<int>() {
                NoGoTileId
                ,BlockTileId_Rock
                ,BlockTileId_Copper
                ,BlockTileId_SandRock
                ,BlockTileId_GrassDirt
                ,BlockTileId_GreenDot
            };

            //Create3x3Tiles(SprGrassBlockTiles, 12, 0, 6, 9, GrassTileId, SeamlessRock);
            //Create3x3Tiles(SprCaveBackgroundBlockTiles, 18 + 6 * 0, 0, 6, 9, BlockTileID_CaveBack, new List<int> { NoGoTileId, BlockTileID_WoodBackground, BlockTileID_CaveBack, BlockTileId_WaterGrassBack, BlockTileID_DirtBackground });
            //Create3x3Tiles(SprWoodBackgroundBlockTiles, 18 + 6 * 1, 0, 6, 9, BlockTileID_WoodBackground, new List<int> { NoGoTileId, BlockTileID_WoodBackground, BlockTileID_CaveBack, BlockTileId_WaterGrassBack, BlockTileID_DirtBackground });
            //Create3x3Tiles(SprBlock_Sandrock, 18 + 6 * 2, 0, 6, 9, BlockTileId_SandRock, SeamlessRock);
            Create3x3Tiles(SprBlock_GrassDirt, 15, 0, 6, 9, BlockTileId_GrassDirt, SeamlessRock);
            //Create3x3Tiles(SprBlock_GreenDot, 18 + 6 * 4, 0, 6, 9, BlockTileId_GreenDot, new List<int>() { NoGoTileId, BlockTileId_GreenDot, BlockTileId_GrassDirt });
            //Create3x3Tiles(SprBlock_WaterGrass, 18 + 6 * 5, 0, 6, 9, BlockTileId_WaterGrass, new List<int>() { NoGoTileId, BlockTileId_WaterGrass, BlockTileId_GrassDirt });
            //Create3x3Tiles(SprBlock_WaterGrassBack, 18 + 6 * 6, 0, 6, 9, BlockTileId_WaterGrassBack, new List<int>() { NoGoTileId, BlockTileId_WaterGrassBack, BlockTileID_CaveBack });

            ////ice glass
            //Create3x3Tiles(SprBlock_Hedge, 30 + 6 * 1, 9, 6, 9, BlockTileId_Hedge, new List<int>() { NoGoTileId, BlockTileId_Hedge });
            ////Old grass tiles
            //Create3x3Tiles(SprSandRockBack, 30 + 6 * 3, 9, 6, 9, BlockTileID_CaveBack, new List<int> { NoGoTileId, SandRockBackTileId });
            //Create3x3Tiles(SprBlock_Crag, 30 + 6 * 4, 9, 6, 9, BlockTileId_Crag, new List<int>() { NoGoTileId, BlockTileId_Crag });

            //Create3x3Tiles(SprBlock_DirtBackground, 30 + 6 * 0, 18, 6, 9, BlockTileID_DirtBackground, new List<int> { NoGoTileId, BlockTileID_WoodBackground, BlockTileID_CaveBack, BlockTileId_WaterGrassBack, BlockTileID_DirtBackground });
            Tiles.AddSprite(Spr_Sky_Level0, new List<Rectangle>() { new Rectangle(5, 2, 1, 1), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(Spr_Sky_Level1, new List<Rectangle>() { new Rectangle(9, 2, 1, 1), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(Spr_Sky_Level2, new List<Rectangle>() { new Rectangle(12, 2, 1, 1), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(Spr_Sky_Level3, new List<Rectangle>() { new Rectangle(13, 2, 1, 1), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(Spr_Sky_Level4, new List<Rectangle>() { new Rectangle(14, 2, 1, 1), }, 0.0f, Tiling.Single);

            Tiles.AddSprite(SprCursorArrow, new List<Rectangle>() { new Rectangle(7, 3, 1, 1), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprCursorCrosshair, new List<Rectangle>() { new Rectangle(11, 2, 1, 1), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprCursorQuestion, new List<Rectangle>() { new Rectangle(12, 2, 1, 1), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprCursorTalk, new List<Rectangle>() { new Rectangle(12, 1, 1, 1), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprCursorSword, new List<Rectangle>() { new Rectangle(13, 2, 1, 1), }, 0.0f, Tiling.Single);

            Tiles.AddSprite(SprBow, new List<Rectangle>() { new Rectangle(12, 3, 1, 1) }, 0.6f);
            Tiles.AddSprite(SprArrow, new List<Rectangle>() { new Rectangle(13, 3, 1, 1), new Rectangle(15, 2, 1, 1) }, 0.6f);
            Tiles.AddSprite(SprPlinthBow, new List<Rectangle>() { new Rectangle(14, 3, 1, 1) }, 0.6f);

            Tiles.AddSprite(SprPlinthEmpty, new List<Rectangle>() { new Rectangle(14, 5, 1, 1) }, 0.6f);
            Tiles.AddSprite(SprPlinthShield, new List<Rectangle>() { new Rectangle(15, 5, 1, 1), new Rectangle(14, 5, 1, 1) }, 0.6f);
            Tiles.AddSprite(SprPlinthSword, new List<Rectangle>() { new Rectangle(16, 5, 1, 1), new Rectangle(14, 5, 1, 1) }, 0.6f);


            Tiles.AddSprite(SprBomb, new List<Rectangle>() { new Rectangle(11, 9, 1, 1), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprBombUI, new List<Rectangle>() { new Rectangle(11, 10, 2, 2), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprMenuUIInventory, new List<Rectangle>() { new Rectangle(11, 12, 5, 3), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprMenuUIMap, new List<Rectangle>() { new Rectangle(11, 20, 5, 3), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprMenuUIOption, new List<Rectangle>() { new Rectangle(11, 23, 5, 3), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprSmoke_Yellow, new List<Rectangle>() { new Rectangle(12, 9, 1, 1), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprBowUI, new List<Rectangle>() { new Rectangle(15, 0, 2, 2), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprSelectedItemUI, new List<Rectangle>() { new Rectangle(15, 2, 2, 2), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprCheckboxUI, new List<Rectangle>() { new Rectangle(9, 22, 1, 1), new Rectangle(10, 22, 1, 1), }, 0.0f, Tiling.Single);

            Tiles.AddSprite(SprTree_Doodad, new List<Rectangle>() { new Rectangle(0, 1, 1, 1) }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprGrass_Doodad1, new List<Rectangle>() { new Rectangle(1, 1, 1, 1) }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprGrass_Doodad2, new List<Rectangle>() { new Rectangle(2, 1, 1, 1)}, 0.0f, Tiling.Single);


            //BLOCK
            Tiles.AddSprite(SprBlock_Rock, new List<Rectangle>() {
               new Rectangle(5, 4, 1, 1),
            }, 0.0f, Tiling.Single, BlockTileId_Rock);
            Tiles.AddSprite(SprBlock_Copper, new List<Rectangle>() {
               new Rectangle(6, 4, 1, 1),
            }, 0.0f, Tiling.Single, BlockTileId_Copper);

            Tiles.AddSprite(SprBlock_Obsidian, new List<Rectangle>() {
               new Rectangle(9, 4, 1, 1),
            }, 0.0f, Tiling.Single, BlockTileId_Obsidian);

            Tiles.AddSprite(SprCoinSm, new List<Rectangle>() {
               new Rectangle(0, 4, 1, 1),
            }, 0.0f);
            Tiles.AddSprite(SprCoin, new List<Rectangle>() {
               new Rectangle(1, 4, 1, 1),
            }, 0.0f);

            List<Rectangle> LadderTiles = new List<Rectangle>() {
                new Rectangle(2, 6, 1, 1),
                new Rectangle(3, 6, 1, 1),
                new Rectangle(4, 6, 1, 1),
                new Rectangle(5, 6, 1, 1)
            };
            Tiles.AddSprite(SprLadderTiles, LadderTiles, 0, Tiling.Vertical1x3);
            //Tiles.AddSprite(SprLadderTiles + "L", LadderTiles, 0, Tiling.Vertical1x3, LadderTileId_L);//This is a duplicate of R

            //SprGuyItemHeldWalk

            Tiles.AddSprite(SprGuyItemHeldWalk, new List<Rectangle>() {
               new Rectangle(0, 0, 1, 1),
                new Rectangle(1, 0, 1, 1),
                new Rectangle(2, 0, 1, 1),
                new Rectangle(1, 0, 1, 1),
            }, 0.8f);
            Tiles.AddSprite(SprGuyWalk, new List<Rectangle>() {
               new Rectangle(0, 0, 1, 1),
                new Rectangle(1, 0, 1, 1),
                new Rectangle(0, 0, 1, 1),
                new Rectangle(2, 0, 1, 1),
            }, 0.8f);

            Tiles.AddSprite(SprGuyJump, new List<Rectangle>() {
               new Rectangle(1, 0, 1, 1),
            }, 0.8f);
            Tiles.AddSprite(SprGuyHang, new List<Rectangle>() {
               new Rectangle(8, 0, 1, 1),
               new Rectangle(9, 0, 1, 1),
            }, 0.8f);
            Tiles.AddSprite(SprGuyMount, new List<Rectangle>() {
               new Rectangle(10, 0, 1, 1),
               new Rectangle(11, 0, 1, 1),
            }, 0.2f);
            Tiles.AddSprite(SprGuyClimb, new List<Rectangle>() {
               new Rectangle(4, 1, 1, 1),
               new Rectangle(5, 1, 1, 1),
               new Rectangle(6, 1, 1, 1),
               new Rectangle(5, 1, 1, 1),
            }, 0.8f);
            Tiles.AddSprite(SprGuyCurl, new List<Rectangle>() {
               new Rectangle(3, 0, 1, 1),
            }, 0.0f);
            Tiles.AddSprite(SprGuyWalkAttack, new List<Rectangle>() {
               new Rectangle(2, 1, 1, 1),
               new Rectangle(1, 1, 1, 1),
               new Rectangle(2, 1, 1, 1),
               new Rectangle(3, 1, 1, 1),
            }, 0.5f);
            Tiles.AddSprite(SprGuyCrouchAttack, new List<Rectangle>() {
               new Rectangle(4, 2, 1, 1),
               new Rectangle(3, 2, 1, 1),
               new Rectangle(4, 2, 1, 1),
               new Rectangle(5, 2, 1, 1),
            }, 0.5f);
            Tiles.AddSprite(SprZombieWalk, new List<Rectangle>() {
               new Rectangle(3, 13, 1, 1),
               new Rectangle(2, 13, 1, 1),
               new Rectangle(3, 13, 1, 1),
               new Rectangle(4, 13, 1, 1),
            }, 0.8f);
            Tiles.AddSprite(SprSkeleWalk, new List<Rectangle>() {
               new Rectangle(3+6, 13, 1, 1),
               new Rectangle(2+6, 13, 1, 1),
               new Rectangle(3+6, 13, 1, 1),
               new Rectangle(4+6, 13, 1, 1),
            }, 0.8f);
            Tiles.AddSprite(SprRockMonsterWalk, new List<Rectangle>() {
               new Rectangle(8, 9, 1, 1),
               new Rectangle(9, 9, 1, 1),
            }, 0.8f);
            Tiles.AddSprite(SprHorizon, new List<Rectangle>() {
               new Rectangle(16, 9, 1, 6),
            }, 0.4f);

            Tiles.AddSprite(SprGrass0, new List<Rectangle>() { new Rectangle(4, 8, 1, 1) }, 0f, Tiling.Single, FlowerTileId);
            Tiles.AddSprite(SprGrass1, new List<Rectangle>() { new Rectangle(5, 8, 1, 1) }, 0f, Tiling.Single, FlowerTileId);
            Tiles.AddSprite(SprGrass2, new List<Rectangle>() { new Rectangle(6, 8, 1, 1) }, 0f, Tiling.Single, FlowerTileId);
            Tiles.AddSprite(SprGrass3, new List<Rectangle>() { new Rectangle(7, 8, 1, 1) }, 0f, Tiling.Single, FlowerTileId);


            Tiles.AddSprite(SprRockMonsterTiles, new List<Rectangle>() {
                new Rectangle(8, 8, 1, 1),
                new Rectangle(9, 8, 1, 1),
                new Rectangle(10, 8, 1, 1),
                new Rectangle(11, 8, 1, 1),
            }, 0, Tiling.Horizontal3x1);
            Tiles.AddSprite(SprTreeTiles, new List<Rectangle>() {
                new Rectangle(0, 8, 1, 1),
                new Rectangle(1, 8, 1, 1),
                new Rectangle(2, 8, 1, 1),
                new Rectangle(3, 8, 1, 1),
            }, 0, Tiling.Vertical1x3, TreeTileId);

            Tiles.AddSprite(SprCaveVineTiles, new List<Rectangle>() {
                new Rectangle(8, 15, 1, 1),
                new Rectangle(9, 15, 1, 1),
                new Rectangle(10, 15, 1, 1),
                new Rectangle(11, 15, 1, 1),
            }, 0, Tiling.Vertical1x3, CaveVineTileId);

            Tiles.AddSprite(SprTileDirt, new List<Rectangle>() {
               new Rectangle(2, 2, 1, 1),
            }, 0.4f);
            //Tiles.AddSprite(SprNoGo, new List<Rectangle>() {
            //   new Rectangle(2, 1, 1, 1),
            //}, 0.4f, Tiling.Single, NoGoTileId);
            Tiles.AddSprite(SprCrack, new List<Rectangle>() {
               new Rectangle(2, 3, 1, 1),
               new Rectangle(3, 3, 1, 1),
               new Rectangle(4, 3, 1, 1),
               new Rectangle(5, 3, 1, 1),
            }, 0.4f);
            Tiles.AddSprite(SprParticleSmall, new List<Rectangle>() { new Rectangle(6, 0, 1, 1), }, 0.4f);
            Tiles.AddSprite(SprParticleBig, new List<Rectangle>() { new Rectangle(7, 0, 1, 1), }, 0.4f);
            Tiles.AddSprite(SprTileCoal, new List<Rectangle>() { new Rectangle(3, 4, 1, 1), }, 0.4f);
            Tiles.AddSprite(SprTileCopper, new List<Rectangle>() { new Rectangle(4, 4, 1, 1), }, 0.4f);
            Tiles.AddSprite(SprTileSilver, new List<Rectangle>() { new Rectangle(1, 0, 1, 1), }, 0.4f);
            Tiles.AddSprite(SprTileGold, new List<Rectangle>() { new Rectangle(2, 0, 1, 1), }, 0.4f);

            Tiles.AddSprite(SprSword, new List<Rectangle>() {
                new Rectangle(7, 5, 1, 1),//Normal
                new Rectangle(13, 4, 1, 1),//Tar
                new Rectangle(14, 4, 1, 1),//water
                new Rectangle(15, 4, 1, 1),//lava
                new Rectangle(16, 4, 1, 1),//obsidian

            }, 0.4f);
            Tiles.AddSprite(SprSwordItem, new List<Rectangle>() { new Rectangle(8, 5, 1, 1), }, 0.4f);
            Tiles.AddSprite(SprPowerSword, new List<Rectangle>() { new Rectangle(9, 5, 1, 1), }, 0.4f);
            Tiles.AddSprite(SprPowerSwordItem, new List<Rectangle>() { new Rectangle(10, 5, 1, 1), }, 0.4f);
            Tiles.AddSprite(SprPowerSwordProjectile, new List<Rectangle>() { new Rectangle(11, 5, 1, 1), }, 0.4f);
            Tiles.AddSprite(SprShield, new List<Rectangle>() { new Rectangle(12, 5, 1, 1) }, 0.6f);

            Tiles.AddSprite(SprTorch, new List<Rectangle>() { new Rectangle(0, 20, 1, 1), new Rectangle(1, 20, 1, 1), }, 1.7f, Tiling.Single, TorchTileId);
            Tiles.AddSprite(SprTorchOut, new List<Rectangle>() { new Rectangle(2, 20, 1, 1) }, 0, Tiling.Single, TorchTileId);

            Tiles.AddSprite(SprTorchWall, new List<Rectangle>() { new Rectangle(4, 21, 1, 1), new Rectangle(5, 21, 1, 1), }, 1.7f, Tiling.Single, TorchTileId);
            Tiles.AddSprite(SprTorchOutWall, new List<Rectangle>() { new Rectangle(6, 21, 1, 1) }, 0, Tiling.Single, TorchTileId);

            Tiles.AddSprite(SprLantern, new List<Rectangle>() { new Rectangle(8, 3, 1, 1), new Rectangle(9, 3, 1, 1), }, 2.1f, Tiling.Single, TorchTileId);
            Tiles.AddSprite(SprBigChest, new List<Rectangle>() {
                new Rectangle(6, 6, 1, 1),
                new Rectangle(7, 6, 1, 1) }, 0, Tiling.Single, BigChestTileId);
            Tiles.AddSprite(SprSmallChest, new List<Rectangle>() {
                new Rectangle(8, 6, 1, 1),
                new Rectangle(9, 6, 1, 1) }, 0, Tiling.Single, SmallChestTileId);
            Tiles.AddSprite(SprSilverSmallChest, new List<Rectangle>() {
                new Rectangle(9, 7, 1, 1),
                new Rectangle(10, 7, 1, 1) }, 0, Tiling.Single, SilverSmallChestTileId);
            Tiles.AddSprite(SprGoldSmallChest, new List<Rectangle>() {
                new Rectangle(13, 9, 1, 1),
                new Rectangle(14, 9, 1, 1) }, 0, Tiling.Single, GoldSmallChestTileId);

            Tiles.AddSprite(SprGoldChest, new List<Rectangle>() {
                new Rectangle(6, 7, 1, 1),
                new Rectangle(7, 7, 1, 1) }, 0, Tiling.Single, GoldChestTileId);
            Tiles.AddSprite(SprMountainBackdrop, new List<Rectangle>() { new Rectangle(17, 8, 8, 8) }, 0f);
            Tiles.AddSprite(SprGlowfish_Swim, new List<Rectangle>() { new Rectangle(2, 14, 1, 1), new Rectangle(3, 14, 1, 1) }, .5f);
            Tiles.AddSprite(SprSeaweed, new List<Rectangle>() { new Rectangle(13, 10, 1, 1), new Rectangle(14, 10, 1, 1) }, 2f);

            Tiles.AddSprite(SprBoots, new List<Rectangle>() { new Rectangle(6, 12, 1, 1) }, 2f);
            Tiles.AddSprite(SprGuyOpenChest, new List<Rectangle>() {
               new Rectangle(7, 1, 1, 1),
               new Rectangle(8, 1, 1, 1),
            }, 0.4f);
            Tiles.AddSprite(SprTextBk, new List<Rectangle>() {
               new Rectangle(12, 0, 1, 1),
               new Rectangle(13, 0, 1, 1),
               new Rectangle(14, 0, 1, 1),
               new Rectangle(12, 1, 1, 1),
               new Rectangle(13, 1, 1, 1),
               new Rectangle(14, 1, 1, 1),
            }, 0.4f);
            Tiles.AddSprite(SprSign, new List<Rectangle>() { new Rectangle(10, 6, 1, 1) }, 0f);

            Tiles.AddSprite(SprApple, new List<Rectangle>() { new Rectangle(11, 6, 1, 1) }, 0.92f);//Attack Hand

            Tiles.AddSprite(SprMoreTextCursor, new List<Rectangle>() {
                new Rectangle(8, 0, 1, 1),
                new Rectangle(9, 0, 1, 1),
                new Rectangle(10, 0, 1, 1),
                new Rectangle(11, 0, 1, 1),
                new Rectangle(10, 0, 1, 1),
                new Rectangle(9, 0, 1, 1),
            }, .9f);
            Tiles.AddSprite(SprBackground_Trees, new List<Rectangle>() {
                new Rectangle(17, 17, 8, 7)
            }, 0f);

            Tiles.AddSprite(SprGlowFlower0, new List<Rectangle>() { new Rectangle(0, 17, 1, 1), new Rectangle(1, 17, 1, 1) }, 0f, Tiling.Single, GlowFlowerTileId);
            Tiles.AddSprite(SprGlowFlower1, new List<Rectangle>() { new Rectangle(2, 17, 1, 1), new Rectangle(3, 17, 1, 1) }, 0f, Tiling.Single, GlowFlowerTileId);

            Tiles.AddSprite(SprRock0, new List<Rectangle>() { new Rectangle(7, 9, 1, 1) }, 0f, Tiling.Single, RockTileId);
            Tiles.AddSprite(SprRock1, new List<Rectangle>() { new Rectangle(7, 10, 1, 1) }, 0f, Tiling.Single, RockTileId);
            Tiles.AddSprite(SprRock2, new List<Rectangle>() { new Rectangle(7, 11, 1, 1) }, 0f, Tiling.Single, RockTileId);
            Tiles.AddSprite(SprShroom0, new List<Rectangle>() { new Rectangle(10, 9, 1, 1) }, 0f, Tiling.Single, MushroomTileId);
            Tiles.AddSprite(SprShroom1, new List<Rectangle>() { new Rectangle(10, 10, 1, 1) }, 0f, Tiling.Single, MushroomTileId);
            Tiles.AddSprite(SprShroom2, new List<Rectangle>() { new Rectangle(10, 11, 1, 1) }, 0f, Tiling.Single, MushroomTileId);
            Tiles.AddSprite(SprFlower0, new List<Rectangle>() { new Rectangle(8, 10, 1, 1) }, 0f, Tiling.Single, FlowerTileId);
            Tiles.AddSprite(SprFlower1, new List<Rectangle>() { new Rectangle(8, 11, 1, 1) }, 0f, Tiling.Single, FlowerTileId);
            Tiles.AddSprite(SprFlower2, new List<Rectangle>() { new Rectangle(9, 10, 1, 1) }, 0f, Tiling.Single, FlowerTileId);
            Tiles.AddSprite(SprFlower3, new List<Rectangle>() { new Rectangle(9, 11, 1, 1) }, 0f, Tiling.Single, FlowerTileId);

            Tiles.AddSprite(SprGuyWave, new List<Rectangle>() { new Rectangle(7, 1, 1, 1), new Rectangle(8, 1, 1, 1) }, 0.9f, Tiling.Single);


            Tiles.AddSprite(SprLever, new List<Rectangle>() { new Rectangle(9, 12, 1, 1), new Rectangle(10, 12, 1, 1) }, 0f, Tiling.Single);

            Tiles.AddSprite(SprDuck, new List<Rectangle>() { new Rectangle(1, 5, 1, 1), new Rectangle(0, 5, 1, 1), new Rectangle(1, 5, 1, 1), new Rectangle(2, 5, 1, 1), }, 1f, Tiling.Single);

            Tiles.AddSprite(SprParticleCloud, new List<Rectangle>() { new Rectangle(12, 9, 1, 1) }, 0f, Tiling.Single);
            Tiles.AddSprite(SprItemPotion, new List<Rectangle>() { new Rectangle(7, 4, 1, 1) }, 0f, Tiling.Single);
            Tiles.AddSprite(SprItemBomb, new List<Rectangle>() { new Rectangle(8, 4, 1, 1) }, 0f, Tiling.Single);
            Tiles.AddSprite(SprHeartUI, new List<Rectangle>() { new Rectangle(10, 3, 2, 2) }, 0f, Tiling.Single);

            // Tiles.AddSprite(SprBackDoorWood_Open,new List<Rectangle>() { new Rectangle(1, 6, 2, 2) }, 0f, Tiling.Single);
            // Tiles.AddSprite(SprBackDoorWood_Close  , new List<Rectangle>() {new Rectangle(0, 6, 2, 2)}  , 0f, Tiling.Single);
            Tiles.AddSprite(SprDoor_LR, new List<Rectangle>() {
                new Rectangle(3, 19, 1, 1),//open
                new Rectangle(2, 19, 1, 1),//close
                new Rectangle(7, 19, 1, 1),//lock
                new Rectangle(8, 19, 1, 1),//gold ?? lock
                 new Rectangle(3, 20, 1, 1),//electronic
                new Rectangle(7, 21, 1, 1),//metal close
                new Rectangle(8, 21, 1, 1),//metal open
            }, 0f, Tiling.Single);

            Tiles.AddSprite(SprDoor_TB, new List<Rectangle>() {
                new Rectangle(5, 19, 1, 1),//open
                new Rectangle(4, 19, 1, 1),//close
                new Rectangle(11, 19, 1, 1),//lock
                new Rectangle(12, 19, 1, 1),//gold ?? lock
                new Rectangle(4, 20, 1, 1),//electronic
                                new Rectangle(9, 21, 1, 1),//metal close
                new Rectangle(10, 21, 1, 1),//metal open
            }, 0f, Tiling.Single);


            Tiles.AddSprite(SprSmallKeyUI, new List<Rectangle>() {
                new Rectangle(13, 18, 2, 2),//gold ?? lock
            }, 0f, Tiling.Single);

            Tiles.AddSprite(SprMarble, new List<Rectangle>() { new Rectangle(12, 15, 1, 1) }, 0f, Tiling.Single);
            Tiles.AddSprite(SprMarbleUI, new List<Rectangle>() { new Rectangle(12, 16, 2, 2) }, 0f, Tiling.Single);

            Tiles.AddSprite(SprSmallKey, new List<Rectangle>() { new Rectangle(6, 19, 1, 1) }, 0f, Tiling.Single);
            Tiles.AddSprite(SprBlackNogo, new List<Rectangle>() { new Rectangle(5, 0, 1, 1) }, 0f, Tiling.Single);

            Tiles.AddSprite(SprGrub, new List<Rectangle>() { new Rectangle(0, 19, 1, 1), new Rectangle(1, 19, 1, 1) }, .5f, Tiling.Single);
            Tiles.AddSprite(SprGrubLava, new List<Rectangle>() {
                new Rectangle(0, 21, 1, 1),
                new Rectangle(1, 21, 1, 1),
                new Rectangle(2, 21, 1, 1),
                new Rectangle(3, 21, 1, 1),
            }, .5f, Tiling.Single);
            Tiles.AddSprite(SprGrubWater, new List<Rectangle>() {
                new Rectangle(0, 22, 1, 1),
                new Rectangle(1, 22, 1, 1) ,
                new Rectangle(2, 22, 1, 1),
                new Rectangle(3, 22, 1, 1) ,
                new Rectangle(2, 22, 1, 1) ,
                new Rectangle(1, 22, 1, 1) },
                .5f, Tiling.Single);
            Tiles.AddSprite(SprGrubRock, new List<Rectangle>() { new Rectangle(0, 23, 1, 1), new Rectangle(1, 23, 1, 1) }, .5f, Tiling.Single);


            Tiles.AddSprite(SprFallThroughDissolve, new List<Rectangle>() {
                new Rectangle(12, 8, 1, 1),
                new Rectangle(13, 8, 1, 1),
                new Rectangle(14, 8, 1, 1),
                new Rectangle(15, 8, 1, 1),
                }, .3f, Tiling.Single);
            Tiles.AddSprite(SprGuyFall, new List<Rectangle>() { new Rectangle(9, 1, 1, 1), new Rectangle(10, 1, 1, 1) }, 0.6f);

            //plant bomb guy / bomb
            Tiles.AddSprite(SprPlantBombDudeIdle, new List<Rectangle>() { new Rectangle(4, 15, 1, 1) }, 0.6f);
            Tiles.AddSprite(SprPlantBombDudeAttack, new List<Rectangle>() {
                new Rectangle( 5,15, 1, 1),
                new Rectangle( 6,15, 1, 1),
                new Rectangle( 7,15, 1, 1)
            }, 0.2f);
            Tiles.AddSprite(SprPlantBombDudeSleep, new List<Rectangle>() { new Rectangle(3, 16, 1, 1) }, 0.6f);
            Tiles.AddSprite(SprPlantBomb, new List<Rectangle>() { new Rectangle(4, 16, 1, 1) }, 0.6f);
            Tiles.AddSprite(SprPlantBombDudeCover, new List<Rectangle>() { new Rectangle(5, 16, 1, 1), new Rectangle(6, 16, 1, 1), new Rectangle(7, 16, 1, 1) }, 0.2f);


            Tiles.AddSprite(SprGuyHand, new List<Rectangle>() { new Rectangle(11, 1, 1, 1) }, 0.0f);//Attack Hand
            Tiles.AddSprite(SprSavePoint, new List<Rectangle>() { new Rectangle(9, 19, 1, 1), new Rectangle(10, 19, 1, 1) }, 0.0f);//Attack Hand


            Tiles.AddSprite(SprButtonSwitch, new List<Rectangle>() { new Rectangle(5, 20, 1, 1), new Rectangle(6, 20, 1, 1) }, 0.0f);//Attack Hand
            Tiles.AddSprite(SprSparkle, new List<Rectangle>() { new Rectangle(7, 20, 1, 1), new Rectangle(6, 20, 1, 1) }, 0.0f);//Attack Hand



            Tiles.AddSprite(SprBrazier, new List<Rectangle>() { new Rectangle(12, 6, 1, 1), new Rectangle(13, 6, 1, 1), new Rectangle(14, 6, 1, 1) }, 0.92f);//Attack Hand

            Tiles.AddSprite(SprCapeGuyWalk, new List<Rectangle>() {
                new Rectangle(10, 0, 1, 1),
                new Rectangle(9, 0, 1, 1),
                new Rectangle(8, 0, 1, 1),
                new Rectangle(9, 0, 1, 1)
            }
            , 0.92f);//Attack Hand
            Tiles.AddSprite(SprCapeGuyTalk, new List<Rectangle>() {
                new Rectangle(11, 0, 1, 1),
                new Rectangle(12, 0, 1, 1)
            }
            , 0.82f);//Attack Hand

            Tiles.AddSprite(SprGuyDead, new List<Rectangle>() {
                new Rectangle(3, 0, 1, 1),
            }
            , 0.82f);//Attack Hand



            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxMine));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxWhack));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxJump));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxGrassMove));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxGrassCut));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxBattleStart));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxClimb));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxLand));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxChestOpen));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxChop));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxCoinGet));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxSizzle));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxEnterWater));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxExitWater));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxZombGrowl0));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxZombGrowl1));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxZombHit0));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxZombHit1));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxDieSquishy));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxSwordSwing));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxLavaBall));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxExitStairs));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxBootsJump));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxGetKeyItem));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxShowNewItem));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxShowMenu));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxHideMenu));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxTextBlip));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxBombexplode));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxTakeOutItem));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxBombsizzle));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxThrow));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPickupItem));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPop1));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPop2));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPop2));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxMarbleDrop));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxDoorOpen));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxDoorLocked));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxDoorUnlock));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxDoorClose));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxGetPowerup));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxGetPowerupLong));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPowerSwordChargeBase));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPowerSwordChargeFinal));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPowerSwordChargeRise));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPowerSwordShoot));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxTorchout));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxFallthrough));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxShieldOut));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPlantBombGuyDamage));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPlantBombGuyHide));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPlantBombGuyUnHide));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPlantBombExplode));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxShieldDeflect));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxSavePoint1));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxSavePoint2));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxSwitchButtonPress));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxPuzzleSolved));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxSwordLightFire));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxElectronicNogo));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxGrubDie));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxGrubHit));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxChangeSubweapon));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxDrawBow));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxArrowShoot));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxNoArrowShoot));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxDead));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxContinue));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxDuckQuack));

            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxSlide));

            Audio.Songs.Add(Content.Load<Microsoft.Xna.Framework.Media.Song>(IttyBitty8));
        }


        public static void Create3x3Tiles(string name, int x0, int y0, int w, int h, int TileId, List<int> SeamlessIds)
        {
            //Creates a seamless set of tiles for a 3x3 grid configuration
            List<Rectangle> rects = new List<Rectangle>();

            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    rects.Add(new Rectangle(x0 + x, y0 + y, 1, 1));
                }
            }

            Sprite s = Tiles.AddSprite(name, rects, 0, Tiling.Grid3x3, TileId);
            s.SeamlessIds = SeamlessIds;
        }


    }
}
