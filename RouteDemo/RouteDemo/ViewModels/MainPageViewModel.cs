﻿using Prism.Navigation;
using RouteDemo.Helpers;
using RouteDemo.Model;
using RouteDemo.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Bindings;

namespace RouteDemo.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ICommand PinClickedCommand { get; }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";
            contentview = new ContentView();
        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            await Task.Delay(1000); // workaround for #30 [Android]Map.Pins.Add doesn't work when page OnAppearing  
            GetListRoute();
        }
        private ContentView contentview;
        private CancellationTokenSource ctsRouting = new CancellationTokenSource();
        public bool IsRunning { get; set; }
        private Pin PinCar, PinPlate;
        private Polyline RouteLine;
        private Position currentRoute;
        public int PlayMin = 0;

        public int PlayMax = 99;

        public int playCurrent = 0;
        public int PlayCurrent { get => playCurrent; set => SetProperty(ref playCurrent, value); }

        public bool isWatching = true;
        public bool IsWatching { get => isWatching; set => SetProperty(ref isWatching, value); }
        public Position CurrentRoute { get => currentRoute; set => SetProperty(ref currentRoute, value); }
        public List<Position> ListRoute { get; set; } = new List<Position>();
        public ObservableCollection<Pin> Pins { get; set; } = new ObservableCollection<Pin>();
        public ObservableCollection<Polyline> Polylines { get; set; } = new ObservableCollection<Polyline>();
        public MoveCameraRequest MoveCameraRequest { get; } = new MoveCameraRequest();

        public AnimateCameraRequest AnimateCameraRequest { get; } = new AnimateCameraRequest();

        public bool isPlaying;
        public bool IsPlaying { get => isPlaying; set => SetProperty(ref isPlaying, value); }

        private void GetListRoute()
        {
            string routes = @"akfbC_}eeSMtAQjCA|C}AjAaEjA{EtCiFbDyEpCoFpB_F^mAxAb@rEbA`FiBrGmCbH{CpGwElHqGvHuGzGkGjGqGrG_FzE}GzG{IzIwGzGgHfHoHpHgHjHwHvHeHhHcHfHkHhHcHnHwHzHuHrHsHpHmHfHsHrG{HnFgIrEwJzDwIdCqJjBcKhAeKt@wJp@mJv@kJfBwIdC}HbCgHrBqDbAuJpCcLtEgGvDyF~EyElFiEpFsE|FmEvFqEzFmFhHsFtImFfJcFxIkF~IoGnHmIlEwIbBwI|AuJbE}HfFcIzFcIdGyIfFsK~C_LfB_Lr@eLSeLe@sKUiKv@qJtAaKhBiJhDuIzCiHB{CoBeAgAu@b@a@jBqAvBy@tAEBCLU\U`@Yh@{AbC{BdDgAlAgAdAeAfAgAhAaDbD_CzBeCvBoBrAsBvAgChBmChBcDxAmF|D}EzNyD`L_CbLiAjQaDnD????????????]jAgGvMiHbRwHtNaMfN_GrJ|JdLjNxInOxHfHpPjE~QYdVqDvV`DtPbHpUcFtWkEnVoA~UuA~TCdRSjSwDzSiArTcC|MqJ|K}G~ScKfPiI`S_F|OuBdU|HzRtOhKlGbVfMlNzE`MnHv{@sIzHsD`K|JfJjJfMb@hPnJdIlBxLdDrKvCpKlAzDdAhEUpBSl@Ox@mA|E_AxDuBdIcHjIeFrHsJrOkHxGgRfIsOpF_JlNiJbP}L~KuChIuBdMuBvTcFvVsE|SwEhSmCjI???J{BtIuDvQoB|O{FvTgHr\iCnWi@nVrApUZxQGdWsBbV_Cb[aL`QmOjRiLdP}B~E{GlPgFdUkHjV_RtTwHzPqJpXcJ`W}EnQkBjPeLfDyA|@_AbBmCdEnB`MvMvHpQpEtR|BtG~S`GlR_JfRcErQVdSaFjQgLnFiNfDqFxKfGtRoFhWse@rc@wMtRsGpTwDzQaAlTkDnVcF`WkFxVdCzN|E`WrDxQ~CjUuChWg@rUXjOkEvQaNdQsApTuFbPmJnUyE|P?hSc@fEc@|DOzA?????B@`@BXXx@n@v@bAh@lA\jAZbAZhA\bD`ApDbAhDbAfCdA~@\BEPDtBt@pCt@bEfAjD~ATt@????@DBNHp@IbAg@jA{AdD}A`Dc@l@??????????Uh@InAdAtA`ApAbA`Bp@zABJ????XPFD`@^l@r@l@p@z@nBnAfCzA|CbApD^jD_AnDUxA?Fb@zG{H|IsDxG??????B~NoA`M}FbOW|OeLdKaJdB_GdQgMtSkPhP}KtP??}]xj@eJdCqAn@kApAeAjAcAhAcAfAi@z@Yt@]`Aa@dA]pAAfAHf@@H??????????????Ah@AH]z@w@`Bw@dAwA`By@~@wAbBsA~AoEdEuEbBiBdDUpF^jFi@jEiClCoElBuEjBoDnC}Ey@_D_E{BuDcDo@oEtAwCdD{CtBuCtDcBjEmDtBkBlCkArBeAbB_DrD}DfDkBnD}BpD_CrDw@jEIzDCzDG|EIrFChE{AbDk@dEaCpDmD`CoDp@_CpAq@rAq@`AsAbAsFvDwDrEmBvHgD|EgCpDiDdCaCPkEvAoE~@yEgB_FkAaCl@wMpD{Cr@gIpB}GjC}FpAmERkBHUA????G@q@FY@mAQiAy@aAoA}BkEwBuEyBmEcD_@cDn@qBp@uBr@sBj@_E~AmBrA{AvAEhBi@zEkCxDLjF_BfEcDlCsBfDYrDq@rHmAjFeBjHcCrHiDrFwC~DaBbGaArGl@zG{@~FqBlGm@nHaBbHkBdH_AfHD|G`@`Ha@fHiAxGyAfGsBjGmB~FuBxGiBbGsCxEaGlBsFfCcCjGeBzG_BbGyBrDqA`BiAhAWP??????q@b@GF{@p@eA|@uAbAyDjCsFpEiGbC_GxCoGjCiFnE{GjBaGxEuEzDiHz@{FbDeGhDiF`EiDtFgDjFiD~DwB`F{@~E~BzD\~EJ`GJnFQhHH|CPdEHtEBbE?x@@F@t@@j@DbBFvBFvBL|EN`FHlECnCU~Cs@bDq@tB]^??MV[l@e@dAm@hA}AtC{AvCgBhDsBzDkBlD}A|C[j@????EJGL[f@[d@SZ`@t@\B??????JD\P^J^Pp@XjBj@`CdArCnApB~@t@Z`Br@xAbAl@T??PHXLf@X|@`@tBz@hBKpCkArAmAl@kA^w@q@mAYk@PUNODa@[Y]?@B????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????IAM^N`@DH????RVLTPXX^H`@S\o@bAgArAsAv@sAl@uAB}Ak@iBu@eCmAkB{@yAk@eCeAkAi@qAk@iB{@aBq@_Bs@oAw@SmAb@kAt@sAj@iAp@{Az@iBtAgC~A_DjBsD|AyCxAsCnA}BjAiCt@wCR{AD_ADiBCeDIcEQ}EM_EEwBQ}I?kBAoBEkBU{EQiFNkFEsFOmG_@cGwBeEfAaFlBeEfD}D|CyEzCcFvEyEhF{BRQtAwAvAqAjBc@hFk@rE}AhD_ExGwEdGuAlE}DtF}BdGoCvF}BfEsD`LaI~EgFfDgGhBcHhB_HzCgFdGoBtFqCtBsFfBgGtByGvByGrBaHnAcHz@mGEgHc@qGr@eJ|A_HjBaH~@uHjAsGnBmGk@mHn@oGtAkF\eA??????JONS~@qAf@o@lDwF~AwExA{FdBqH|@oHv@yG`DwEhEgEWwF|AsEhCyC]uEzCuC|EsBbHsBfFeArD^nBjExBxEzCjFzFvBlHg@bHaA~FsCbHcB`HcBtGoBpGyA~B@|IjD`FsAfEiAjEoA|CsE|CsEhB}FhCwFfFuDrEgDnDgEfFeA`F_EjAiGdBaFEiGR}IN}Gx@eHpCeFvCoE|C}EpEiD|CcF`BqCpA{A`Bu@lAmAlBaFfDyChDmDrEwBbFAzBrDvCrD|DtB`E{AdDeDtEy@nDgDrAeEm@oFVyEn@yDhEaChEoCpCcDdAoAfAqAbBgB~BeEDsD`AyDdBoDtCeDtCaD`EY~CkBpDyBfD}CxCcEhDgEhC_EdBmEnAeEx@iCv@uAjAmBdCiEzC{DbC_CzCgBbDiDrC}EhCgF|CeEfCiErBsEf@iE|ByChDq@bDc@lCcAvBiA`A{A`A}Bq@}C[oBO_BTcBz@eA|BmCzAqCl@oDn@kDXiDd@sEWuE?{E^{D`CcCtCqBzA}AfBqBWmEH{E~@uD[cD_AeDqAoCoA}BIO????????????OWQ][o@g@s@o@{@o@u@q@w@k@}@oAyB_AuAc@y@DeBtAeCdAwBtAmCrAkCWeBmBoA_Cq@}Bq@}C{@wA_@uC{@mCw@yBm@kDoA}DoA_F{AoEkBg@yEt@}F^kFGqGBeGDeCb@yB|@sBxBkGpCaF|ByE|AmFD{Ep@_ChDwDb@}EEoHtBqHnC{FhFwDbF{DlBeFwAyFiAcGz@_Gn@_G?gGBcHpAsGp@qG[uEk@_C[gCYkCq@gHmAsGeBcH{AgH}@sHyCsD_D_CdA_FdCyEx@{BrDqWzE}Tj@{G~B}Sn@oSpKaRvGaUdS_JvMmHn@UtFyOzDcXzBoOaJsJzPiHhP}KdHiNUeS|CoPg{@gwAXaKpEeGdFcBlFqBrBqQzHyXzIaUtI}UpHkOfMsNrEwHdA}E`EkRlCwKxHwQpHsMnJoMdKcMpJwNvBaYxC}[Hg[eAaNi@sSZqPbA}N`BaOxAuOxG{MtDiSlB}O`E}PbCmI??bBiGbNan@rDyRvB}TxCcRxIyGzIiLlG{NbG{HxE_CdNkEjPkIlLcMrIiPdK_N|BuJ`CqJkAmKwBwHk@mBoB}G]uA}@wCSkAS_B??{@eF_I_H}AkNiFsLeMcMz@eK~JkNoK}YrBc[uGuP}N{RmE}QsNiIuHgSpB}R~K{]xKaR~JgV|K_P|DuU`@qVpDwXEoV|AsVjAqWzEcUdEaXoIuWmAwOlDyO[gUqDaSgDiPcMsIwOaIoI{LuE{CCI??????????vBoDjLoLjL{Q~G_RjHaO~Ogp@~EgObHmIxOiJbMwKlFeFjJyLpU{GzFeAzF_CpGmCbIeBpGu@~Fy@`Qk@vCH~DN`ENfEJhJ@nL}@bKcBnJuC~HuEhHgFlHkFrHoFtIuEfJuB~IuAhIgDdGsFrEeH`E{GrDuGfEoHjD_GzDaGpE_GvEcGpF_HvFeH|F}GrGsF`F{C|HmDtK_DdI{BbI{BjIuBlIyB|KeBlJs@pJm@nJu@~IkA~IkBfIaCfIeDjImEnH_FrGqFfGeGtGyG`GeGfGkGdGgG`GeG|F}F|F}F|F_GvFwFxF}F`GaG`CcC~KaL`GaGrGmGbGgGpGoGtGwGlGqGfGgG`GqG~E}GvDuG`DgHvCiIfCkFdEkBjCwCe@sC@qCjCgB|CmBtDaClDyB~Cy@pAUZI??????TSCiA??B[D_BJeATmCIw@}@i@oAw@y@yCwAoCkDeBwC_BeDiCsD{BuD}BqDiD_CwDqDmDqD_DsB{E}CeDoDeAoAiEiAyE_BiFgDqD_EaDkDoB@eAtC}AxDuAvA}@jF}D|EcClEeCxFkD`GuDjFaDjFgDjFuDjFiDbGsDdF_DnFaDrE{ClBiAjBiAjBkAjFmDnFqDhEoCfEmDvEuDzEoB`FkAbFkAlDs@vDmB`EmCnEaClEgCrFcBnEeAxFgBrFmB`FcBnFoB`FcBtE{AvEsAbEeAq@uHcAkFiByFPuG??t@cCx@iCt@aCnA_Gr@gFj@mEz@wFRyFTwFYaFg@cFi@gGo@}Gm@mGo@aHq@gHq@}GRsGf@aG`@}E\oEh@gEj@aDVcDcAiDqAcDqAcDeAsCgAqCg@iCx@{Az@}@x@eA^e@j@u@r@}@x@_AlB_CvAiD~@yD`AyDzAmDbA_ChA}CjAwDfAmEbAgElAeE|A}D~A_DpBwD~CiGrB_EhBiDzAuClBmDzBaE`B{Dt@_EdBsAtDX`EZdE\jE^xEb@zEFxEGbDEhHOrFIlFG~EGvEOlE?nECzEExEe@nCeD~B_EpAwBvA_F~HmBfEWhDMdCs@pC_B|CqBhD}BxDoCbEqCxEeCxEqBlF_BvF]nFl@rFn@hFj@|Fn@lEd@zDb@fD^vCXzBX|@JD???|@L`Bl@hB|@~BfAlCpAnClAhD~AxDfBvErA`FtAhFxAhFx@pFv@vFr@hFtAzExBzEzBpEvBnEnBhElClEtClE~ChEhCxEz@xEr@zEt@|Db@nD`BrDjBlDa@lDsCdE_ArEf@~Dd@zGl@nKu@pEiB|EyAdEiBtDiAjBi@~CcAfDcAdEs@bEIdCWzD_ApEqAdEyA|Aq@pEgAhB[fE{@pDu@rDs@|Ck@tDs@~Co@bE}@|Du@tDu@|Dw@vCm@hBYhAWRGjNyC~PeDnL{J????pq@adAfEsG`SeZz@sAd@s@PWR]d@u@l@}@v@mAt@oAv@iA|@sA~@yA`CqDfC}DhC}DlCeElCcEbCuDhCuDrCqDnB}BHM????f@m@TSn@s@JK~AeBxC{D`DsE`D_F~C}ExCyEbDcFfDoFxCcF`CeExByDnCoELSvKwQvMyS`LaQ|L_PfLeObKwNzImNtK{P|L_SlL_SbKwQvIsOnGsKhH_N`I}OlYis@jJkTbIkRfHuPrFoMdIaRnLwWrGmIzBsCjBeCf@o@LOt@}@jAaBfA{A`CqDpBwCrB{CbCoDfCqD|BkDlBiChBmChBkCtB{CrBwCxBcDfBmClBqCtBaDlBqCvAqBh@aAf@mADiABC????????????????????????????????????^?H?vBuBzImMdLoPtIaLfK}FxFqBnCgA`SeHtOkFPG??eIjBy@Q??Sk@CMa@oAg@uAo@{Am@cBm@yBk@_Ck@cCk@cCc@aCa@{B]wBWiBIs@?C????ACCSK_ACOUsB}@}Ik@uI]eKKmLHuKZkKf@kIt@wIbAwIjA}InAwJlAgJlA}IjAgJ`AeJd@{IReILkFHiDGcCDy@@?????????????SYEa@ASEsAk@qEy@oDaAaFoEoRqCsHgD{HiEeIcEuHqFmJaCeE}CkFwCcFeDuFgD}FuCaFuC}E_DeFgD{F}CsFyCgFwCgEsBsCwAgCiB_DoBgDgAkBsBgD}AyEdCyCp@kEq@}D|A{BdCkBvB{AMc@iBrAsCrBgIbGuCbBmDjByDtBmDfBoCtA_Ah@aB|@oBhAcClAeBv@AB????OLcAr@ODUjBfDlL~AhHLrAJ|A_AhB??cA|@eFnC{KtEyCjAwDxAoDeDsCmF????????a@m@qDgGAGyPgYpCtE}OoZZQF_@CG???????????e@CA????????????????????????????????????????????????????????????????oBPmGgKqHkMgFoIoAsByGcLuFqI\]????oCqBwKyOsNkS_LcOyJwNqGaJiIwLuF{Hk@}@sPEUxA?????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????[?m@rIwCjDl@tHxKlG`J`IfL~IjMnMtQfNzRhN`SzIrMxGzKxUz`@~EbHh@gBBC????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????YWA???_@K_A~@nAbCdEjH^p@^p@\n@`@p@^n@`@p@??zAbCz@rA|@xAx@`B|@zAr@tAl@hAd@bA`@n@f@x@z@tAx@vAl@fAr@lAlAvBnAxBpAxBr@tAb@Eh@_@v@]t@a@hDwA|@]BA????^UTIhBw@|Ac@bBw@bBy@|Ak@~@m@t@q@n@i@FIPQVaAImAc@{AOeAWoA_@qAUu@_@uAg@aBg@_BW{AQw@????|LmJ`CqAp@[v@c@z@e@fAm@hAm@jAq@jAq@fAk@|@g@t@Yn@Ah@T^d@h@|@l@bAt@pAz@vA|@rAv@rAx@tAbC`EjCrEhCjE|CjFtDnGfEfH`E`H~D|G|DzG`EbHxD~GrDnGbEbHhEnHnEvHzElI`EjItCjHvB`HhBjHnAnHv@hG`@~ETdDBx@??BT???L?N@\FhBD~CDjDGxEUnGi@dHu@xG}@hHaArHaAnHcAvHaArH}@|Gs@lGc@dE[pDYpEY`GOpGInG?fGFrFNrFV~F`@lGh@hGj@jF^dC@D??DRDZFh@Lz@PfAh@|CHb@ZdB^hBd@lB~@`EzA~Hp@lCZzAu@l@uAf@aBh@aBh@aBn@{Al@eBn@qAd@aA\eA^iA`@iC~@kA`@eAb@w@XWJc@Vk@b@q@`@aAr@eA`A_AhA_AtA}@tAaDxEiC`E????wPnViB|BWZq@nASb@u@fAkAbBQVaCpDoExGwFfI{FlIaF~GsFdIgHnKgFtHmExFmDtEa@h@]d@Y`@Y^e@j@o@x@e@n@u@bAo@x@w@~@w@jAk@tAu@dBy@fBy@nB{@vBcAzB}@vBaA~BgA`CcAxB}@hBk@vAc@hAk@nAy@lBcAbC_AvBoA|CmAvCwAbD{AlDwAlDsA|CuDrIaA~B_BvDwAdDwAbDuA~CyAjDwAhDyAjDyAhDiAvCiAlCkArC}@hC_ArCwA|C}AxCaBvC{AvCsAbCmA`CwAdC_BjCqAbCoAxBoBdDkBhDoBpDsBtDyBzDkCrEuBnDaDvFiChEcC|D_CtDcC|DaCtDaCvD_CrDyBnDsBbDkBvCcBlCsBbDwBdDmClDiCfDgCbD_BjBCF[b@Y^ONg@p@o@v@u@bAoF|GaC|CgCzDsChE{C|EmChEyCxEwC`FyBpD_C`EoCxEkCjE}B~DoCvEqCpEqClEcCrDoAtBo@z@??S`@QR]f@o@dAy@lAy@rA{BjDyB|CyAvB_DzDgDvDaDvDsD~EaD|EiCzDoBzCm@|@UZIL[d@cBjCcBlCgBlCeBnCiAnB{@fAqAtBmBrCuB`DyAxB_BlCyBbDuFnIqBpCaA|AqAhBaBbCqB|CuBdDaCpD{BdD_CpDsBzCcBfCwApBkAhBqAlBoAjBmBtCaDvEkBpCyDdGqB`CaDt@eDl@sB`@GB????????????????????????????????????y@VWFYDa@HiB^aDn@{Cl@kCj@wCl@aC^kCh@{Bb@eCf@aDn@kDt@sDt@kDt@aDr@gDl@gDj@aBb@K?????a@FG?cARwAXeB\{EdAkE`BqEzAkEpAoEz@mEVoDRmEnA{E|AuGnB_RvGyOv@ySwBaOxEiUkG}TiHmQiLmXiMiWmEwR{E{NuFePyH{GcBqQiB}QmB}TqBoStGmPvKgNnI{Ix@wLbC}IxQaSbCoRT{RVyRVki@ZgD`Jw@|Aw@zAy@|Aw@|Aw@xAw@vAw@~A}@hBaAfBy@`Bw@zAy@zAy@`B}@bBy@dBw@dBi@`Bg@jBi@rBg@tBuAvF{AzEqBxEqBzEoAfFmAdE{BjDkCvCaCxC{BnBMpBjAzCf@rAf@rAj@zAl@zA|AxDbAtCOrEe@vCe@bD[nDU~C[tD_@xDWtDRrEb@zEb@xEb@xEd@nE\`EX|C^lD^hE^bE^`EOjEDxDOfC]|B_@|B[bCu@lF{@jFaB~FkBbG]nGlBnF~@|E\vEiBdDoEtAmEtAaF`BaFfBmFhBeFfBgFfBiF|AeFhAwEvBsDtB{D~BcEzB}D|A_Dl@_AV????OFi@P{@VgAXyDz@_EhB{DvC{DdDaEjCgErCeEnCuBrAgEjCeGrDyD`CyD`CiEhCwEvCaEdC}CzB_DvBqDzBqD|BiElCeEfCuD`CeDlBiDjBwIxF_CnAwCfAcDdB`@x@nBfAlAp@hAx@fAdArCdCzA~DxAdE~@bFxBdDhDXnB~EnC|DzD~CtCrDrCxDnDrCnDxBzD~BnCzBvCtAjDrBfAbD`@rAf@x@l@Rj@X\j@At@SfAGn@O`CBbBo@j@kCt@mDxAkElCqEpC{D`CmE`ByEj@qBvE|AvDe@tG{BxGaDhHiElHmFbHcGnGgGhGoGxG";
            var lstlatlng = GeoHelper.DecodePoly(routes);
            if (lstlatlng != null && lstlatlng.Count > 0)
            {
                ListRoute = lstlatlng;
                PlayCurrent = 0;
                PlayMax = lstlatlng.Count - 1;
                RouteLine = new Polyline
                {
                    IsClickable = false,
                    StrokeColor = Color.FromHex("#4285f4"),
                    StrokeWidth = 3f,
                    ZIndex = 1
                };
                RouteLine.Positions.Add(new Position(lstlatlng[0].Latitude, lstlatlng[0].Longitude));
                DrawDoubleCar(lstlatlng[0].Latitude, lstlatlng[0].Longitude);
                MoveCameraRequest.MoveCamera(CameraUpdateFactory.NewCameraPosition(new CameraPosition(PinCar.Position, 14)));
                for (int i = 0; i < lstlatlng.Count; i++)
                {
                    DrawRouteLine(lstlatlng[i].Latitude, lstlatlng[i].Longitude);
                }
                Polylines.Add(RouteLine);

                StartRoute();
            }
        }
        private void DrawRouteLine(double lat, double lng)
        {
            RouteLine.Positions.Add(new Position(lat, lng));
        }
        private void DrawDoubleCar(double lat, double lng)
        {
            PinCar = new Pin()
            {
                Type = PinType.Place,
                Label = "pin_car",
                Anchor = new Point(.5, .5),
                Position = new Position(lat, lng),
                Rotation = 0,
                Icon = BitmapDescriptorFactory.FromBundle("car_blue.png"),
                ZIndex = 2,
                Tag = "89A16705",
                IsDraggable = false
            };
            Pins.Add(PinCar);

            //PinPlate = new Pin()
            //{
            //    Type = PinType.Place,
            //    Label = "pin_plate",
            //    Address = "Sumida-ku, Tokyo, Japan",
            //    Anchor = new Point(.5, .75),
            //    Position = new Position(lat, lng),
            //    Icon = BitmapDescriptorFactory.FromView(new PinInfowindowActiveView("89A16705")),
            //    ZIndex = 2,
            //    Tag = "89A16705" + "Plate",
            //    IsDraggable = false
            //};

            //Pins.Add(PinPlate);
        }

        private void DrawDirection(VehicleRoute vehicle)
        {
            var pin = new Pin()
            {
                Type = PinType.Place,
                Label = vehicle.Time.ToString("dd/MM/yyyy"),
                Anchor = new Point(.5, .5),
                Position = new Position(vehicle.Latitude, vehicle.Longitude),
                Rotation = vehicle.Direction ?? 0,
                Icon = BitmapDescriptorFactory.FromBundle("ic_arrow_tracking.png"),
                Tag = "direction",
                ZIndex = 1,
                IsDraggable = false
            };

            Pins.Add(pin);
        }


        private void StartRoute()
        {
            try
            {
                if (ctsRouting != null)
                    ctsRouting.Cancel();

                ctsRouting = new CancellationTokenSource();

                CurrentRoute = ListRoute[0];

                PinCar.Rotation = (float)GeoHelper.ComputeHeading(ListRoute[0].Latitude, ListRoute[0].Longitude, ListRoute[1].Latitude, ListRoute[1].Longitude);

                PinCar.Position = new Position(CurrentRoute.Latitude, CurrentRoute.Longitude);
                //PinPlate.Position = PinCar.Position;

                MoveCameraRequest.MoveCamera(CameraUpdateFactory.NewCameraPosition(new CameraPosition(PinCar.Position, 14)));

                SuperInteligent();

                IsPlaying = true;
            }
            catch (Exception ex)
            {
                if (ctsRouting != null)
                    ctsRouting.Cancel();

                IsPlaying = false;
            }
        }

        private void SuperInteligent()
        {
            PlayCurrent++;

            CurrentRoute = ListRoute[PlayCurrent];
            RotateMarker(CurrentRoute.Latitude, CurrentRoute.Longitude, () =>
            {
                MarkerAnimation(CurrentRoute.Latitude, CurrentRoute.Longitude, () =>
                {
                    if (PlayCurrent + 1 > PlayMax || ctsRouting.IsCancellationRequested)
                    {
                        IsPlaying = false;
                        return;
                    }

                    SuperInteligent();
                });
            });

        }

        public void MarkerAnimation(double latitude, double longitude, Action callback)
        {
            if (this.IsRunning)
            {
                callback();
            }
            else
            {
                IsRunning = true;
                var startPosition = new Position(PinCar.Position.Latitude, PinCar.Position.Longitude);
                var finalPosition = new Position(latitude, longitude);
                void callbackanimate(double input)
                {
                    var postionnew = GeoHelper.LinearInterpolator(input,
                        startPosition,
                        finalPosition);
                    PinCar.Position = new Position(postionnew.Latitude, postionnew.Longitude);
                    if (IsWatching && !ctsRouting.IsCancellationRequested)
                    {
                        _ = MoveCameraRequest.MoveCamera(CameraUpdateFactory.NewPosition(postionnew));
                    }
                }
                contentview.Animate(
                "moveCar",
                animation: new Animation(callbackanimate),
                rate: 10,
                length: 500,
                finished: (val, b) =>
                {
                    IsRunning = false;
                    callback();
                }
                );

            }
        }


        private void RotateMarker(double latitude,
            double longitude, Action callback)
        {
            // * tính góc quay giữa 2 điểm location
            var angle = GeoHelper.ComputeHeading(PinCar.Position.Latitude, PinCar.Position.Longitude, latitude, longitude);
            if (angle == 0)
            {
                callback();
                return;
            }
            var startRotaion = PinCar.Rotation;
            //tính lại độ lệch góc
            var deltaAngle = GeoHelper.GetRotaion(startRotaion, angle);
            void callbackanimate(double input)
            {
                var fractionAngle = GeoHelper.ComputeRotation(
                                     input,
                                      startRotaion,
                                      deltaAngle);

                PinCar.Rotation = (float)fractionAngle;
            }
            contentview.Animate(
                "rotateCar",

                animation: new Animation(callbackanimate),
                rate: 10,
                length: 50,

                finished: (val, b) =>
                {
                    callback();
                }
                );
        }
    }
}