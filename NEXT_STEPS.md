# api-orders - next steps

Current phase: pre-MVP, first vertical slice.

The project has the domain model and a thin application layer, but the API is not yet a usable product. The shortest path to an MVP is to make purchase-order creation work end to end, then add the minimum read path and lock it down with tests.

## What to focus on today

Time budget: about 3 hours.

1. Complete purchase-order creation through HTTP.
   - Add the `POST /purchase-orders` endpoint.
   - Return a useful success response with the new order id.
   - Make sure the order total is correct.
   - Keep the domain rules as the source of truth.

2. Add the minimum read path.
   - Expose a way to fetch a purchase order by id.
   - Expose enough catalog/supplier data to create a valid order.
   - Keep the read model simple.

3. Tighten tests and API behavior.
   - Fix the failing supplier test for the right reason.
   - Add coverage for the create-order flow.
   - Make validation and not-found responses predictable.

## Suggested order

1. Start with the create-order vertical slice.
2. Then add the read endpoint for the created order.
3. Then add the supplier/catalog read surface.
4. Finish with tests and error handling.

## Working rule

Prefer the simplest architecture that satisfies the current requirement. Do not add extra abstractions, persistence layers, or admin features until the first slice is usable.

## When you come back

Ask for:

- `challenge mode` to get the next task;
- `review` to check your implementation;
- `architect mode` if you want the design revisited.

