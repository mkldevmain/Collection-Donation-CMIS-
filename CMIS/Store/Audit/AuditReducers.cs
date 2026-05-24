using Fluxor;

namespace CMIS.Store.Audit
{
    public static class AuditReducers
    {
        [ReducerMethod]
        public static AuditState OnLoadAuditData(AuditState state, LoadAuditDataAction action)
        {
            return new AuditState(true, state.Summaries, state.AuditLogs, state.Metrics);
        }

        [ReducerMethod]
        public static AuditState OnLoadAuditDataSuccess(AuditState state, LoadAuditDataSuccessAction action)
        {
            return new AuditState(false, action.Summaries, action.AuditLogs, action.Metrics);
        }

        [ReducerMethod]
        public static AuditState OnLoadAuditDataFailure(AuditState state, LoadAuditDataFailureAction action)
        {
            return new AuditState(false, state.Summaries, state.AuditLogs, state.Metrics);
        }
    }
}
