using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Unities
{
    /// <summary>
    /// token 的命名方式：
    /// [窗口] + 动词 + 名词 + [状态] + Token
    /// eg: AddUserSuccessToken
    /// </summary>
    public class TokenManage
    {
        public static readonly string MAIN_WINDOW_SHOW_TOAST_TOKEN = "MainWindowShowToastToken";
        public static readonly string MAIN_WINDOW_SHOW_DIALOG_TOKEN = "MainWindowShowDialogToken";
        public static readonly string MAIN_WINDOW_MASK_LAYER_TOKEN = "MainWindowMaskLayerToken";
        public static readonly string MAIN_WINDOW_REFRESH_MENU_TOKEN = "MainWindowRefreshMenuToken";
        public static readonly string LOGIN_WINDOW_CLOSE_TOKEN = "LoginWindowCloseToken";
        public static readonly string USER_ACTION_WINDOW_CLOSE_TOKEN = "UserActionWindowCloseToken";
public static readonly string FEERULESECTION_ACTION_WINDOW_CLOSE_TOKEN = "FeeRuleSectionActionWindowCloseToken";

public static readonly string VALUECAR_ACTION_WINDOW_CLOSE_TOKEN = "ValueCarActionWindowCloseToken";

public static readonly string LONGTERMRENTALCAR_ACTION_WINDOW_CLOSE_TOKEN = "LongTermRentalCarActionWindowCloseToken";

public static readonly string VALUECARDTYPE_ACTION_WINDOW_CLOSE_TOKEN = "ValueCardTypeActionWindowCloseToken";

public static readonly string LONGTERMRENTALCARDTYPE_ACTION_WINDOW_CLOSE_TOKEN = "LongTermRentalCardTypeActionWindowCloseToken";

public static readonly string VALUECARDACTION_ACTION_WINDOW_CLOSE_TOKEN = "ValueCardActionActionWindowCloseToken";

public static readonly string DELAYCARDACTION_ACTION_WINDOW_CLOSE_TOKEN = "DelayCardActionActionWindowCloseToken";

public static readonly string PARKWAYSTOPTIME_ACTION_WINDOW_CLOSE_TOKEN = "ParkWayStopTimeActionWindowCloseToken";

public static readonly string PARKWAYGROUP_ACTION_WINDOW_CLOSE_TOKEN = "ParkWayGroupActionWindowCloseToken";

public static readonly string PARKWAYCARD_ACTION_WINDOW_CLOSE_TOKEN = "ParkWayCardActionWindowCloseToken";

public static readonly string PARKWAY_ACTION_WINDOW_CLOSE_TOKEN = "ParkWayActionWindowCloseToken";

public static readonly string PARKWAYVOICE_ACTION_WINDOW_CLOSE_TOKEN = "ParkWayVoiceActionWindowCloseToken";

public static readonly string PARKSETTING_ACTION_WINDOW_CLOSE_TOKEN = "ParkSettingActionWindowCloseToken";

public static readonly string PARKDEVICE_ACTION_WINDOW_CLOSE_TOKEN = "ParkDeviceActionWindowCloseToken";

public static readonly string LICENSEPLATERECOGNITION_ACTION_WINDOW_CLOSE_TOKEN = "LicensePlateRecognitionActionWindowCloseToken";

public static readonly string CARDSWIPEDEVICE_ACTION_WINDOW_CLOSE_TOKEN = "CardSwipeDeviceActionWindowCloseToken";

public static readonly string OPENGATEREASON_ACTION_WINDOW_CLOSE_TOKEN = "OpenGateReasonActionWindowCloseToken";

public static readonly string HANDOVER_ACTION_WINDOW_CLOSE_TOKEN = "HandOverActionWindowCloseToken";

public static readonly string USERWAY_ACTION_WINDOW_CLOSE_TOKEN = "UserWayActionWindowCloseToken";

        public static readonly string PARKING_ABNORMAL_DETAILS_WINDOW_CLOSE_TOKEN = "UserWayActionWindowCloseToken";

        public static readonly string OPENGATERECORD_ACTION_WINDOW_CLOSE_TOKEN = "OpenGateRecordActionWindowCloseToken";

public static readonly string CALENDAR_ACTION_WINDOW_CLOSE_TOKEN = "CalendarActionWindowCloseToken";

public static readonly string CARTYPEPARA_ACTION_WINDOW_CLOSE_TOKEN = "CarTypeParaActionWindowCloseToken";

public static readonly string CARCONVERT_ACTION_WINDOW_CLOSE_TOKEN = "CarConvertActionWindowCloseToken";

public static readonly string BLACKCAR_ACTION_WINDOW_CLOSE_TOKEN = "BlackCarActionWindowCloseToken";

public static readonly string CARFREE_ACTION_WINDOW_CLOSE_TOKEN = "CarFreeActionWindowCloseToken";

public static readonly string PARKSETTINGCARD_ACTION_WINDOW_CLOSE_TOKEN = "ParkSettingCardActionWindowCloseToken";

public static readonly string CARVISITOR_ACTION_WINDOW_CLOSE_TOKEN = "CarVisitorActionWindowCloseToken";

public static readonly string PARKINGABNORMAL_ACTION_WINDOW_CLOSE_TOKEN = "ParkingAbnormalActionWindowCloseToken";

public static readonly string PARKINGARREARS_ACTION_WINDOW_CLOSE_TOKEN = "ParkingArrearsActionWindowCloseToken";

public static readonly string PARKINGOUTRECORD_ACTION_WINDOW_CLOSE_TOKEN = "ParkingOutRecordActionWindowCloseToken";

public static readonly string PARKINGINRECORD_ACTION_WINDOW_CLOSE_TOKEN = "ParkingInRecordActionWindowCloseToken";

public static readonly string ORDER_ACTION_WINDOW_CLOSE_TOKEN = "OrderActionWindowCloseToken";

public static readonly string SUM_ACTION_WINDOW_CLOSE_TOKEN = "SumActionWindowCloseToken";

public static readonly string PARKAREA_ACTION_WINDOW_CLOSE_TOKEN = "ParkAreaActionWindowCloseToken";

public static readonly string PARKINFO_ACTION_WINDOW_CLOSE_TOKEN = "ParkInfoActionWindowCloseToken";

public static readonly string ORDERREFUND_ACTION_WINDOW_CLOSE_TOKEN = "OrderRefundActionWindowCloseToken";

public static readonly string FEERULE_ACTION_WINDOW_CLOSE_TOKEN = "FeeRuleActionWindowCloseToken";

public static readonly string CHARGINGRULE_ACTION_WINDOW_CLOSE_TOKEN = "ChargingRuleActionWindowCloseToken";

public static readonly string WORKER_ACTION_WINDOW_CLOSE_TOKEN = "WorkerActionWindowCloseToken";

public static readonly string STUDENT_ACTION_WINDOW_CLOSE_TOKEN = "StudentActionWindowCloseToken";

public static readonly string ABC_ACTION_WINDOW_CLOSE_TOKEN = "ABCActionWindowCloseToken";

public static readonly string TEST_ACTION_WINDOW_CLOSE_TOKEN = "TestActionWindowCloseToken";

public static readonly string FEE_ACTION_WINDOW_CLOSE_TOKEN = "FeeActionWindowCloseToken";
        public static readonly string CODE_CLASS_ACTION_WINDOW_CLOSE_TOKEN = "CodeClassActionWindowCloseToken";
        public static readonly string CODE_FIELD_ACTION_WINDOW_CLOSE_TOKEN = "CodeFieldActionWindowCloseToken";

        public static readonly string ROLE_ACTION_WINDOW_CLOSE_TOKEN = "RoleActionWindowCloseToken";
        public static readonly string REFRESH_SUMMARY_SELECTED_USER_TOKEN = "RefreshSummarySelectedUserToken";
public static readonly string REFRESH_SUMMARY_SELECTED_FEERULESECTION_TOKEN = "RefreshSummarySelectedFeeRuleSectionToken";

public static readonly string REFRESH_SUMMARY_SELECTED_VALUECAR_TOKEN = "RefreshSummarySelectedValueCarToken";

public static readonly string REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCAR_TOKEN = "RefreshSummarySelectedLongTermRentalCarToken";

public static readonly string REFRESH_SUMMARY_SELECTED_VALUECARDTYPE_TOKEN = "RefreshSummarySelectedValueCardTypeToken";

public static readonly string REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCARDTYPE_TOKEN = "RefreshSummarySelectedLongTermRentalCardTypeToken";

public static readonly string REFRESH_SUMMARY_SELECTED_VALUECARDACTION_TOKEN = "RefreshSummarySelectedValueCardActionToken";

public static readonly string REFRESH_SUMMARY_SELECTED_DELAYCARDACTION_TOKEN = "RefreshSummarySelectedDelayCardActionToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKWAYSTOPTIME_TOKEN = "RefreshSummarySelectedParkWayStopTimeToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKWAYGROUP_TOKEN = "RefreshSummarySelectedParkWayGroupToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKWAYCARD_TOKEN = "RefreshSummarySelectedParkWayCardToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKWAY_TOKEN = "RefreshSummarySelectedParkWayToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKWAYVOICE_TOKEN = "RefreshSummarySelectedParkWayVoiceToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKSETTING_TOKEN = "RefreshSummarySelectedParkSettingToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKDEVICE_TOKEN = "RefreshSummarySelectedParkDeviceToken";

public static readonly string REFRESH_SUMMARY_SELECTED_OPENGATEREASON_TOKEN = "RefreshSummarySelectedOpenGateReasonToken";

public static readonly string REFRESH_SUMMARY_SELECTED_HANDOVER_TOKEN = "RefreshSummarySelectedHandOverToken";

public static readonly string REFRESH_SUMMARY_SELECTED_USERWAY_TOKEN = "RefreshSummarySelectedUserWayToken";

public static readonly string REFRESH_SUMMARY_SELECTED_OPENGATERECORD_TOKEN = "RefreshSummarySelectedOpenGateRecordToken";

public static readonly string REFRESH_SUMMARY_SELECTED_CALENDAR_TOKEN = "RefreshSummarySelectedCalendarToken";

public static readonly string REFRESH_SUMMARY_SELECTED_CARTYPEPARA_TOKEN = "RefreshSummarySelectedCarTypeParaToken";

public static readonly string REFRESH_SUMMARY_SELECTED_CARCONVERT_TOKEN = "RefreshSummarySelectedCarConvertToken";

public static readonly string REFRESH_SUMMARY_SELECTED_BLACKCAR_TOKEN = "RefreshSummarySelectedBlackCarToken";

public static readonly string REFRESH_SUMMARY_SELECTED_CARFREE_TOKEN = "RefreshSummarySelectedCarFreeToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKSETTINGCARD_TOKEN = "RefreshSummarySelectedParkSettingCardToken";

public static readonly string REFRESH_SUMMARY_SELECTED_CARVISITOR_TOKEN = "RefreshSummarySelectedCarVisitorToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKINGABNORMAL_TOKEN = "RefreshSummarySelectedParkingAbnormalToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKINGARREARS_TOKEN = "RefreshSummarySelectedParkingArrearsToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKINGOUTRECORD_TOKEN = "RefreshSummarySelectedParkingOutRecordToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKINGINRECORD_TOKEN = "RefreshSummarySelectedParkingInRecordToken";

public static readonly string REFRESH_SUMMARY_SELECTED_ORDER_TOKEN = "RefreshSummarySelectedOrderToken";

public static readonly string REFRESH_SUMMARY_SELECTED_SUM_TOKEN = "RefreshSummarySelectedSumToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKAREA_TOKEN = "RefreshSummarySelectedParkAreaToken";

public static readonly string REFRESH_SUMMARY_SELECTED_PARKINFO_TOKEN = "RefreshSummarySelectedParkInfoToken";

public static readonly string REFRESH_SUMMARY_SELECTED_ORDERREFUND_TOKEN = "RefreshSummarySelectedOrderRefundToken";

public static readonly string REFRESH_SUMMARY_SELECTED_FEERULE_TOKEN = "RefreshSummarySelectedFeeRuleToken";

public static readonly string REFRESH_SUMMARY_SELECTED_CHARGINGRULE_TOKEN = "RefreshSummarySelectedChargingRuleToken";

public static readonly string REFRESH_SUMMARY_SELECTED_WORKER_TOKEN = "RefreshSummarySelectedWorkerToken";

public static readonly string REFRESH_SUMMARY_SELECTED_STUDENT_TOKEN = "RefreshSummarySelectedStudentToken";

public static readonly string REFRESH_SUMMARY_SELECTED_ABC_TOKEN = "RefreshSummarySelectedABCToken";

public static readonly string REFRESH_SUMMARY_SELECTED_TEST_TOKEN = "RefreshSummarySelectedTestToken";

public static readonly string REFRESH_SUMMARY_SELECTED_FEE_TOKEN = "RefreshSummarySelectedFeeToken";
        public static readonly string REFRESH_SUMMARY_SELECTED_CODE_CLASS_TOKEN = "RefreshSummarySelectedCodeClassToken";
        public static readonly string REFRESH_SUMMARY_SELECTED_CODE_FIELD_TOKEN = "RefreshSummarySelectedCodeFieldToken";

        public static readonly string REFRESH_SUMMARY_SELECTED_ROLE_TOKEN = "RefreshSummarySelectedRoleToken";
        public static readonly string SHOW_CODE_FIELD_TOKEN = "ShowCodeFieldToken";
    }
}
