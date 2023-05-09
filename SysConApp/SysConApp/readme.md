
A pattern for chatCompletions. Note if the closing system message contradicts the transcript you might be told, so if it is dynamically created and it is going to, then you might consider clearing the transcript. You probably only want to be keeping a recent window of messages anyhow.

system: "Assistant is a large language model trained by OpenAI."
user:			#### the conversation transcript #### 
assistant:		#
user:			#
assistant:		#
user:			#
assistant:		#
system: "Answer the user's question using ONLY this content: {context}. If you cannot answer the question, say 'Sorry, I don't know the answer to this one'"


Example 1:

system: "Assistant is a large language model trained by OpenAI."
user:  what is a Pagu Club Cocktail made from?
system: "Answer the user's question using ONLY this content: EMPTY. If you cannot answer the question, say 'Sorry, I don't know the answer to this one'"

response: Sorry, I don't know the answer to this one

Example 2:

system: "Assistant is a large language model trained by OpenAI."
user:  what is a Pagu Club Cocktail made from?
system: "Answer the user's question using ONLY this content: a Pagu Club Cocktail is made from Gin, Triple Sec, Lime Juice and both Aromatic and Orange Bitters. If you cannot answer the question, say 'Sorry, I don't know the answer to this one'"

response: A Pagu Club Cocktail is made from Gin, Triple Sec, Lime Juice and both Aromatic and Orange Bitters.

Don't be tempted to add multiple system messages structured like this. They would inherently contradict each other. You just want to increase what you are adding to the {context} parameter.

The response 'Sorry, I don't know the answer to this one' can obviously be intercepted by your code and the request re-written and re-executed appropriately.