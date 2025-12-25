EXTERNAL StartQuest(questId)
EXTERNAL AdvanceQuest(questId)
EXTERNAL FinishQuest(questId)

// quest ids (questId + "Id" for variable name)
VAR DeliveryQuest = "DeliveryQuest"
VAR DeliveryCheckPointsQuest = "DeliveryCheckPointsQuest"
VAR SimpleDeliveryQuest = "SimpleDeliveryQuest"
VAR DeliveryQuestCheckPoint2 = "DeliveryQuestCheckPoint2"

//quest states (quest id + "State")
VAR DeliveryQuestState = "REQUIREMENTS_NOT_MET"
VAR DeliveryCheckPointsQuestState = "REQUIREMENTS_NOT_MET"
VAR SimpleDeliveryQuestState = "REQUIREMENTS_NOT_MET"
VAR DeliveryQuestCheckPoint2State = "REQUIREMENTS_NOT_MET"

=== DeliveryQuestStart ===
{ DeliveryQuestState:
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> END
}

= requirementsNotMet
Come back once you've made all the necessary deliveries.
-> END

= canStart
Hey you're the delivery guy right? Can you take this package and deliver it to this location?
* [Yes]
    Great! Thank you this will help out the community a lot.
    ~ StartQuest(DeliveryQuest)
    -> END
* [No]
    Oh, ok. You can come back any time you'd like.
    -> END

= inProgress
How's the progress?
-> END

= canFinish
Cool you got the delivery, now take it to my friend.
-> END

= finished
Thank you for the delivery.
-> END

=== DeliveryQuestFinish ===
{DeliveryQuestState:
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> default
}

= canFinish
Thank you for delivering the package! You've done us a great service.
Before you go, can I ask for your opinion? The community is voting on what to improve next in the neighborhood.
* [Improve the roads]
    That's practical thinking. Better roads mean safer deliveries for everyone!
    -> questComplete
* [Add more street lights]
    Safety first, I like it. More lights will help drivers navigate at night.
    -> questComplete
* [Build a community center]
    Bringing people together, I respect that. We could use a place to gather.
    -> questComplete

= questComplete
Thanks for your input. Every voice matters in our community!
~ FinishQuest(DeliveryQuest)
-> END

= finished
Thanks again for the delivery!
-> END

= default
Hm? What do you want? I wasn't expecting any deliveries.
-> END

=== DeliveryCheckPointsQuestStart ===
{DeliveryCheckPointsQuestState:
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> default
}

= requirementsNotMet
-> END
= canStart
I need you to deliver this package through three checkpoints. You'll get bonus time at each checkpoint!
+ [Accept quest]
     Great! Pick up the box and drive through all three checkpoints before delivering it!
    ~ StartQuest(DeliveryCheckPointsQuest)
    -> END
+ [Decline]
    Maybe later.
    -> END

= inProgress
Hurry! Drive through the checkpoints before time runs out!
-> END

= canFinish
#This should not appear - delivery auto-finishes
-> END

= finished
Thanks for the delivery!
-> END

= default
What do you want?
-> END

=== DeliveryCheckPointsQuestFinish ===
{DeliveryCheckPointsQuestState:
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> default
}

= requirementsNotMet
-> END

= canFinish
Perfect delivery! You hit all the checkpoints in time!
Hey, quick question - we're planning a community event. What would you like to see?
* [Street racing tournament]
    Now that's exciting! Fast cars, friendly competition. I'm in!
    -> questComplete
* [Food truck festival]
    Delicious idea! Nothing brings people together like good food.
    -> questComplete
* [Outdoor movie night]
    Classic choice! A great way to relax after a long day of deliveries.
    -> questComplete

= questComplete
Awesome! We'll consider that for the next event. Thanks for your time!
~ FinishQuest(DeliveryCheckPointsQuest)
-> END

= finished
Great work on that checkpoint delivery!
-> END

= default
I'm not expecting any deliveries right now.
-> END

=== SimpleDeliveryQuestStart ===
{SimpleDeliveryQuestState:
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> default
}

= requirementsNotMet
Come back once you've made all the necessary deliveries for today.
-> END

= canStart
Hey! I need you to deliver this package. It's not urgent, so take your time!
+ [Accept quest]
    Great! Pick up the box and deliver it when you're ready.
    ~ StartQuest(SimpleDeliveryQuest)
    -> END
+ [Decline]
    Maybe later.
    -> END

= inProgress
The box is ready for pickup. Deliver it whenever you can!
-> END

= canFinish
You shouldn't see this message.
-> END

= finished
Thanks for the delivery!
-> END

= default
What do you need?
-> END

=== SimpleDeliveryQuestFinish ===
{SimpleDeliveryQuestState:
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> default
}

= canFinish
Perfect! Thanks for bringing this over!
Oh, one more thing - the neighborhood council is discussing traffic improvements. What's your take?
* [Add speed bumps for safety]
    Good thinking. Slowing down cars will protect pedestrians and kids.
    -> questComplete
* [Widen the main road]
    More space means less congestion. Smart for delivery drivers like you!
    -> questComplete
* [Install traffic lights at intersections]
    Organization is key. Traffic lights will keep things flowing smoothly.
    -> questComplete

= questComplete
I appreciate your perspective. Your experience on the road really helps!
~ FinishQuest(SimpleDeliveryQuest)
-> END

= finished
Great work on that delivery!
-> END

= default
I'm not expecting anything right now.
-> END

=== DeliveryQuestCheckPoint2Start ===
{DeliveryQuestCheckPoint2State:
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> default
}

= requirementsNotMet
-> END

= canStart
I need you to deliver another package through three checkpoints. You'll get bonus time at each checkpoint!
+ [Accept quest]
    Great! Pick up the box and drive through all three checkpoints before delivering it!
    ~ StartQuest(DeliveryQuestCheckPoint2)
    -> END
+ [Decline]
    Maybe later.
    -> END

= inProgress
Hurry! Drive through the checkpoints before time runs out!
-> END

= canFinish
-> END

= finished
Thanks for the delivery!
-> END

= default
What do you want?
-> END

=== DeliveryQuestCheckPoint2Finish ===
{DeliveryQuestCheckPoint2State:
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> default
}

= requirementsNotMet
-> END

= canFinish
Perfect delivery! You hit all the checkpoints in time!
Real quick - we're planning neighborhood improvements. What should we prioritize?
* [Plant more trees and green spaces]
    Environmental choice! Trees make the streets more pleasant and cleaner air for everyone.
    -> questComplete
* [Repair damaged sidewalks]
    Practical and necessary. Safe walkways benefit the whole community.
    -> questComplete
* [Add bike lanes]
    Forward thinking! Alternative transportation options are always good.
    -> questComplete

= questComplete
Thanks for weighing in! The community appreciates drivers like you who care.
~ FinishQuest(DeliveryQuestCheckPoint2)
-> END

= finished
Great work on that checkpoint delivery!
-> END

= default
I'm not expecting any deliveries right now.
-> END
