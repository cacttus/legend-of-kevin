using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class Res
    {
        public Audio Audio { get; private set; }
        public Tiles Tiles { get; private set; }
        public SpriteFont Font { get; private set; }
        public SpriteFont Font2 { get; private set; }
        ContentManager Content;

        public string SfxMine = "mine";
        public string SfxWhack = "whack";
        public string SfxJump = "jump";
        public string SfxChestOpen = "chestopen";
        public string SfxGrassMove = "grassmove";
        public string SfxBattleStart = "battlestart";
        public string SfxClimb = "climb";
        public string SfxLand = "land";
        public string SfxChop = "chop";
        public string SfxGrassCut = "grasscut";
        public string SfxCoinGet = "coinget";
        public string SfxSizzle = "sizzle";
        public string SfxEnterWater = "enterwater";
        public string SfxExitWater = "exitwater";
        public string SfxZombHit0 = "zomb_hit_0";
        public string SfxZombHit1 = "zomb_hit_1";
        public string SfxZombGrowl0 = "zomb_growl_0";
        public string SfxZombGrowl1 = "zomb_growl_1";
        public string SfxDieSquishy = "die_squishy";
        public string SfxLavaBall = "lavaball";
        public string SfxSwordSwing = "axswing";
        public string SfxExitStairs = "exit_stairs";
        public string SfxBootsJump = "boots_jump";
        public string SfxGetKeyItem = "getkeyitem";
        public string SfxShowNewItem = "shownewitem";
        public string SfxShowMenu = "showmenu";
        public string SfxHideMenu = "hidemenu";
        public string SfxTextBlip = "textblip";
        public string SfxBombexplode = "bombexplode";
        public string SfxTakeOutItem = "bombstart";
        public string SfxBombsizzle = "bombsizzle";
        public string SfxThrow = "throw";
        public string SfxPickupItem = "pickupitem";
        public string SfxPop1 = "pop1";
        public string SfxPop2 = "pop2";
        public string SfxPop3 = "pop3";
        public string SfxMarbleDrop = "marbledrop";
        public string SfxDoorOpen = "door_open";
        public string SfxDoorLocked = "door_locked";
        public string SfxDoorUnlock = "door_unlock";
        public string SfxDoorClose = "door_close";
        public string SfxGetPowerupLong = "get_powerup_long";
        public string SfxGetPowerup = "get_powerup";
        public string SfxPowerSwordChargeBase = "powerswordcharge";
        public string SfxPowerSwordChargeFinal = "powerswordchargefinal";
        public string SfxPowerSwordChargeRise = "powerswordchargerise";
        public string SfxPowerSwordShoot = "powerswordshoot";
        public string SfxTorchout = "torchout";
        public string SfxFallthrough = "fallthrough";
        public string SfxShieldOut = "shieldout";
        public string SfxPlantBombGuyDamage = "plantbombguy_damage";
        public string SfxPlantBombGuyHide = "plantbombguy_hide";
        public string SfxPlantBombGuyUnHide = "plantbombguy_unhide";
        public string SfxPlantBombExplode = "plantbomb_explode";
        public string SfxShieldDeflect = "shield_deflect";
        public string SfxSavePoint1 = "save_point_1";
        public string SfxSavePoint2 = "save_point_2";
        public string SfxSwitchButtonPress = "switch_button_press";
        public string SfxPuzzleSolved = "good_thing";
        public string SfxSwordLightFire = "sword_light_fire";
        public string SfxElectronicNogo = "electronic_nogo";
        public string SfxGrubDie = "grub_die";
        public string SfxGrubHit = "grub_hit";
        public string SfxChangeSubweapon = "change_subweapon";
        public string SfxDrawBow = "draw_bow";
        public string SfxArrowShoot = "arrow_shoot";
        public string SfxNoArrowShoot = "bow_noarrow_shoot";
        public string SfxDead = "dead";
        public string SfxContinue = "continue";
        //--------Sprites
        //--------Sprites
        //--------Sprites
        //--------Sprites
        public string SprGuyWalk = "SprGuyWalk";
        //public string SprGuyCrouch = "SprGuyCrouch";
        public string SprGuyItemHeldWalk = "SprGuyItemHeldWalk";
        public string SprGuyJump = "SprGuyJump";
        public string SprGuyHang = "SprGuyHang";
        public string SprGuyClimb = "SprGuyClimb";
        public string SprGuyMount = "SprGuyMount";
        public string SprGuyCurl = "SprGuyCurl";
        public string SprGuyWalkAttack = "SprGuyWalkAttack";
        public string SprGuyCrouchAttack = "SprGuyCrouchAttack";
        public string SprGuyFall = "SprGuyFall";
        public string SprTileDirt = "SprTileDirt";
        public string SprTileCoal = "SprTileCoal";
        public string SprTileSilver = "SprTileSilver";
        public string SprTileCopper = "SprTileCopper";
        public string SprTileGold = "SprTileGold";

        public string SprNoGo = "SprNoGo";
        public string SprCrack = "SprCrack";
        public string SprParticleSmall = "SprParticleBrown";
        public string SprSword = "SprSword";
        public string SprSwordItem = "SprSwordItem";
        public string SprPowerSword = "SprPowerSword";
        public string SprPowerSwordItem = "SprPowerSwordItem";
        public string SprPowerSwordProjectile = "SprPowerSwordProjectile";
        public string SprShield = "SprShield";
        public string SprPlinthEmpty = "SprPlinthEmpty";
        public string SprPlinthSword = "SprPlinthSword";
        public string SprPlinthShield = "SprPlinthShield";
        public string SprBow = "SprBow";
        public string SprArrow = "SprArrow";
        public string SprPlinthBow = "SprPlinthBow";
        public string SprTorch = "SprTorch";
        public string SprTorchOut = "SprTorchOut";
        public string SprTorchWall = "SprTorchWall";
        public string SprTorchOutWall = "SprTorchOutWall";
        public string SprLantern = "SprLantern";
        public string SprHorizon = "SprHorizon";
        public string SprGrassBlockTiles = "SprGrassTiles";
        public string SprRockMonsterTiles = "SprRockMonsterTiles";
        public string SprTreeTiles = "SprTreeTiles";
        public string SprLadderTiles = "SprLadderTiles";
        public string SprBigChest = "SprBigChest";
        public string SprMountainBackdrop = "SprMountainBackdrop";
        public string SprCaveBackgroundBlockTiles = "SprCaveBackgroundBlockTiles";
        public string SprWoodBackgroundBlockTiles = "SprWoodBackgroundBlockTiles";
        public string SprSandRockBack = "SprSandRockBack";
        public string SprBlock_Rock = "SprBlock_Rock";
        public string SprBlock_Copper = "SprBlock_Copper";
        public string SprBlock_Gold = "SprBlock_Gold";
        public string SprBlock_Sandrock = "SprBlock_Sandrock";
        public string SprSmallChest = "SprSmallChest";
        public string SprSilverSmallChest = "SprSilverSmallChest";
        public string SprGoldSmallChest = "SprGoldSmallChest";
        public string SprGoldChest = "SprGoldChest";
        public string SprCoin = "SprCoin";
        public string SprCoinSm = "SprCoinSm";
        public string SprZombieWalk = "SprZombieWalk";
        public string SprSkeleWalk = "SprSkeleWalk";
        public string SprBlock_Obsidian = "SprBlock_Obsidian";
        public string SprCursorArrow = "SprCursorArrow";
        public string SprCursorCrosshair = "SprCursorCrosshair";
        public string SprCursorQuestion = "SprCursorQuestion";
        public string SprCursorSword = "SprCursorSword";
        public string SprCursorTalk = "SprCursorTalk";
        public const string SprParticleBig = "SprParticleBig";
        public string SprGrassDirt2Tiles = "SprGrassDirt2Tiles";
        public string SprRockMonsterWalk = "SprRockMonsterWalk";
        public string SprBlock_GrassDirt = "SprBlock_GrassDirt";
        public string SprBlock_GreenDot = "SprBlock_GreenDot";
        public string SprBlock_WaterGrass = "SprBlock_WaterGrass";
        public string SprBlock_Crag = "SprBlock_Crag";
        public string SprBlock_DirtBackground = "SprBlock_DirtBackground";
        public string SprBlock_WaterGrassBack = "SprBlock_WaterGrassBack";
        public string SprBomb = "SprMine";
        public string SprBombUI = "SprMineUI";
        public string SprBowUI = "SprBowUI";
        public string SprSelectedItemUI = "SprSelectedItemUI";
        public string SprCheckboxUI = "SprCheckboxUI";
        public string SprSmoke_Yellow = "SprSmoke_Yellow";
        public string SprMineshaftExit = "SprMineshaftExit";
        public string SprMenuUIInventory = "SprMenuUIInventory";
        public string SprMenuUIMap = "SprMenuUIMap";
        public string SprMenuUIOption = "SprMenuUIOption";
        public string SprGlowfish_Swim = "SprGlowfish_Swim";
        public string SprSeaweed = "SprSeaweed";
        public string SprBoots = "SprBoots";
        public string SprGuyOpenChest = "SprGuyOpenChest";
        public string SprTextBk = "SprTextBk";
        public string SprSign = "SprSign";
        public string SprMoreTextCursor = "SprMoreTextCursor";
        public string SprBackground_Trees = "SprBackground_Trees";
        public string SprCaveVineTiles = "SprCaveVineTiles";
        public string SprGlowFlower0 = "SprGlowFlower0";
        public string SprGlowFlower1 = "SprGlowFlower1";
        public string SprRock0 = "SprRock0";
        public string SprRock1 = "SprRock1";
        public string SprRock2 = "SprRock2";
        public string SprShroom0 = "SprShroom0";
        public string SprShroom1 = "SprShroom1";
        public string SprShroom2 = "SprShroom2";
        public string SprFlower0 = "SprFlower0";
        public string SprFlower1 = "SprFlower1";
        public string SprFlower2 = "SprFlower2";
        public string SprFlower3 = "SprFlower3";
        public string SprGrass0 = "SprGrass0";
        public string SprGrass1 = "SprGrass1";
        public string SprGrass2 = "SprGrass2";
        public string SprGrass3 = "SprGrass3";

        public string SprLever = "SprLever";
        public string SprParticleCloud = "SprParticleCloud";
        public string SprParticleRock = "SprParticleRock";
        public string SprItemPotion = "SprItemBomb";
        public string SprItemBomb = "SprItemPotion";
        public string SprHeartUI = "SprHeartUI";
        public string SprDoor_LR = "SprDoor_LR";
        public string SprDoor_TB = "SprDoor_TB";
        public string SprSmallKey = "SprSmallKey";
        public string SprSmallKeyUI = "SprSmallKeyUI";
        public string SprMarble = "SprMarble";
        public string SprMarbleUI = "SprMarbleUI";
        public string SprBlackNogo = "SprBlackNogo";
        public string SprGrub = "SprGrub";
        public string SprGrubLava = "SprGrubLava";
        public string SprGrubWater = "SprGrubWater";
        public string SprGrubRock = "SprGrubRock";
        public string SprBlock_Hedge = "SprBlock_Hedge";
        public string SprFallThroughDissolve = "SprFallThroughDissolve";
        public string SprPlantBombDudeAttack = "SprPlantBombDudeAttack";
        public string SprPlantBombDudeSleep = "SprPlantBombDudeSleep";
        public string SprPlantBombDudeCover = "SprPlantBombDudeCover";
        public string SprPlantBombDudeIdle = "SprPlantBombDudeIdle";
        public string SprPlantBomb = "SprPlantBomb";
        public string SprGuyHand = "SprGuyHand";
        public string SprSavePoint = "SprSavePoint";
        public string SprButtonSwitch = "SprButtonSwitch";
        public string SprBrazier = "SprBrazier";
        public string SprSparkle = "SprSparkle";
        public string SprApple = "SprApple";
        public string SprCapeGuyWalk = "SprCapeGuyWalk";
        public string SprCapeGuyTalk = "SprCapeGuyTalk";
        public string SprGuyDead = "SprGuyDead";

        public int NoGoTileId = 9999;//**NOT IN THE SPRITE KEY - this is the default "black" background tile so player can't see shit.
        public int BorderTileId { get; private set; } = 1;// World
        public int BlockTileId_Rock { get; private set; } = 2;// Wo
        public int BlockTileId_Copper { get; private set; } = 3;// 
        public int LadderTileId_R { get; private set; } = 4;// Worl
        public int BlockTileId_SandRock { get; private set; } = 5;
        public int BlockTileID_CaveBack { get; private set; } = 6;

        public int SwordItemTileId { get; private set; } = 7;
        public int Sun_20Percent { get; private set; } = 8;
        public int Sun_5Percent { get; private set; } = 9;
        public int SilverSmallChestTileId { get; private set; } = 10;
        public int GoldSmallChestTileId { get; private set; } = 11;
        public int SandRockBackTileId { get; private set; } = 12;

        public int BigChestTileId { get; private set; } = 13;
        public int LadderTileId_L { get; private set; } = 14;// Worl
        public int TreeTileId { get; private set; } = 15;// World.Re
        public int TorchTileId { get; private set; } = 16;// World.R
        public int MonsterGrassTileId { get; private set; } = 17;// 
        //Monster Rock Tile Id

        public int GuyTileId { get; private set; } = 19;
        public int SmallChestTileId { get; private set; } = 20;
        public int PlantBombGuyTileId { get; private set; } = 21;

        public int ShieldItemTileId { get; private set; } = 22;
        public int BrazierTileId { get; private set; } = 23;
        public int GoldChestTileId { get; private set; } = 24;

        public int FlowerTileId { get; private set; } = 25; //50% water
        public int ZombieTileId { get; private set; } = 26;
        public int BootsTileId { get; private set; } = 27;
        public int Water100TileId { get; private set; } = 28; //100% water
        public int Water50TileId { get; private set; } = 29; //50% water
        public int Lava100TileId { get; private set; } = 30; //100% water

        public int Lava50TileId { get; private set; } = 31; //50% water
        public int BlockTileId_Obsidian { get; private set; } = 32; //50% water
        public int TriggerTileId0 { get; private set; } = 33;
        public int TriggerTileId1 { get; private set; } = 34;
        public int TriggerTileId2 { get; private set; } = 35;
        public int TriggerTileId3 { get; private set; } = 36;

        public int TriggerTileId4 { get; private set; } = 37;
        public int TriggerTileId5 { get; private set; } = 38;
        public int TriggerTileId6 { get; private set; } = 39;
        public int TriggerTileId7 { get; private set; } = 40;
        public int TriggerTileId8 { get; private set; } = 41;
        public int TriggerTileId9 { get; private set; } = 42;

        public int RockMonsterTileId { get; private set; } = 43; //50% water
        public int RockTileId { get; private set; } = 44; //50% water
        public int MushroomTileId { get; private set; } = 45; //50% water
        public int BlockTileId_GrassDirt { get; private set; } = 46;
        public int LanternTileId { get; private set; } = 47;// World.Res.
        public int BlockTileId_GreenDot { get; private set; } = 48;

        public int Key_MonsterTileId { get; private set; } = 49; //50% water
        public int SkeleTileId { get; private set; } = 50;
        public int BlockTileId_Hedge { get; private set; } = 51;
        public int Door_Nolock_TileId { get; private set; } = 52;

        public int FallThrough_Tile_TileId { get; private set; } = 53;
        public int Bombable_Tile_TileId { get; private set; } = 54;

        public int SmallKey_TileId { get; private set; } = 55; //50% water
        public int Door_Lock_TileId { get; private set; } = 56; //50% water
        public int Door_Lock_Monster_TileId { get; private set; } = 57; //50% water
        public int Door_Lock_RightOnly_TileId { get; private set; } = 58; //50% water
        public int Door_Lock_LeftOnly_TileId { get; private set; } = 59; //50% water
        public int Doorless_Portal_TileId { get; private set; } = 60; //50% water

        public int BlockTileId_WaterGrass { get; private set; } = 61;
        public int BlockTileId_WaterGrassBack { get; private set; } = 62;
        public int SeaweedTileId { get; private set; } = 63;
        public int GlowfishTileId { get; private set; } = 64;
        //Ice Block
        public int SignTileId_0 { get; private set; } = 66;

        public int SignTileId_1 { get; private set; } = 67;
        public int SignTileId_2 { get; private set; } = 68;
        public int SignTileId_3 { get; private set; } = 69;
        public int SignTileId_4 { get; private set; } = 70;
        public int SignTileId_5 { get; private set; } = 71;
        public int SignTileId_6 { get; private set; } = 72;

        public int SignTileId_7 { get; private set; } = 73;
        public int SignTileId_8 { get; private set; } = 74;
        public int SignTileId_9 { get; private set; } = 75;
        public int SignTileId_10 { get; private set; } = 76;
        public int BombTileId { get; private set; } = 77;
        public int BombPowerupTileId { get; private set; } = 78;

        public int CaveVineTileId { get; private set; } = 79;
        public int GlowFlowerTileId { get; private set; } = 80;
        public int LeverTileId_0 { get; private set; } = 81;//Levers + Gates
        public int LeverTileId_1 { get; private set; } = 82;
        public int LeverTileId_2 { get; private set; } = 83;
        public int LeverTileId_3 { get; private set; } = 84;

        public int LeverTileId_4 { get; private set; } = 85;
        public int LeverTileId_5 { get; private set; } = 86;
        public int LeverTileId_6 { get; private set; } = 87;
        public int LeverTileId_7 { get; private set; } = 88;
        public int GateTileId_0 { get; private set; } = 89;
        public int GateTileId_1 { get; private set; } = 90;

        public int GateTileId_2 { get; private set; } = 91;
        public int GateTileId_3 { get; private set; } = 92;
        public int GateTileId_4 { get; private set; } = 93;
        public int GateTileId_5 { get; private set; } = 94;
        public int GateTileId_6 { get; private set; } = 95;
        public int GateTileId_7 { get; private set; } = 96;

        public int SlopeTile_BR { get; private set; } = 97;
        public int SlopeTile_BL { get; private set; } = 98;
        public int SlopeTile_TR { get; private set; } = 99;
        public int SlopeTile_TL { get; private set; } = 100;
        public int EnemSharkTileId { get; private set; } = 101;
        public int EnemGrubGrassTileId { get; private set; } = 102;

        public int PowerSwordTileId { get; private set; } = 103;
        public int CrystalRockTileId { get; private set; } = 104;
        public int Elevator_A_TileId { get; private set; } = 105;
        public int Elevator_B_TileId { get; private set; } = 106;
        public int BugTileId { get; private set; } = 107;
        public int EnemGrubWaterTileId { get; private set; } = 108;

        public int SavePointTileId { get; private set; } = 109;
        public int SwitchButtonTileId { get; private set; } = 110;
        public int SwitchDoorTileId { get; private set; } = 111;
        public int SwitchConduitTileId { get; private set; } = 112;
        //113
        public int Tar80TileId { get; private set; } = 114;

        public int BlockTileId_Crag { get; private set; } = 115;
        public int EnemGrubLavaTileId { get; private set; } = 116;
        public int EnemGrubRockTileId { get; private set; } = 117;
        public int BlockTileID_DirtBackground { get; private set; } = 118;
        public int BowItemTileId { get; private set; } = 119;
        public int Spike1_LTileId { get; private set; } = 120;

        public int Spike1_RTileId { get; private set; } = 121;
        public int Spike1_TTileId { get; private set; } = 122;
        public int Spike1_BTileId { get; private set; } = 123;
        public int Spike2_LTileId { get; private set; } = 124;
        public int Spike2_RTileId { get; private set; } = 125;
        public int Spike2_TTileId { get; private set; } = 126;

        public int Spike2_BTileId { get; private set; } = 127;
        public int CapeGuyTileId = 128;
        public int AppleTileId = 129;
        public int BlockTileID_WoodBackground = 130;

        public Res(ContentManager c)
        {
            Content = c;
            Audio = new Audio();
            Tiles = new Tiles();
        }


        public void Load(GraphicsDevice d)
        {
            Font = Content.Load<SpriteFont>("Font");
            Font2 = Content.Load<SpriteFont>("Font2");
            Tiles.Texture = Content.Load<Texture2D>("miner-16x16");

            //**NOGO is alwasy a seamless tile
            //Mesh the grass with the various rocks
            List<int> SeamlessRock = new List<int>() {
                NoGoTileId,
                BlockTileId_Rock             ,
                BlockTileId_Copper       ,
                    BlockTileId_SandRock,
                    BlockTileId_GrassDirt,
                    BlockTileId_GreenDot
            };

           

            //Create3x3Tiles(SprGrassBlockTiles, 12, 0, 6, 9, GrassTileId, SeamlessRock);
            Create3x3Tiles(SprCaveBackgroundBlockTiles, 18 + 6 * 0, 0, 6, 9, BlockTileID_CaveBack,  new List<int> { NoGoTileId, BlockTileID_WoodBackground, BlockTileID_CaveBack, BlockTileId_WaterGrassBack, BlockTileID_DirtBackground });
            Create3x3Tiles(SprWoodBackgroundBlockTiles, 18 + 6 * 1, 0, 6, 9, BlockTileID_WoodBackground,  new List<int> { NoGoTileId, BlockTileID_WoodBackground, BlockTileID_CaveBack, BlockTileId_WaterGrassBack, BlockTileID_DirtBackground });
            Create3x3Tiles(SprBlock_Sandrock, 18 + 6 * 2 , 0, 6, 9, BlockTileId_SandRock, SeamlessRock);
            Create3x3Tiles(SprBlock_GrassDirt, 18 + 6 * 3, 0, 6, 9, BlockTileId_GrassDirt, SeamlessRock);
            Create3x3Tiles(SprBlock_GreenDot, 18 + 6 * 4, 0, 6, 9, BlockTileId_GreenDot, new List<int>() { NoGoTileId, BlockTileId_GreenDot, BlockTileId_GrassDirt });
            Create3x3Tiles(SprBlock_WaterGrass, 18 + 6 * 5, 0, 6, 9, BlockTileId_WaterGrass, new List<int>() { NoGoTileId, BlockTileId_WaterGrass, BlockTileId_GrassDirt });
            Create3x3Tiles(SprBlock_WaterGrassBack, 18 + 6 * 6, 0, 6, 9, BlockTileId_WaterGrassBack, new List<int>() { NoGoTileId, BlockTileId_WaterGrassBack, BlockTileID_CaveBack });

            //ice glass
            Create3x3Tiles(SprBlock_Hedge, 30 + 6 * 1, 9, 6, 9, BlockTileId_Hedge, new List<int>() { NoGoTileId, BlockTileId_Hedge });
            //Old grass tiles
            Create3x3Tiles(SprSandRockBack, 30 + 6 * 3, 9, 6, 9, BlockTileID_CaveBack, new List<int> { NoGoTileId, SandRockBackTileId });
            Create3x3Tiles(SprBlock_Crag, 30 + 6 * 4, 9, 6, 9, BlockTileId_Crag, new List<int>() { NoGoTileId, BlockTileId_Crag });

            Create3x3Tiles(SprBlock_DirtBackground, 30 + 6 * 0, 18, 6, 9, BlockTileID_DirtBackground, new List<int> { NoGoTileId, BlockTileID_WoodBackground, BlockTileID_CaveBack, BlockTileId_WaterGrassBack, BlockTileID_DirtBackground });

            Tiles.AddSprite(SprCursorArrow, new List<Rectangle>() {new Rectangle(10, 2, 1, 1),}, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprCursorCrosshair  , new List<Rectangle>() { new Rectangle(11, 2, 1, 1),}, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprCursorQuestion, new List<Rectangle>() { new Rectangle(12, 2, 1, 1),}, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprCursorTalk, new List<Rectangle>() { new Rectangle(12, 1, 1, 1), }, 0.0f, Tiling.Single);
            Tiles.AddSprite(SprCursorSword, new List<Rectangle>() { new Rectangle(13, 2, 1, 1),}, 0.0f, Tiling.Single);

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
            Tiles.AddSprite(SprCheckboxUI, new List<Rectangle>() { new Rectangle(9, 22, 1, 1), new Rectangle(10, 22, 1, 1),}, 0.0f, Tiling.Single);

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
               new Rectangle(4, 0, 1, 1),
                new Rectangle(5, 0, 1, 1),
                new Rectangle(6, 0, 1, 1),
                new Rectangle(5, 0, 1, 1),
            }, 0.8f);

            Tiles.AddSprite(SprGuyJump, new List<Rectangle>() {
               new Rectangle(7, 0, 1, 1),
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
            Tiles.AddSprite(SprParticleSmall, new List<Rectangle>() { new Rectangle(2, 4, 1, 1), }, 0.4f);
            Tiles.AddSprite(SprParticleBig, new List<Rectangle>() { new Rectangle(3, 4, 1, 1), }, 0.4f);
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
               new Rectangle(4, 9, 1, 1),
               new Rectangle(5, 9, 1, 1),
               new Rectangle(6, 9, 1, 1),
               new Rectangle(4, 10, 1, 1),
               new Rectangle(5, 10, 1, 1),
               new Rectangle(6, 10, 1, 1),
            }, 0.4f);
            Tiles.AddSprite(SprSign, new List<Rectangle>() { new Rectangle(10, 6, 1, 1) }, 0f);

            Tiles.AddSprite(SprApple, new List<Rectangle>() { new Rectangle(11, 6, 1, 1) }, 0.92f);//Attack Hand

            Tiles.AddSprite(SprMoreTextCursor, new List<Rectangle>() {
                new Rectangle(7, 14, 1, 1),
                new Rectangle(8, 14, 1, 1),
                new Rectangle(9, 14, 1, 1),
                new Rectangle(10, 14, 1, 1),
                new Rectangle(9, 14, 1, 1),
                new Rectangle(8, 14, 1, 1),
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



            Tiles.AddSprite(SprLever, new List<Rectangle>() { new Rectangle(9, 12, 1, 1), new Rectangle(10, 12, 1, 1) }, 0f, Tiling.Single);

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
            Tiles.AddSprite(SprBlackNogo, new List<Rectangle>() { new Rectangle(0, 1, 1, 1) }, 0f, Tiling.Single);

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
            Tiles.AddSprite(SprGuyFall, new List<Rectangle>() {new Rectangle(9,1,1,1), new Rectangle(10,1,1,1) }, 0.6f);

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
            Tiles.AddSprite(SprSavePoint, new List<Rectangle>() { new Rectangle(9, 19, 1, 1), new Rectangle(10,19,1,1) }, 0.0f);//Attack Hand


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
            ,0.82f);//Attack Hand

            Tiles.AddSprite(SprGuyDead, new List<Rectangle>() {
                new Rectangle(2, 3, 1, 1),
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
            


        }


        public void Create3x3Tiles(string name, int x0, int y0, int w, int h, int TileId, List<int> SeamlessIds)
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
