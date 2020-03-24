# api/groupship

## Responsibilities
**My** Groupship management

## Filters
### General
`Authentication`
`Groupship - IGroupModel`

### Optional
```
InGroup(bool)
HasUnhandledGroupRequest(bool)
NotBlocked
GroupRequirementType(RelationshipRequirementType)
AtLeastRole(Role)
```

## Actions

### AbandonRequest
```
InGroup(false)
HasUnhandledGroupRequest(true)
```

### JoinDirectly
```
InGroup(false)
GroupRequirementType(Anyone)
```

### JoinWithAnswer
```
InGroup(false)
GroupRequirementType(CorrectAnswerRequired)
```

### // HandleRequest -> group/management

### SendInvestigation
```
InGroup(false)
HasUnhandledGroupRequest(false)
NotBlocked
GroupRequirementType(Investigation)
```

### SendValidation
```
InGroup(false)
HasUnhandledGroupRequest(false)
NotBlocked
GroupRequirementType(ValidationMessageRequired)
```

### SetGroupDisplayName, SetSelfDisplayName
```
InGroup(true)
```

### QuitGroup
- `[InGroup(true)]`
- Should not be founder
- Event: UserQuitGroupEvent{GroupId,UserId,SessionId}

### QuitAdmin
```
InGroup(true)
AtLeastRole(Admin)
```

<!-- ### QuitOwner
```
InGroup(true)
AtLeastRole(Admin)
```
Event: QuitFounderEvent{OldFounder,NewFounder} -->


---
## Responsibilities
I am administrator or founder of group A. I want to modify the attributes of group A.

### Put profile (Name, Description)
```
InGroup(true)
AtLeaseRole(Admin)
```

### Set avatar
```
InGroup(true)
AtLeaseRole(Admin)
```

### Set Requirement
```
InGroup(true)
AtLeaseRole(Founder)
```