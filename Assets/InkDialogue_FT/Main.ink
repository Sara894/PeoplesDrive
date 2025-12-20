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
~ FinishQuest(DeliveryQuestCheckPoint2)
-> END

= finished
Great work on that checkpoint delivery!
-> END

= default
I'm not expecting any deliveries right now.
-> END