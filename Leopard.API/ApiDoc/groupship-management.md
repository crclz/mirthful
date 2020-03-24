# api/groupship-management

## Responsibilities
I am administrator or founder of group A. I want to manage someone's relationship with group A.

## Filters

### General
`Authentication`
`GroupshipManagement - IGroupshipManagementModel :IGroupModel`
- check group exist
- get my dealer & append
- check I am at least admin
- get or create its dealer & append

```cs
interface IGroupshipManagementModel: IGroupModel
{
    string ItId;
}
```

### Optional
```
ItInGroup(bool)
```

## Actions

### HandleRequest
- `[ItInGroup(false)]`
- It has unhandled request

### AssignAdmin
- `[ItInGroup(true)]`
- It's role is Normal

### Kick
- `[ItInGroup(true)]`
- It's role is Normal