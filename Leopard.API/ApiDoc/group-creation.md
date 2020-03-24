# api/groupship

## Responsibilities
Create group

## Filters
### Common
- [ServiceFilter(typeof(AuthenticationFilter))]

## Actions

### Create
- Event: GroupCreatedEvent{GroupId, DesiredSessionId, FounderId} // handled by Session, SessionMember, Groupship