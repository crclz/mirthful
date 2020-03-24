# api/friendship

## Filters

### General
`Authentication`
`Friendship`
### Optinal
```
HasUnhandledRequest(Who, bool)
SelfNotBlocked
UserRequirementType(RelationshipRequirementType)
AreFriends(bool)
```

## Actions

### AbandonRequest
```
[HasUnhandledRequest(Target, true)]
```

### AddDirectly
```
[AreFriends(false)]
[UserRequirementType(Anyone)]
```

### AddWithAnswer
```
[AreFriends(false)]
[UserRequirementType(CorrectAnswerRequired)]
```

### HandleRequest
```
[AreFriends(false)]
[HasUnhandledRequest(Self, true)]
```

### SendInvestigation
```
[AreFriends(false)]
[HasUnhandledRequest(Target, false)]
[SelfNotBlocked]
[UserRequirementType(InvestigationRequired)]
```

### SendValidation
```
[AreFriends(false)]
[HasUnhandledRequest(Target, false)]
[SelfNotBlocked]
[UserRequirementType(ValidationMessageRequired)]
```

### SetNickname
```
[AreFriends(true)]
```

### QuitFriendship
```
[AreFriends(true)]
```