#include "Factory.h"

Parent *Factory::GimmeAParent()
{
	return new Parent();
}

Child *Factory::GimmeAChild()
{
	return new Child();
}

Parent *Factory::GimmeAChildAsAParent()
{
	return new Child();
}
