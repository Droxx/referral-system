# SnappCar assignment - Referral System

This repository contains a basic referral system for the SnappCar assignment.

## How to use

There are three main endpoints

- POST: `referrals/invite-user`
  - This endpoint allows an existing user to invite a new user by providing their email address.
  - If there are no completed referrals for the invited user. A new entity is created for that referral, and an email is sent to the invited user
- PATCH: `webhooks/user-registered`
  - This is intended to be called as a webhook when a new user registers.
  - The logic here will check if any valid referral exists for the registered user's email.
  - If so, it will mark the most recent referral as `accepted`. And credit the new user's account with 10 credits.
  - All other referrals for that email will be marked as `expired`.
- PATCH: `webhooks/rental-changed`
  - This is intended to be called as a webhook when a rental's status changes.
  - If the rental status changes to `completed`, the system will check if the renter has any accepted referrals.
  - If an accepted referral exists, it will mark the referral as `completed` and credit the referrer's account with an additional 5 credits.

There are additional GET endpoints to retrieve referral information for testing purposes.

## Reasonings
- The repository layer is managed by an interface, and then an abstract base class
  - While my solution only stores referrals (I trust that other services are keeping track of their own entities). I wanted to provide a structure that could be extended to other entities in the future.
  - If, in future, this needed to be extended to a persisted store. The same approach can be taken to extend another base class that implements the repository interface.
- DTO and Contracts are seperated, because keeping distance between the two allows for more flexibility in changing either without affecting the other.
  - This, however, comes with the cost of maintining a mapper layer. Which I have implemented in a basic way. 
- I've kept the business logic in a "use-case" layer to separate it from the controllers. This allows for easier testing and better separation of concerns.
  - Use case interfaces can be mocked. 

## Improvements
- Flesh out my stubbed email service to provide actual examples of how i would call that service.
- Obviously this is a very small scale implementation, done quickly. I would, in a working environment. Consider a much more extensible system, but I would need more context about services I am dependant on.
- Link more closely with the user and rental systems to ensure data consistency. And prevent abuse
- Perhaps work more with the Owner and Rental IDs given in the webhook payload. But for the purposes of this assignment. I made the assumption that the rental service would handle that logic.
- Validation on email addresses
- I made an assumption that rate limiting and authentication are handled outside of this service. In the ingresses of the environment.
- I would refactor the invite flow. I think the way I check for existing referrals could be improved.
- I would add another layer between the use cases and the controllers. To manage DB transactions.
- I would improve the mapping approach. I did this to provide some distance between DTO models and the contracts. But it's more clunky than I would like.
- I would introduce a more robust integration testing system that ran a version of the API service. So we could test endpoints. Rather than just testing use cases in isolation.