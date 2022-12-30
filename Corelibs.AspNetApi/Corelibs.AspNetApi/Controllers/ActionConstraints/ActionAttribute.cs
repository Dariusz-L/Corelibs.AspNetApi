using Microsoft.AspNetCore.Mvc;

namespace Corelibs.AspNetApi.Controllers.ActionConstraints
{
    public class ActionAttribute : QueryParameterConstraintAttribute
    {
        public ActionAttribute(string parameterValue) : base("action", parameterValue) { }
    }

    public class Route_ID : RouteAttribute
    {
        public Route_ID() : base("{id}") { }
    }

    public class Action_CreateAttribute : ActionAttribute
    {
        public Action_CreateAttribute() : base("create") {}
    }

    public class Action_DeleteAttribute : ActionAttribute
    {
        public Action_DeleteAttribute() : base("delete") { }
    }

    public class Action_EditAttribute : ActionAttribute
    {
        public Action_EditAttribute() : base("edit") { }
    }

    public class Action_ChangeNameAttribute : ActionAttribute
    {
        public Action_ChangeNameAttribute() : base("changeName") { }
    }

    public class Action_ChangeDescriptionAttribute : ActionAttribute
    {
        public Action_ChangeDescriptionAttribute() : base("changeDescription") { }
    }
}
