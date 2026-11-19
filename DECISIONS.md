# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved
Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

_(your answer here)_

## 2. Result instead of an exception
Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

_(your answer here)_

## 3. Who knows about EF Core?
Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

_(your answer here)_

## 4. The flow of a request
Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

_(your answer here)_

## 5. What you rejected from what the AI gave you
Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

_(your answer here)_

## 6. If you had 30 more minutes
What would you improve and what risk did you leave in the project?

# DECISIONS

## 1. A rule you moved
I moved R7 (two active bookings of a room cannot overlap). In the starter it was an `if` inside a `foreach` in `Services/BookingService.cs`, and it was also buggy. Now the interval logic is in `Domain/Bookings/DateRange.cs` (`Overlaps`), and the rule itself is in `Domain/Bookings/Booking.cs` (`Booking.Create`). I put it there so that no booking can be created without passing the rule, whichever endpoint or handler is used. The new rules R9, R10 and R11 went into the same place: `Booking.Create`, `Booking.Cancel` and `Room.MaxBookingDuration`. That showed me why the domain is the right home, because each new rule was one `if` and one `Error`, with no change to any endpoint.

## 2. Result instead of an exception
"The room is occupied" is a normal, expected business outcome, not an unexpected failure. With `Result<T>` the method signature shows that it can fail, and the caller is forced to handle the error. Exceptions are for unexpected situations. In my code `Booking.Create` returns `BookingErrors.RoomOccupied` (an `Error` of type `Conflict`). The handler returns it, and the endpoint calls `result.Error.ToProblem()` in `Common/ErrorHttpExtensions.cs`, which maps Conflict to a 409 ProblemDetails response. No `try/catch`, no middleware.

## 3. Who knows about EF Core?
Only `Infrastructure/Persistence`: `RoomHubDbContext`, the `RoomConfiguration` and `BookingConfiguration` classes, `RoomRepository`, `BookingRepository`, `DatabaseInitializer` and `Program.cs` (registration). The domain, the handlers and the endpoints know only `IRoomRepository`, `IBookingRepository` and `IUnitOfWork` from `Domain`. I gain handlers that do not depend on EF Core and can be tested with a fake. I pay with extra interfaces and classes, and a risk of repositories that only forward calls, so I kept only the methods I need. For R9 I added one method, `GetActiveForParticipantAsync`, and nothing else.

## 4. The flow of a request
POST /api/rooms/{roomId}/bookings
-> `Features/Bookings/CreateBooking/CreateBookingEndpoint.Handle`
(JSON -> `Request` -> `CreateBookingCommand`)
-> `ICommandHandler<CreateBookingCommand, CreateBookingResult>` = `CreateBookingHandler.Handle`
-> `IRoomRepository.GetByIdAsync` (`RoomRepository`) -> missing: `RoomErrors.NotFound` (404)
-> `IBookingRepository.GetActiveForRoomAsync` (`BookingRepository`)
-> `IBookingRepository.GetActiveForParticipantAsync` (`BookingRepository`, for R9)
-> `Booking.Create(...)` (`TimeProvider` gives `nowUtc`; uses `DateRange` and `Room.MaxBookingDuration`) -> `Result<Booking>`
-> failure: `result.Error.ToProblem()` -> 400/409
-> success: `IBookingRepository.Add` -> `IUnitOfWork.SaveChangesAsync` (`RoomHubDbContext`) -> SQLite
-> `TypedResults.Created("/api/bookings/{id}", Response)` -> 201

## 5. What you rejected from what the AI gave you
(Write your own example here. It has to be something that really happened in your exam.)
Examples of the kind of thing to write if it is true for you:
- the AI made `Booking` a child of `Room` (`room.Bookings`), but the contract has `GET /api/bookings/{id}` and `POST /api/bookings/{id}/cancel`, so I made `Booking` its own aggregate with its own repository;
- the AI returned too many attendees as 409, but R4 is validation, so I made it a 400.

## 6. If you had 30 more minutes
I would add automated tests for the domain (`Booking.Create` with a fixed `nowUtc`, touching intervals, exactly 4 hours and exactly 2 hours, the third booking on one day, cancelling exactly 60 minutes before the start) and replace `EnsureCreated` with real migrations. Remaining risks: two simultaneous requests could both pass the overlap check or the R9 daily limit before either saves (a race condition, with no database-level constraint to stop it). Also, `GetActiveForParticipantAsync` loads all active bookings of a participant and the day is filtered in memory, which is fine for a small exam project but would not scale, and `bookedBy` is compared case-sensitively, so "Ana" and "ana" count as two different people.
