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
Driver, I need you to deliver this medicine. Make haste and good luck!
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
Thank you so much for bringing the medicine. We managed to save the patient just in time.
Now that the crisis is over, the neighborhood is discussing how to prevent situations like this.
* [Improve local clinics]
    Better equipment and space would let us treat more people.
    -> questComplete
* [Faster emergency routes]
    Clearer roads could save lives in emergencies..
    -> questComplete
* [Community health volunteers]
    Training locals to help in emergencies could make a big difference.
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
Hello, driver. I need you to deliver this hot soup to the children’s shelter. Please try to get there quickly so it doesn’t get cold.
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
Thank you, driver. Be sure to stop by for some soup next time.
Since you help so many families, we’d love your thoughts on how to support children in this area.
* [Improve the shelter building]
    A warmer, safer place would mean a lot to the kids.
    -> questComplete
* [Add a small outdoor play area]
    Fresh air and space to play helps them feel like kids again.
    -> questComplete
* [Provide school supplies]
    Giving them tools to learn gives them hope for the future.
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
Hello, dear. It’s so nice to see you. Today, I need you to deliver this food to the family you help once every week. It's not urgent, so take your time!
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
Thank you so much.Without you, we wouldn’t be able to get by like this.
You see so many parts of the neighborhood… maybe you could help us decide what to focus on next. What's your take?
* [Improve the roads]
    Better roads would make it easier for deliveries like yours to reach everyone safely.
    -> questComplete
* [Add more street lights]
    More light at night would help protect families and keep the streets safer.
    -> questComplete
* [Build a small community pantry]
    A shared pantry would help people who can’t always afford enough food.
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
Hello, dear. How are you today? I need you to deliver these clothes and tents to the homeless. Please try to get there before it gets dark so they can set everything up in time.!
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
It may not be a real roof, but it’s something to keep me safe. Thank you… truly.
People like you remind us that we’re not invisible. If you could change one thing here, what would it be?
* [Build temporary housing]
   A place to sleep would give people a chance to rebuild.
    -> questComplete
* [Create protected green shelters]
    Safe, clean spaces can offer dignity and comfort.
    -> questComplete
* [Expand community outreach]
    More people helping means fewer people being left behind.
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
