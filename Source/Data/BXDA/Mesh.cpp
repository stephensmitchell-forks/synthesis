#include "Mesh.h"

using namespace BXDA;

Mesh::Mesh(Guid guid) : guid(guid)
{}

void Mesh::addSubMesh(const SubMesh & submesh)
{
	subMeshes.push_back(std::make_shared<SubMesh>(submesh));
}

void Mesh::addSubMesh(std::shared_ptr<SubMesh> submesh)
{
	subMeshes.push_back(submesh);
}

void BXDA::Mesh::addPhysics(const Physics & physics)
{
	this->physics += physics;
}

Guid Mesh::getGUID() const
{
	return guid;
}

int Mesh::getVersion() const
{
	return CURRENT_VERSION;
}

void Mesh::calculateWheelShape(Vector3<> axis, Vector3<> origin, double & maxRadius, double & minWidth, double & maxWidth) const
{
	maxRadius = 0.0;
	minWidth = 0.0;
	maxWidth = 0.0;

	bool first = true;

	for (std::shared_ptr<SubMesh> subMesh : subMeshes)
	{
		double radius;
		double width;
		
		subMesh->calculateWheelShape(axis, origin, radius, width);

		if (first || radius > maxRadius)
			maxRadius = radius;
		if (first || width < minWidth)
			minWidth = width;
		if (first || width > maxWidth)
			maxWidth = width;

		first = false;
	}
}

std::string Mesh::toString()
{
	return "BXDA::Mesh: " + guid.toString() + ", Sub-Meshes: " + std::to_string(subMeshes.size()) + ", Physics Properties: (" + physics.toString() + ")";
}

void Mesh::write(BinaryWriter & output) const
{
	// Output general information
	output.write(CURRENT_VERSION);
	output.write(guid.toString());

	// Output meshes
	output.write((int)subMeshes.size());
	for (std::shared_ptr<SubMesh> submesh : subMeshes)
		output.write(*submesh);

	// Output colliders
	output.write((int)subMeshes.size());
	SubMesh tempColliderMesh = SubMesh();
	for (std::shared_ptr<SubMesh> submesh : subMeshes)
	{
		submesh->getConvexCollider(tempColliderMesh);
		output.write(tempColliderMesh);
	}

	// Output physics data
	output.write(physics);
}
