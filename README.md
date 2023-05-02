# DotNetChat
LangChain style Chat Agents in Dot Net 

The C# code here is basically equivallent to the following Python:

```Python
from langchain.agents import AgentType
from langchain.memory import ConversationBufferMemory
from langchain.agents import initialize_agent
from langchain.chat_models import ChatOpenAI
from langchain.agents import tool

@tool
def order_info(args: str) -> str:
    """Provides information about where to buy a particular book and how much it costs."""
    return args + " is available on Amazon.com at a cost of $50."

@tool(return_direct=True)
def purchase_book(args: str) -> str:
    """Executes a purchase of a new book from Amazon.com."""
    return "Your book '" + args + "' has been ordered from Amazon.com."

@tool
def cancel_order(args: str) -> str:
    """Cancels an order for a particular book. For example, if the book is no longer wanted. Input is the title of the book."""
    return "The order for the book '" + args + "' has been cancelled."

def step(agent, input):
    print("user: " + input)
    print("assistant: " + agent.run(input))

tools = [
    order_info,
    purchase_book,
    cancel_order
]

memory = ConversationBufferMemory(memory_key="chat_history", return_messages=True)
llm=ChatOpenAI(temperature=0)
agent = initialize_agent(tools, llm, agent=AgentType.CHAT_CONVERSATIONAL_REACT_DESCRIPTION, verbose=True, memory=memory)

step(agent, "I am thinking about buying a copy of the Python Cookbook where is it available?")
step(agent, "ok that sounds good, order me a copy")
step(agent, "actually I changed my mind, I don't want that book after all")
```
